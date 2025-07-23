using BuildingBlocks.Exeption;
using FluentAssertions;
using Moq;
using TransferMS.Infrastructure.Repository.Interface;
using TransferMS.Application.Commands.CreateTransfer;
using TransferMS.Application.Models;
using TransferMS.Application.Services.CheckingAccount;
using TransferMS.Application.Services.HttpClientConnect;
using TransferMS.Domain.Entities;
using TransferMS.Infrastructure.Repository.Interface;

namespace TransferMS.Tests.Commands.CreateTransfer
{
    [TestFixture]
    public class TransferCommandHandlerTests
    {
        private Mock<ITransferRepository> _transferRepoMock;
        private Mock<IIdempotencyRepository> _idempotencyRepoMock;
        private Mock<ICheckingAccountService> _checkingAccountMock;
        private Mock<ICheckingAccountHttpClient> _httpClientMock;
        private TransferCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _transferRepoMock = new Mock<ITransferRepository>();
            _idempotencyRepoMock = new Mock<IIdempotencyRepository>();
            _checkingAccountMock = new Mock<ICheckingAccountService>();
            _httpClientMock = new Mock<ICheckingAccountHttpClient>();

            _checkingAccountMock.Setup(x => x.JwtToken).Returns("fake-token");

            _handler = new TransferCommandHandler(
                _transferRepoMock.Object,
                _idempotencyRepoMock.Object,
                _checkingAccountMock.Object,
                _httpClientMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldSucceed_WhenAllStepsAreSuccessful()
        {
            // Arrange
            var command = new TransferCommand
            {
                Amount = 100,
                DestinationAccountNumber = 12345,
                IdempotencyKey = Guid.NewGuid()
            };

            var debitResult = new OperationResult
            {
                Success = true,
                AccountId = "account-from"
            };

            var creditResult = new OperationResult
            {
                Success = true,
                AccountId = "account-to"
            };

            _httpClientMock.Setup(x =>
                x.DebitAsync(command.Amount, It.IsAny<Guid>(), "fake-token"))
                .ReturnsAsync(debitResult);

            _httpClientMock.Setup(x =>
                x.CreditAsync(command.DestinationAccountNumber, command.Amount, It.IsAny<Guid>(), "fake-token"))
                .ReturnsAsync(creditResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _transferRepoMock.Verify(x => x.AddAsync(It.IsAny<Transfer>()), Times.Once);
            _transferRepoMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            _idempotencyRepoMock.Verify(x => x.AddAsync(It.IsAny<Idempotency>()), Times.Once);
            _idempotencyRepoMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_ShouldThrow_WhenAmountIsZeroOrNegative()
        {
            // Arrange
            var command = new TransferCommand
            {
                Amount = 0
            };

            // Act & Assert
            Assert.ThrowsAsync<ServiceException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void Handle_ShouldThrow_WhenDebitFails()
        {
            // Arrange
            var command = new TransferCommand
            {
                Amount = 100,
                IdempotencyKey = Guid.NewGuid()
            };

            var failedDebit = new OperationResult { Success = false };

            _httpClientMock.Setup(x =>
                x.DebitAsync(command.Amount, It.IsAny<Guid>(), "fake-token"))
                .ReturnsAsync(failedDebit);

            // Act & Assert
            Assert.ThrowsAsync<ServiceException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task Handle_ShouldAttemptRefund_WhenCreditFails()
        {
            // Arrange
            var command = new TransferCommand
            {
                Amount = 100,
                DestinationAccountNumber = 123,
                IdempotencyKey = Guid.NewGuid()
            };

            var debitResult = new OperationResult
            {
                Success = true,
                AccountId = "from-acc"
            };

            var creditResult = new OperationResult
            {
                Success = false
            };

            var refundResult = new OperationResult
            {
                Success = true,
                AccountId = "refund-acc"
            };

            _httpClientMock.Setup(x =>
                x.DebitAsync(command.Amount, It.IsAny<Guid>(), "fake-token"))
                .ReturnsAsync(debitResult);

            _httpClientMock.Setup(x =>
                x.CreditAsync(command.DestinationAccountNumber, command.Amount, It.IsAny<Guid>(), "fake-token"))
                .ReturnsAsync(creditResult);

            _httpClientMock.Setup(x =>
                x.CreditAsync(null, command.Amount, It.IsAny<Guid>(), "fake-token"))
                .ReturnsAsync(refundResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _httpClientMock.Verify(x =>
                x.CreditAsync(null, command.Amount, It.IsAny<Guid>(), "fake-token"), Times.Once);
        }
    }
}
