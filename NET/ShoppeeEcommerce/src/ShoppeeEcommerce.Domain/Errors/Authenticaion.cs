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
            public static Error InvalidRefreshToken() =>
                Error.Validation(
                    "Authentication.InvalidRefreshToken",
                    "Invalid JWT refresh token.");
            public static Error RefreshTokenMissingClaims() =>
                Error.Validation(
                    "Authentication.RefreshTokenMissingClaims",
                    "JWT refresh token does not have required claim(s).");
            public static Error RevokeRefreshTokenFailed() =>
                Error.Failure(
                    "Authentication.RevokeRefreshTokenFailed",
                    "Unexpected error occurred when trying to revoke user's refresh token.");
        }
    }
}
