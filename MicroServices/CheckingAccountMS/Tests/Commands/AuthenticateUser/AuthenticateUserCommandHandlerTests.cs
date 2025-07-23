using BuildingBlocks.Auth;
using BuildingBlocks.Auth.Interfaces;
using BuildingBlocks.Exeption;
using BuildingBlocks.Security;
using CheckingAccountMS.Application.Commands.AuthenticateUser;
using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using Moq;
using System.Linq.Expressions;

namespace CheckingAccountMS.Tests.Commands.AuthenticateUser
{
    [TestFixture]
    public class AuthenticateUserCommandHandlerTests
    {
        private Mock<ICheckingAccountRepository> _repositoryMock = null!;
        private Mock<IJwtTokenGenerator> _tokenGeneratorMock = null!;
        private AuthenticateUserCommandHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<ICheckingAccountRepository>();
            _tokenGeneratorMock = new Mock<IJwtTokenGenerator>();

            _handler = new AuthenticateUserCommandHandler(
                _repositoryMock.Object,
                _tokenGeneratorMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Cpf = "123.456.789-00",
                Password = "Senha123",
                AccountNumber = 00001
            };

            string salt, encryptedPassword;
            GeneratePassword(command.Password, out salt, out encryptedPassword);

            var account = new CheckingAccount
            {
                Cpf = "12345678900",
                Password = encryptedPassword,
                Salt = salt,
                AccountNumber = 00001
            };

            _repositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(account);

            _tokenGeneratorMock
                .Setup(t => t.GenerateToken(It.IsAny<CheckingAccount>()))
                .Returns("fake-jwt-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("fake-jwt-token", result.Token);
        }

        [Test]
        public void Handle_ShouldThrowUnauthorized_WhenAccountNotFound()
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Cpf = "000.000.000-00",
                Password = "senha",
                AccountNumber = 00001
            };

            _repositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync((CheckingAccount?)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<ServiceException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.That(exception!.Message, Is.EqualTo(ServiceError.Unauthorized.Type));
        }

        [Test]
        public void Handle_ShouldThrowUnauthorized_WhenPasswordIsInvalid()
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Cpf = "123.456.789-00",
                Password = "senhaerrada",
                AccountNumber = 00001
            };

            string salt, encryptedPassword;
            GeneratePassword(command.Password, out salt, out encryptedPassword);

            var account = new CheckingAccount
            {
                Cpf = "12345678900",
                Password = "ghfI2cntFxgKkiRUOBjxCfc+u611jPxuMS334tAkMqsnY1aJo9IlgUIRqnsA5YHV",
                Salt = salt,
                AccountNumber = 00001
            };

            _repositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(account);

            // Act & Assert
            var exception = Assert.ThrowsAsync<ServiceException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.That(exception!.Message, Is.EqualTo(ServiceError.Unauthorized.Type));
        }

        private static void GeneratePassword(string password, out string salt, out string encryptedPassword)
        {
            // Gera senha criptografada
            salt = PasswordHasher.GenerateSalt();
            var hash = PasswordHasher.HashPassword(password, salt);
            encryptedPassword = EncryptionService.Encrypt(password + salt);
        }
    }
}
