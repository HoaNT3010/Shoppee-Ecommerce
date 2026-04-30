using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.Storage;
using ShoppeeEcommerce.Domain.Exceptions;

namespace ShoppeeEcommerce.Infrastructure.Storage.CloudinaryFS
{
    internal class CloudinaryFileService : IFileService
    {
        readonly Cloudinary _cloudinary;
        readonly ILogger<CloudinaryFileService> _logger;

        public CloudinaryFileService(
            Cloudinary cloudinary,
            ILogger<CloudinaryFileService> logger)
        {
            _cloudinary = cloudinary;
            _logger = logger;
        }

        public async Task DeleteAsync(string publicId,
            CancellationToken cancellationToken = default)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error is not null)
            {
                _logger.LogError("Cloudinary delete failed for {PublicId}: {Error}",
                    publicId, result.Error.Message);
                throw new FileUploadException(
                    $"Failed to delete file '{publicId}': {result.Error.Message}");
            }
            _logger.LogInformation("Deleted file {PublicId} from Cloudinary", publicId);
        }

        public async Task DeleteManyAsync(IEnumerable<string> publicIds,
            CancellationToken cancellationToken = default)
        {
            var tasks = publicIds.Select(id => DeleteAsync(id, cancellationToken));
            await Task.WhenAll(tasks);
        }

        public async Task<FileUploadResult> UploadAsync(
            Stream fileStream,
            string fileName,
            string? folder = null,
            CancellationToken cancellationToken = default)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = folder,
                // Let Cloudinary generate a unique id
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false,
                Transformation = new Transformation()
                // auto-optimize quality
                .Quality("auto")
                // serve webp/avif where supported
                .FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

            if (result.Error is not null)
            {
                _logger.LogError("Cloudinary upload failed for {FileName}: {Error}",
                    fileName, result.Error.Message);
                throw new FileUploadException(
                    $"Failed to upload file '{fileName}': {result.Error.Message}");
            }
            _logger.LogInformation("Uploaded file {FileName} to Cloudinary. PublicId: {PublicId}",
                fileName, result.PublicId);

            return new FileUploadResult(
                Url: result.SecureUrl.ToString(),
                PublicId: result.PublicId,
                Format: result.Format,
                Size: result.Bytes);
        }

        public async Task<IReadOnlyList<FileUploadResult>> UploadManyAsync(
            IEnumerable<(Stream Stream, string FileName)> files,
            string? folder = null,
            CancellationToken cancellationToken = default)
        {
            // Materialized list to preserve file order
            var fileList = files.ToList();
            // Upload in parallel but cap concurrency to avoid hammering the API
            var semaphore = new SemaphoreSlim(3);
            var tasks = fileList.Select(async file =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    return await UploadAsync(file.Stream, file.FileName, folder, cancellationToken);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            return await Task.WhenAll(tasks);
        }
    }
}
