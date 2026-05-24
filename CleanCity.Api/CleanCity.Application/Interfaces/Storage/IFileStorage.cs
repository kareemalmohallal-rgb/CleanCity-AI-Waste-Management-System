namespace CleanCity.Application.Interfaces.Storage;

public sealed record StoredFileResult(
    string OriginalFileName,
    string RelativePath, // مثال: reports/2026/02/xxx.jpg
    string PublicUrl     // مثال: /uploads/reports/2026/02/xxx.jpg
);

public interface IFileStorage
{
    Task<StoredFileResult> SaveAsync(
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken ct);

    Task DeleteAsync(string relativePath, CancellationToken ct);
}