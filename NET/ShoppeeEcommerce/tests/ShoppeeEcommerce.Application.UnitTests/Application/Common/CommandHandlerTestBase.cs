using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Application.Tests.Application.Common
{
    public abstract class CommandHandlerTestBase<TEntity, TKey>
        : HandlerTestBase<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        protected Mock<IUnitOfWork> UoWMock { get; }

        protected CommandHandlerTestBase()
        {
            UoWMock = new Mock<IUnitOfWork>();
        }

        protected IUnitOfWork Uow => UoWMock.Object;
    }
}
