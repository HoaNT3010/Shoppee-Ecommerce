using ErrorOr;

namespace ShoppeeEcommerce.Domain.Errors
{
    public static partial class Errors
    {
        public static class User
        {
            public static Error NotFoundWithEmail(string email) =>
                Error.NotFound(
                    "User.NotFoundWithEmail",
                    $"User with email '{email}' was not found.");
            public static Error LockedOut() =>
                Error.Unauthorized(
                    "User.LockedOut",
                    "The current user is locked out of the system.");
        }
    }

}
