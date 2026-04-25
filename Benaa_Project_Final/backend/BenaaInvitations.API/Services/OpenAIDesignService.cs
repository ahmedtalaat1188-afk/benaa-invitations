using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BenaaInvitations.API.Services
{
    public class OpenAIDesignService : IAIDesignService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAIDesignService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAIApiKey"] ?? "";
        }

        public async Task<string> GenerateInvitationBackgroundAsync(string prompt)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("YOUR_OPENAI_API_KEY"))
            {
                // Fallback for simulation/demo if no key provided
                return "https://img.freepik.com/free-vector/graduation-party-invitation-template_23-2148906560.jpg";
            }

            var requestBody = new
            {
                model = "dall-e-3",
                prompt = $"Arabic invitation background for a school event. Theme: {prompt}. High resolution, elegant, no text, professional design.",
                n = 1,
                size = "1024x1024",
                quality = "standard"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/images/generations");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenAI Error: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("data")[0].GetProperty("url").GetString() ?? "";
        }
    }
}
