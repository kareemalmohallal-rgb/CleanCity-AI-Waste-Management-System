using System.Net.Http.Headers;
using System.Text.Json;
using CleanCity.Application.DTOs.AI;
using CleanCity.Application.Interfaces.Services;

namespace CleanCity.Infrastructure.AI
{
    public class PythonWasteImageValidationService : IWasteImageValidationService
    {
        private readonly HttpClient _httpClient;

        public PythonWasteImageValidationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WasteImageValidationResult> ValidateAsync(
            Stream imageStream,
            string fileName,
            string contentType,
            CancellationToken ct = default)
        {
            using var form = new MultipartFormDataContent();
            using var fileContent = new StreamContent(imageStream);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            form.Add(fileContent, "file", fileName);

            var response = await _httpClient.PostAsync("/predict-waste", form, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"AI service failed: {response.StatusCode} - {body}");

            var result = JsonSerializer.Deserialize<WasteImageValidationResult>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                throw new InvalidOperationException("Invalid AI response.");

            result.RawResponse = body;
            return result;
        }
    }
}