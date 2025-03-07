using System.Text;
using System.Text.Json;
using PhoneNumbers;

namespace VoyageAgence.services
{
    public class WhatsAppServiceResult
    {
        public bool Success { get; set; }
        public string MessageId { get; set; }
        public string Error { get; set; }
    }

    public class WhatsAppService
    {
        private readonly string _instanceId;
        private readonly string _token;
        private readonly ILogger<WhatsAppService> _logger;
        private readonly PhoneNumberUtil _phoneUtil;
        private readonly HttpClient _httpClient;

        public WhatsAppService(IConfiguration configuration, ILogger<WhatsAppService> logger, HttpClient httpClient)
        {
            _instanceId = configuration["Ultramsg:InstanceId"]
                ?? throw new ArgumentNullException("Ultramsg:InstanceId configuration is missing");
            _token = configuration["Ultramsg:Token"]
                ?? throw new ArgumentNullException("Ultramsg:Token configuration is missing");
            _logger = logger;
            _phoneUtil = PhoneNumberUtil.GetInstance();
            _httpClient = httpClient;
        }

        public async Task<WhatsAppServiceResult> SendWhatsAppMessageAsync(string phoneNumber, string message)
        {
            try
            {
                // Format phone number
                var formattedNumber = FormatPhoneNumber(phoneNumber);
                if (formattedNumber == null)
                {
                    return new WhatsAppServiceResult
                    {
                        Success = false,
                        Error = "Numéro de téléphone invalide"
                    };
                }

                // URL de l'API Ultramsg
                var url = $"https://api.ultramsg.com/{_instanceId}/messages/chat";

                // Corps de la requête
                var requestBody = new
                {
                    token = _token,
                    to = formattedNumber,
                    body = message
                };

                // Convertir en JSON
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Envoyer la requête
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode(); // Lève une exception si le statut n'est pas 200-299

                // Lire la réponse
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<UltramsgResponse>(responseContent);

                if (responseData?.Sent == "true")
                {
                    _logger.LogInformation("Message WhatsApp envoyé avec succès. ID: {MessageId}", responseData.Id);
                    return new WhatsAppServiceResult
                    {
                        Success = true,
                        MessageId = responseData.Id
                    };
                }
                else
                {
                    _logger.LogError("Erreur lors de l'envoi du message WhatsApp. Réponse: {Response}", responseContent);
                    return new WhatsAppServiceResult
                    {
                        Success = false,
                        Error = "Erreur lors de l'envoi du message"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du message WhatsApp");
                return new WhatsAppServiceResult
                {
                    Success = false,
                    Error = $"Erreur lors de l'envoi du message: {ex.Message}"
                };
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            try
            {
                // Assume French number if no country code is provided
                if (!phoneNumber.StartsWith("+"))
                {
                    phoneNumber = "+221" + (phoneNumber.StartsWith("0") ? phoneNumber.Substring(1) : phoneNumber);
                }

                var number = _phoneUtil.Parse(phoneNumber, "SN");
                if (!_phoneUtil.IsValidNumber(number))
                {
                    _logger.LogWarning($"Numéro invalide: {phoneNumber}");
                    return null;
                }

                var formatted = _phoneUtil.Format(number, PhoneNumberFormat.E164);
                _logger.LogInformation($"Numéro formaté: {formatted}");
                return formatted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du formatage du numéro de téléphone");
                return null;
            }
        }

        // Classe pour désérialiser la réponse Ultramsg
        private class UltramsgResponse
        {
            public string Sent { get; set; }
            public string Id { get; set; }
        }
    }
}