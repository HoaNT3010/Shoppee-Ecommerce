namespace ShoppeeEcommerce.Application.Abstractions.Storage
{
    public sealed record FileUploadResult(
        string Url,
        string PublicId,
        string Format,
        long Size
    );

    public interface IFileService
    {
        Task<FileUploadResult> UploadAsync(
            Stream fileStream,
            string fileName,
            string? folder = null,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string publicId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FileUploadResult>> UploadManyAsync(
            IEnumerable<(Stream Stream, string FileName)> files,
            string? folder = null,
            CancellationToken cancellationToken = default);

        Task DeleteManyAsync(
            IEnumerable<string> publicIds,
            CancellationToken cancellationToken = default);
    }
}
