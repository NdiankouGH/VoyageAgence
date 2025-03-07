using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VoyageAgence.Data;
using VoyageAgence.Models;
using VoyageAgence.services;
using Microsoft.AspNetCore.Identity.UI.Services;
using VoyageAgence.Services; // Add this line

var builder = WebApplication.CreateBuilder(args);

// Ajouter la connexion à la base de données
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<Utilisateur>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Ajout des rôles
    .AddEntityFrameworkStores<ApplicationDbContext>();
    
builder.Services.AddControllersWithViews();

// Enregistrer WhatsAppService
builder.Services.AddHttpClient<WhatsAppService>();

// Register the email sender service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

// ...existing code...
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var supportedCultures = new[] { "en", "fr" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
    options.SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
});

var app = builder.Build();

/// <summary>
/// Initialiser les rôles et créer l'administrateur par défaut
/// </summary>
static async Task InitializeRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<Utilisateur>>();

    // Définition des rôles
    string[] roleNames = { "Admin", "Gestionnaire", "Client" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Vérifier si l'administrateur existe déjà
    string adminEmail = "ndiankou12345@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        // Création de l'admin s'il n'existe pas
        adminUser = new Utilisateur
        {
            UserName = adminEmail,
            Email = adminEmail,
            Nom = "Papis",
            Prenom = "Ndoye",
            NumTel = "+221770931244"
        };

        var result = await userManager.CreateAsync(adminUser, "Fatndoye12");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

// Exécuter l'initialisation des rôles et de l'admin au démarrage
using (var scope = app.Services.CreateScope())
{
    await InitializeRolesAndAdmin(scope.ServiceProvider);
}

// Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ...existing code...

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
