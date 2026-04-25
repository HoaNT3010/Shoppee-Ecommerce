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
            public static Error EmailExisted(string email) =>
                Error.Conflict(
                    "User.EmailExisted",
                    $"The email '{email}' has been used by another user.");
            public static Error UserNameExisted(string userName) =>
                Error.Conflict(
                    "User.UserNameExisted",
                    $"The username '{userName}' has been used by another user.");
            public static Error NotFound() =>
                Error.NotFound(
                    "User.NotFound",
                    "User was not found.");
            public static Error NotFoundWithId(string id) =>
                Error.NotFound(
                    "User.NotFoundWithId",
                    $"User with ID '{id}' was not found.");
            public static Error GetUserInfoFailed(string id) =>
                Error.Failure(
                    "User.GetUserInfoFailed",
                    $"Unexpected error occurred when trying to get info of user with ID '{id}'.");
        }
    }

}
