using BuildingBlocks.Auth;
using BuildingBlocks.Exeption;
using BuildingBlocks.Security;
using CheckingAccountMS.Application.Commands.DeactivateAccount;
using CheckingAccountMS.Domain.Entities;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Tests.Commands.DeactivateAccount
{
    [TestFixture]
    public class DeactivateAccountCommandHandlerTests
    {
        private Mock<ICheckingAccountRepository> _accountRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private DeactivateAccountCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _accountRepoMock = new Mock<ICheckingAccountRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _handler = new DeactivateAccountCommandHandler(_accountRepoMock.Object, _httpContextAccessorMock.Object);
        }

        private void SetupHttpContextWithUserId(string userId)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext
            {
                User = principal
            };

            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);
        }

        [Test]
        public async Task Handle_ShouldDeactivateAccount_WhenPasswordIsCorrect()
        {
            // Arrange
            var userId = "user-123";
            var command = new DeactivateAccountCommand { Password = "ValidPassword" };

            SetupHttpContextWithUserId(userId);

            string salt, encryptedPassword;
            GeneratePassword(command.Password, out salt, out encryptedPassword);

            var account = new CheckingAccount
            {
                CheckingAccountId = userId,
                Password = encryptedPassword,
                Salt = salt,
                IsActive = true
            };

            _accountRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(account);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(Unit.Value, result);
            Assert.IsFalse(account.IsActive);
            _accountRepoMock.Verify(r => r.Update(account), Times.Once);
            _accountRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_ShouldThrowUnauthorizedAccessException_WhenTokenIsMissing()
        {
            // Arrange
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(new DefaultHttpContext());

            var command = new DeactivateAccountCommand { Password = "any" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.AreEqual("Invalid or expired token.", ex.Message);
        }

        [Test]
        public void Handle_ShouldThrowServiceException_WhenAccountNotFound()
        {
            // Arrange
            var userId = "user-456";
            SetupHttpContextWithUserId(userId);

            _accountRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync((CheckingAccount)null);

            var command = new DeactivateAccountCommand { Password = "any" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ServiceException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.AreEqual(ServiceError.InvalidAccount.Type, ex.Message);
        }

        [Test]
        public void Handle_ShouldThrowServiceException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var userId = "user-789";
            SetupHttpContextWithUserId(userId);

            string salt, encryptedPassword;
            GeneratePassword("CorrectPassword", out salt, out encryptedPassword);

            var account = new CheckingAccount
            {
                CheckingAccountId = userId,
                Password = encryptedPassword,
                Salt = salt,
                IsActive = true
            };

            _accountRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CheckingAccount, bool>>>()))
                .ReturnsAsync(account);

            var command = new DeactivateAccountCommand { Password = "WrongPassword" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ServiceException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.AreEqual(ServiceError.IncorrectPassword.Type, ex.Message);
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
