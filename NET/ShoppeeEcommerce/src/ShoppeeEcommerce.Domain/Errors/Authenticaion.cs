using ErrorOr;

namespace ShoppeeEcommerce.Domain.Errors
{
    public static partial class Errors
    {
        public static class Authentication
        {
            public static Error InvalidCredentials() =>
                Error.Unauthorized(
                    "Authentication.InvalidCredentials",
                    $"Invalid user's credentials.");
        }
    }
}
