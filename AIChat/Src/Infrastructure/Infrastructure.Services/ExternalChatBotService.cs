using Application.Service.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class ExternalChatBotService : IExternalChatBotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _thirdPartyApiUrl;
        private readonly ILogger<ExternalChatBotService> _logger;

        public ExternalChatBotService(HttpClient httpClient, IConfiguration configuration, ILogger<ExternalChatBotService> logger)
        {
            _httpClient = httpClient;
            _thirdPartyApiUrl = configuration["ThirdPartyApiUrl"];
            _logger = logger;
        }

        public async Task<string> GetChatResponseAsync(string question)
        {
            var requestUri = $"{_thirdPartyApiUrl}?text={Uri.EscapeDataString(question)}";

            try
            {
                using var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync(); // JSON response
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                if (jsonDocument.RootElement.TryGetProperty("response", out var answer) ||
                    jsonDocument.RootElement.TryGetProperty("message", out answer))
                {
                    return answer.GetString();
                }
                return string.Empty; // Return empty string if "response" property is not found
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error calling third-party API: {ex.Message}");
                return string.Empty; // Return empty string if request fails
            }
        }
    }
}
