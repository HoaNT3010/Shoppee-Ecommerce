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
            public static Error CustomerRegisterFailed() =>
                Error.Failure(
                    "Authentication.CustomerRegisterFailed",
                    "Unexpected error occurred when trying to register new customer.");
        }
    }
}
