using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Authentication.CustomerRegister
{
    internal class CustomerRegisterCommandHandler(
        UserManager<User> userManager)
        : IRequestHandler<CustomerRegisterCommand, ErrorOr<Created>>
    {
        const string CustomerRoleName = "Customer";
        public async Task<ErrorOr<Created>> Handle(CustomerRegisterCommand request, CancellationToken cancellationToken)
        {
            var uniqueUserName = await userManager.FindByNameAsync(request.UserName);
            if (uniqueUserName is not null) return Errors.User.UserNameExisted(request.UserName);
            var uniqueEmail = await userManager.FindByEmailAsync(request.Email);
            if (uniqueEmail is not null) return Errors.User.EmailExisted(request.Email);

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = false,
            };
            var createdResult = await userManager.CreateAsync(user, request.Password);
            if (!createdResult.Succeeded) return Errors.Authentication.CustomerRegisterFailed();

            // Assume the Customer role existed in database.
            // For better handling, checking and adding Customer role 
            // is needed before adding new user to the role.
            await userManager.AddToRoleAsync(user, CustomerRoleName);

            // Future: Can add send confirmation email to customer
            // to validate email.

            return Result.Created;
        }
    }
}
