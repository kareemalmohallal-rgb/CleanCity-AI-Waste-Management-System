using CleanCity.Application.Interfaces.Storage;

namespace CleanCity.Infrastructure.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _uploadsRoot; 
    private readonly string _publicBase;  

    public LocalFileStorage(string uploadsRoot, string publicBase = "/uploads")
    {
        _uploadsRoot = uploadsRoot;
        _publicBase = publicBase.TrimEnd('/');
    }

    public async Task<StoredFileResult> SaveAsync(
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken ct)
    {
        
        var ext = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".bin";

        var safeName = $"{Guid.NewGuid():N}{ext}".ToLowerInvariant();

        var folder = Path.Combine("reports", DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"));
        var physicalDir = Path.Combine(_uploadsRoot, folder);
        Directory.CreateDirectory(physicalDir);

        var relativePath = Path.Combine(folder, safeName).Replace("\\", "/");
        var physicalPath = Path.Combine(_uploadsRoot, folder, safeName);

        await using var fs = new FileStream(physicalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(fs, ct);

        var publicUrl = $"{_publicBase}/{relativePath}";
        return new StoredFileResult(originalFileName, relativePath, publicUrl);
    }

    public Task DeleteAsync(string relativePath, CancellationToken ct)
    {
        var physicalPath = Path.Combine(_uploadsRoot, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        if (File.Exists(physicalPath)) File.Delete(physicalPath);
        return Task.CompletedTask;
    }
}