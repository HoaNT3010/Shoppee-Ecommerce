namespace ShoppeeEcommerce.SharedViewModels.Models.Common
{
    /// <summary>
    /// This request is use for endpoints that require a Guid/uuid value
    /// as the path parameter, e.g: users/{id}, product/{id},...
    /// Using string instead of Guid as the data type for the Id is better
    /// for model binding in ASP .NET Core.
    /// </summary>
    /// <param name="Id">The Id parameter. This value will be validated by FluentValidation
    /// to ensure it can be parsed safely to Guid.</param>
    public record PathGuidIdRequest(
        string Id);
}
