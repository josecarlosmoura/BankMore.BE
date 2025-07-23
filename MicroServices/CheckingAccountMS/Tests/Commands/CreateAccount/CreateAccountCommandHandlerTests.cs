using BuildingBlocks.Exeption;
using CheckingAccountMS.Application.Commands.CreateAccount;
using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using Moq;
using System.Linq.Expressions;

namespace CheckingAccountMS.Tests.Commands.CreateAccount
{
    [TestFixture]
    public class CreateAccountCommandHandlerTests
    {
        private Mock<ICheckingAccountRepository> _repositoryMock;
        private CreateAccountCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<ICheckingAccountRepository>();
            _handler = new CreateAccountCommandHandler(_repositoryMock.Object);
        }

        [Test]
        public void Handle_ShouldThrowInvalidDocument_WhenCpfIsInvalid()
        {
            // Arrange
            var command = new CreateAccountCommand
            {
                Cpf = "123", // CPF inválido
                Name = "Test User",
                Password = "Test@123"
            };

            // Act + Assert
            var exception = Assert.ThrowsAsync<ServiceException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.That(exception.Error, Is.EqualTo(ServiceError.InvalidDocument));
        }

        [Test]
        public void Handle_ShouldThrowUserAlreadyExists_WhenCpfAlreadyExists()
        {
            // Arrange
            var command = new CreateAccountCommand
            {
                Cpf = "12345678909", // CPF válido
                Name = "Test User",
                Password = "Test@123"
            };

            var cpfOnlyDigits = new string(command.Cpf.Where(char.IsDigit).ToArray());
            _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(new CheckingAccount { Cpf = cpfOnlyDigits });

            // Act + Assert
            var exception = Assert.ThrowsAsync<ServiceException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.That(exception.Error, Is.EqualTo(ServiceError.UserAlreadyExists));
        }

        [Test]
        public async Task Handle_ShouldCreateAccountSuccessfully()
        {
            // Arrange
            var command = new CreateAccountCommand
            {
                Cpf = "39053344705", // CPF válido
                Name = "Test User",
                Password = "Test@123"
            };

            _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync((CheckingAccount)null);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<CheckingAccount>()))
                .Returns(Task.CompletedTask);

            _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
