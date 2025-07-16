using BuildingBlocks.Exeption;
using CheckingAccountMS.Application.Commands.CreateTransfer;
using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Domain.Enuns;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq.Expressions;
using System.Text.Json;

namespace CheckingAccountMS.Tests.Commands.CreateTransfer
{
    [TestFixture]
    public class CreateTransferCommandHandlerTests
    {
        private Mock<ICheckingAccountRepository> _accountRepoMock;
        private Mock<ITransactionRepository> _transactionRepoMock;
        private Mock<IIdempotencyRepository> _idempotencyRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private CreateTransferCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _accountRepoMock = new Mock<ICheckingAccountRepository>();
            _transactionRepoMock = new Mock<ITransactionRepository>();
            _idempotencyRepoMock = new Mock<IIdempotencyRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _handler = new CreateTransferCommandHandler(
                _accountRepoMock.Object,
                _transactionRepoMock.Object,
                _idempotencyRepoMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnTransactionId_WhenValid()
        {
            // Arrange
            var account = new CheckingAccount
            {
                CheckingAccountId = Guid.NewGuid().ToString(),
                AccountNumber = 1234,
                Balance = 500,
                IsActive = true
            };

            var command = new CreateTransferCommand
            {
                AccountNumber = 1234,
                Amount = 100,
                TransactionType = TransactionType.Debit,
                IdempotencyKey = Guid.NewGuid()
            };

            _idempotencyRepoMock
                .Setup(x => x.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotency)null!);

            _accountRepoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(account);

            _transactionRepoMock
                .Setup(x => x.AddAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);

            _transactionRepoMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _idempotencyRepoMock
                .Setup(x => x.AddAsync(It.IsAny<Idempotency>()))
                .Returns(Task.CompletedTask);

            _idempotencyRepoMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(account.CheckingAccountId, result);
        }

        [Test]
        public void Handle_ShouldThrowInvalidAccount_WhenAccountNotFound()
        {
            // Arrange
            var command = new CreateTransferCommand
            {
                AccountNumber = 999,
                Amount = 100,
                TransactionType = TransactionType.Debit,
                IdempotencyKey = Guid.NewGuid()
            };

            _idempotencyRepoMock
                .Setup(x => x.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotency)null!);

            _accountRepoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync((CheckingAccount)null!);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ServiceException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.AreEqual(ServiceError.InvalidAccount.Type, ex?.Message);
        }

        [Test]
        public void Handle_ShouldThrowInactiveAccount_WhenAccountInactive()
        {
            var command = new CreateTransferCommand
            {
                AccountNumber = 123,
                Amount = 100,
                TransactionType = TransactionType.Credit,
                IdempotencyKey = Guid.NewGuid()
            };

            var account = new CheckingAccount
            {
                AccountNumber = 123,
                IsActive = false
            };

            _idempotencyRepoMock.Setup(x => x.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotency)null!);

            _accountRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(account);

            var ex = Assert.ThrowsAsync<ServiceException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.AreEqual(ServiceError.InactiveAccount.Type, ex?.Message);
        }

        [Test]
        public void Handle_ShouldThrowInvalidValue_WhenAmountIsZero()
        {
            var command = new CreateTransferCommand
            {
                AccountNumber = 123,
                Amount = 0,
                TransactionType = TransactionType.Debit,
                IdempotencyKey = Guid.NewGuid()
            };

            var account = new CheckingAccount { AccountNumber = 123, IsActive = true };

            _idempotencyRepoMock.Setup(x => x.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotency)null!);

            _accountRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(account);

            var ex = Assert.ThrowsAsync<ServiceException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.AreEqual(ServiceError.InvalidValue.Type, ex?.Message);
        }

        [Test]
        public void Handle_ShouldThrowInsufficientBalance_WhenBalanceTooLow()
        {
            var command = new CreateTransferCommand
            {
                AccountNumber = 123,
                Amount = 1000,
                TransactionType = TransactionType.Debit,
                IdempotencyKey = Guid.NewGuid()
            };

            var account = new CheckingAccount { AccountNumber = 123, Balance = 100, IsActive = true };

            _idempotencyRepoMock.Setup(x => x.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotency)null!);

            _accountRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(account);

            var ex = Assert.ThrowsAsync<ServiceException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.AreEqual(ServiceError.InsufficientBalance.Type, ex?.Message);
        }

        [Test]
        public async Task Handle_ShouldReturnCachedResult_WhenIdempotent()
        {
            var command = new CreateTransferCommand
            {
                IdempotencyKey = Guid.NewGuid()
            };

            var expectedResult = JsonSerializer.Serialize(new Transaction { TransactionId = Guid.NewGuid().ToString() });

            var idempotent = new Idempotency
            {
                IdempotencyKey = command.IdempotencyKey.ToString(),
                Result = expectedResult
            };

            _idempotencyRepoMock
                .Setup(x => x.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idempotent);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
