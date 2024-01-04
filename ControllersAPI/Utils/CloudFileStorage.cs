using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ControllersAPI.Utils;

public sealed class CloudFileStorage(IConfiguration configuration) : IFileStorage
{
    private readonly string _connectionString = configuration.GetConnectionString("AzureConnection")!;

    /// <inheritdoc/>
    public async Task<string> SaveFile(string container, IFormFile file)
    {
        var client = new BlobContainerClient(_connectionString, container);
        await client.CreateIfNotExistsAsync();
        client.SetAccessPolicy(PublicAccessType.Blob);

        string extension = Path.GetExtension(file.FileName);
        string fileName = $"{Guid.NewGuid()}{extension}";

        BlobClient blob = client.GetBlobClient(fileName);
        await blob.UploadAsync(file.OpenReadStream());

        return blob.Uri.ToString();
    }

    /// <inheritdoc/>
    public async Task DeleteFile(string path, string container)
    {
        if (string.IsNullOrEmpty(path))
            return;

        BlobContainerClient client = new BlobContainerClient(_connectionString, container);
        await client.CreateIfNotExistsAsync();

        string fileName = Path.GetFileName(path);
        BlobClient blob = client.GetBlobClient(fileName);
        await blob.DeleteIfExistsAsync();
    }

    /// <inheritdoc/>
    public async Task<string> EditFile(string container, IFormFile file, string path)
    {
        await DeleteFile(path, container);
        return await SaveFile(container, file);
    }
}