using FluentValidation;

namespace ShoppeeEcommerce.WebAPI.Common.Validators
{
    public static class ImageValidationExtensions
    {
        private const long DefaultMaxFileSizeBytes = 2 * 1024 * 1024; // 2MB

        private static readonly string[] DefaultAllowedContentTypes =
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

        private static readonly string[] DefaultAllowedExtensions =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

        public static IRuleBuilderOptions<T, IFormFileCollection> MustBeValidImageCollection<T>(
            this IRuleBuilder<T, IFormFileCollection> ruleBuilder,
            int minFiles = 1,
            int maxFiles = 10)
        {
            return ruleBuilder
                .NotNull()
                .WithMessage("Image collection is required.")

                .Must(files => files.Count >= minFiles)
                .WithMessage($"At least {minFiles} image(s) must be uploaded.")

                .Must(files => files.Count <= maxFiles)
                .WithMessage($"No more than {maxFiles} images are allowed.");
        }

        public static IRuleBuilderOptions<T, IFormFile> MustBeValidImage<T>(
            this IRuleBuilder<T, IFormFile> ruleBuilder,
            long? maxFileSizeBytes = null,
            string[]? allowedContentTypes = null,
            string[]? allowedExtensions = null)
        {
            maxFileSizeBytes ??= DefaultMaxFileSizeBytes;
            allowedContentTypes ??= DefaultAllowedContentTypes;
            allowedExtensions ??= DefaultAllowedExtensions;

            return ruleBuilder
                .NotNull()
                .WithMessage("Image is required.")

                .Must(file => file.Length <= maxFileSizeBytes)
                .WithMessage($"Image size must not exceed {maxFileSizeBytes / 1024 / 1024}MB.")

                .Must(file => allowedContentTypes.Contains(file.ContentType))
                .WithMessage("Unsupported image content type.")

                .Must(file => allowedExtensions.Contains(
                    Path.GetExtension(file.FileName).ToLower()))
                .WithMessage("Invalid image file extension.")

                .Must(HaveValidImageSignature)
                .WithMessage("The uploaded file is not a valid image.");
        }

        private static bool HaveValidImageSignature(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            Span<byte> header = stackalloc byte[12];
            stream.ReadExactly(header);

            // JPEG
            if (header[0] == 0xFF && header[1] == 0xD8)
                return true;

            // PNG
            if (header[0] == 0x89 && header[1] == 0x50 &&
                header[2] == 0x4E && header[3] == 0x47)
                return true;

            // WEBP (RIFF....WEBP)
            if (header[0] == 0x52 && header[1] == 0x49 &&
                header[2] == 0x46 && header[3] == 0x46 &&
                header[8] == 0x57 && header[9] == 0x45 &&
                header[10] == 0x42 && header[11] == 0x50)
                return true;

            return false;
        }
    }
}
