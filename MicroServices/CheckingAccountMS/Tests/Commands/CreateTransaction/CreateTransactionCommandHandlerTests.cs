using BuildingBlocks.Exeption;
using CheckingAccountMS.Application.Commands.CreateTransaction;
using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Domain.Enuns;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace CheckingAccountMS.Tests.Commands.CreateTransaction
{
    [TestFixture]
    public class CreateTransactionCommandHandlerTests
    {
        private Mock<ICheckingAccountRepository> _mockCheckingRepo;
        private Mock<ITransactionRepository> _mockTransactionRepo;
        private Mock<IIdempotencyRepository> _mockIdempotencyRepo;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        private CreateTransactionCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockCheckingRepo = new Mock<ICheckingAccountRepository>();
            _mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockIdempotencyRepo = new Mock<IIdempotencyRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _handler = new CreateTransactionCommandHandler(
                _mockCheckingRepo.Object,
                _mockTransactionRepo.Object,
                _mockIdempotencyRepo.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [Test]
        public async Task Should_ReturnExistingTransaction_WhenIdempotencyKeyExists()
        {
            var existing = new Idempotency
            {
                IdempotencyKey = "123",
                Result = "\"result123\""
            };

            _mockIdempotencyRepo
                .Setup(r => r.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            var command = new CreateTransactionCommand { IdempotencyKey = Guid.Parse("00000000-0000-0000-0000-000000000123") };

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.AreEqual(existing.Result, result);
        }

        [Test]
        public void Should_ThrowException_WhenAccountNotFoundByNumber()
        {
            var command = new CreateTransactionCommand
            {
                IdempotencyKey = Guid.NewGuid(),
                AccountNumber = 999,
                TransactionType = TransactionType.Credit,
                Amount = 100
            };

            _mockIdempotencyRepo.Setup(r => r.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotency)null);

            _mockCheckingRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync((CheckingAccount)null);

            var ex = Assert.ThrowsAsync<ServiceException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.AreEqual(ServiceError.InvalidAccount, ex!.Error);
        }

        [Test]
        public void Should_ThrowException_WhenAccountIsInactive()
        {
            var command = new CreateTransactionCommand
            {
                IdempotencyKey = Guid.NewGuid(),
                AccountNumber = 123,
                TransactionType = TransactionType.Credit,
                Amount = 100
            };

            _mockIdempotencyRepo.Setup(r => r.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotency)null);

            _mockCheckingRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(new CheckingAccount { IsActive = false });

            var ex = Assert.ThrowsAsync<ServiceException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.AreEqual(ServiceError.InactiveAccount, ex!.Error);
        }

        [Test]
        public void Should_ThrowException_WhenAmountIsInvalid()
        {
            var command = new CreateTransactionCommand
            {
                IdempotencyKey = Guid.NewGuid(),
                AccountNumber = 123,
                TransactionType = TransactionType.Credit,
                Amount = 0
            };

            _mockIdempotencyRepo.Setup(r => r.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotency)null);

            _mockCheckingRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(new CheckingAccount { IsActive = true });

            var ex = Assert.ThrowsAsync<ServiceException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.AreEqual(ServiceError.InvalidValue, ex!.Error);
        }

        [Test]
        public void Should_ThrowException_WhenInsufficientBalance()
        {
            var account = new CheckingAccount
            {
                CheckingAccountId = Guid.NewGuid().ToString(),
                IsActive = true,
                Balance = 50
            };

            var command = new CreateTransactionCommand
            {
                IdempotencyKey = Guid.NewGuid(),
                TransactionType = TransactionType.Debit,
                Amount = 100
            };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.CheckingAccountId)
            }));

            _mockHttpContextAccessor.Setup(a => a.HttpContext!.User).Returns(claimsPrincipal);
            _mockIdempotencyRepo.Setup(r => r.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotency)null);

            _mockCheckingRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(account);

            var ex = Assert.ThrowsAsync<ServiceException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.AreEqual(ServiceError.InsufficientBalance, ex!.Error);
        }

        [Test]
        public async Task Should_CreateTransaction_WhenAllValidationsPass()
        {
            var account = new CheckingAccount
            {
                CheckingAccountId = Guid.NewGuid().ToString(),
                AccountNumber = 123456,
                IsActive = true,
                Balance = 1000
            };

            var command = new CreateTransactionCommand
            {
                IdempotencyKey = Guid.NewGuid(),
                TransactionType = TransactionType.Debit,
                Amount = 100
            };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.CheckingAccountId)
            }));

            _mockHttpContextAccessor.Setup(a => a.HttpContext!.User).Returns(claimsPrincipal);
            _mockIdempotencyRepo.Setup(r => r.FirstOrDefaultNoTrackingAsync(It.IsAny<Expression<Func<Idempotency, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotency)null);

            _mockCheckingRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(account);

            _mockTransactionRepo.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);
            _mockTransactionRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockIdempotencyRepo.Setup(r => r.AddAsync(It.IsAny<Idempotency>())).Returns(Task.CompletedTask);
            _mockIdempotencyRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsNotNull(result);
            _mockTransactionRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        }
    }
}
