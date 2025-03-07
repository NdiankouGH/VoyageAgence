// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using VoyageAgence.Models; // Ajout de l'espace de noms pour la classe Utilisateur
using VoyageAgence.services;
using VoyageAgence.Services; // Add this line
    
namespace VoyageAgence.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly IUserStore<Utilisateur> _userStore;
        private readonly IUserEmailStore<Utilisateur> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly WhatsAppService _whatsAppService;


        public RegisterModel(
            UserManager<Utilisateur> userManager,
            IUserStore<Utilisateur> userStore,
            SignInManager<Utilisateur> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            WhatsAppService whatsAppService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _whatsAppService = whatsAppService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Nom")]
            public string Nom { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Prénom")]
            public string Prenom { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Numéro de téléphone")]
            public string NumTel { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Le {0} doit contenir au moins {2} et au maximum {1} caractères.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmer le mot de passe")]
            [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.Nom = Input.Nom;
                user.Prenom = Input.Prenom;
                user.NumTel = Input.NumTel;


                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "Client"); // Attribuer le rôle Client par défaut


                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //Envoie notification whatsapp
                    // Envoi du message WhatsApp après l'inscription réussie
                    string message = $"Bonjour {user.Prenom} {user.Nom}, votre inscription a été confirmée !";
                    var whatsappResult = await _whatsAppService.SendWhatsAppMessageAsync(user.NumTel, message);

                    if (!whatsappResult.Success)
                    {
                        _logger.LogError($"Échec de l'envoi du message WhatsApp : {whatsappResult.Error}");
                    }
                    else
                    {
                        _logger.LogInformation($"Message WhatsApp envoyé avec succès. ID: {whatsappResult.MessageId}");
                    }


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private Utilisateur CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Utilisateur>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Utilisateur)}'. " +
                    $"Ensure that '{nameof(Utilisateur)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Utilisateur> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Utilisateur>)_userStore;
        }
    }
}