using CleanCity.Application.DTOs.AI;

namespace CleanCity.Application.Interfaces.Services
{
    public interface IWasteImageValidationService
    {
        Task<WasteImageValidationResult> ValidateAsync(
            Stream imageStream,
            string fileName,
            string contentType,
            CancellationToken ct = default);
    }
}