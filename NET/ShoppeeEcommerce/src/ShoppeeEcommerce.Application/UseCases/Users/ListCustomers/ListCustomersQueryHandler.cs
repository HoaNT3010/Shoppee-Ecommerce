using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppeeEcommerce.Domain.Constants;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Users.ListCustomers;

namespace ShoppeeEcommerce.Application.UseCases.Users.ListCustomers
{
    internal class ListCustomersQueryHandler(
        UserManager<User> userManager)
        : IRequestHandler<ListCustomersQuery, ErrorOr<PagedList<ListCustomersResponse>>>
    {
        public async Task<ErrorOr<PagedList<ListCustomersResponse>>> Handle(
            ListCustomersQuery request,
            CancellationToken cancellationToken)
        {
            var usersQuery = userManager.Users
                .Where(u => u.UserRoles.Any(r => r.Role.Name == ApplicationRole.Customer));
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                //var search = request.SearchTerm.ToLower();

                //usersQuery = usersQuery.Where(u =>
                //    u.UserName!.ToLower().Contains(search) ||
                //    u.Email!.ToLower().Contains(search) ||
                //    u.FirstName!.ToLower().Contains(search) ||
                //    u.LastName!.ToLower().Contains(search));

                var pattern = $"%{request.SearchTerm}%";

                usersQuery = usersQuery.Where(u =>
                    (u.UserName != null && EF.Functions.Like(u.UserName, pattern)) ||
                    (u.Email != null && EF.Functions.Like(u.Email, pattern)) ||
                    (u.FirstName != null && EF.Functions.Like(u.FirstName, pattern)) ||
                    (u.LastName != null && EF.Functions.Like(u.LastName, pattern)));
            }
            var totalCount = await usersQuery.CountAsync(cancellationToken);
            var users = await usersQuery
                .OrderBy(u => u.UserName)
                .Skip((request.PageIndex!.Value - 1) * request.PageSize!.Value)
                .Take(request.PageSize.Value)
                .Select(u => new ListCustomersResponse(
                    u.Id,
                    u.UserName!,
                    u.Email!,
                    u.FirstName,
                    u.LastName))
                .ToListAsync(cancellationToken);
            return new PagedList<ListCustomersResponse>(users, totalCount, request.PageIndex.Value, request.PageSize.Value);
        }
    }
}
