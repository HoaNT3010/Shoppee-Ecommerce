using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Application.Tests.Application.Common
{
    public abstract class HandlerTestBase<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        protected readonly Mock<IRepository<TEntity, TKey>> RepoMock;

        protected HandlerTestBase()
        {
            RepoMock = new Mock<IRepository<TEntity, TKey>>();
        }

        protected Mock<ILogger<THandler>> CreateLoggerMock<THandler>() => new();

        protected void VerifyErrorLog<THandler>(
            Mock<ILogger<THandler>> loggerMock,
            Times times)
        {
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((_, _) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }
    }
}
