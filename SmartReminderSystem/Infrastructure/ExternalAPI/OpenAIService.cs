using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public interface IOpenAIService
{
    Task<string> ParseAppointmentAsync(string userInput);
}

public class OpenAIService : IOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI API key is missing.");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> ParseAppointmentAsync(string userInput)
    {
        var requestBody = new
        {
            model = "gpt-4o", // ✅ Ensure you're using an available model
            messages = new[]
            {
            new { role = "system", content = "You are an assistant that extracts structured data from natural language for medical appointments." },
            new { role = "user", content = $"Extract appointment details: {userInput}" }
        },
            temperature = 0.2
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        // ✅ Correct API call to generate a response
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"OpenAI API Response: {responseBody}"); // ✅ Debugging log

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"OpenAI API request failed: {response.StatusCode} - {responseBody}");
        }

        var result = JsonSerializer.Deserialize<OpenAIResponse>(responseBody);

        if (result == null || result.Choices == null || !result.Choices.Any())
        {
            throw new Exception("OpenAI API response is empty or invalid.");
        }

        return result.Choices.First().Message.Content ?? string.Empty;
    }

}

public class OpenAIResponse
{
    public List<Choice> Choices { get; set; }
}

public class Choice
{
    public Message Message { get; set; }
}

public class Message
{
    public string Content { get; set; }
}