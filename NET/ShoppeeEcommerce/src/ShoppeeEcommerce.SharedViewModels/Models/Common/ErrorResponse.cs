using ErrorOr;

namespace ShoppeeEcommerce.SharedViewModels.Models.Common
{
    public class ErrorResponse
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ErrorType Type { get; set; }
        public int NumericType { get; set; }
        public Dictionary<string, object>? Metadata { get; }
    }
}
