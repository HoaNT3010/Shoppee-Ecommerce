using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Application.Tests.Application.Common
{
    public abstract class QueryHandlerTestBase<TEntity, TKey>
        : HandlerTestBase<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        protected IRepository<TEntity, TKey> Repo => RepoMock.Object;
    }
}
