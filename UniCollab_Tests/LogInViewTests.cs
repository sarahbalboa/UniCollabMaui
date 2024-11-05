using Microsoft.Maui.Controls;
using Moq;
using System.Threading.Tasks;
using UniCollabMaui.Views;
using UniCollabMaui.Service;
using Xunit;

namespace UniCollab_Tests
{
    public class LogInViewTests
    {
        private readonly Mock<INavigation> _mockNavigation;
        private readonly Mock<IDatabaseService> _mockDatabaseService;
        private readonly Mock<IPageDialogService> _mockDialogService; // For DisplayAlert
        private readonly LogIn _logInPage;

        public LogInViewTests()
        {
            // Initialize mocks
            _mockNavigation = new Mock<INavigation>();
            _mockDatabaseService = new Mock<IDatabaseService>();
            _mockDialogService = new Mock<IPageDialogService>();

            // Inject dependencies into LogIn page
            _logInPage = new LogIn
            {
                Navigation = _mockNavigation.Object,
                DatabaseService = _mockDatabaseService.Object,
                DialogService = _mockDialogService.Object,
                Username = new Entry(),
                Password = new Entry(),
                NextButton = new Button()
            };
        }

        [Fact]
        public async Task OnLogInButtonClicked_EmptyUsernameOrPassword_ShowsErrorAlert()
        {
            // Arrange
            _logInPage.Username.Text = "";  // Empty username
            _logInPage.Password.Text = "";  // Empty password

            // Act
            await _logInPage.OnLogInButtonClicked(null, null);

            // Assert
            _mockDialogService.Verify(dialog => dialog.DisplayAlert("Error", "Please enter both username and password.", "OK"));
        }

        [Fact]
        public async Task OnLogInButtonClicked_InvalidCredentials_ShowsErrorAlert()
        {
            // Arrange
            _logInPage.Username.Text = "user";
            _logInPage.Password.Text = "password";
            _mockDatabaseService.Setup(service => service.ValidateUser("user", "password")).ReturnsAsync((User)null);

            // Act
            await _logInPage.OnLogInButtonClicked(null, null);

            // Assert
            _mockDialogService.Verify(dialog => dialog.DisplayAlert("Error", "Invalid username or password.", "OK"));
        }

        [Fact]
        public async Task OnLogInButtonClicked_ValidCredentialsInactiveUser_ShowsInactiveAlert()
        {
            // Arrange
            var inactiveUser = new User { Active = false, Id = 1 };
            _logInPage.Username.Text = "user";
            _logInPage.Password.Text = "password";
            _mockDatabaseService.Setup(service => service.ValidateUser("user", "password")).ReturnsAsync(inactiveUser);

            // Act
            await _logInPage.OnLogInButtonClicked(null, null);

            // Assert
            _mockDialogService.Verify(dialog => dialog.DisplayAlert("Error", "Your account is Inactive. Please contact a system administrator.", "OK"));
        }

        [Fact]
        public async Task OnLogInButtonClicked_ValidActiveUser_NavigatesToHomePage()
        {
            // Arrange
            var activeUser = new User { Active = true, Id = 1 };
            _logInPage.Username.Text = "user";
            _logInPage.Password.Text = "password";
            _mockDatabaseService.Setup(service => service.ValidateUser("user", "password")).ReturnsAsync(activeUser);
            _mockDatabaseService.Setup(service => service.CreateSession(activeUser.Id)).ReturnsAsync("session-id");

            // Act
            await _logInPage.OnLogInButtonClicked(null, null);

            // Assert
            _mockNavigation.Verify(nav => nav.PushAsync(It.IsAny<MainTabbedPage>()));
        }

        [Fact]
        public void OnPasswordTextChanged_NonAlphanumericCharacters_RemovesSpecialCharacters()
        {
            // Arrange
            var entry = new Entry { Text = "pass$word123!" };

            // Act
            _logInPage.OnPasswordTextChanged(entry, null);

            // Assert
            Assert.Equal("password123", entry.Text);
        }

        [Fact]
        public void OnUsernameTextChanged_EnforcesMaxPasswordLength()
        {
            // Arrange
            var entry = new Entry { Text = new string('a', 20) };  // Longer than MaxPasswordLength

            // Act
            _logInPage.OnUsernameTextChanged(entry, null);

            // Assert
            Assert.Equal(new string('a', 15), entry.Text);
        }

        [Fact]
        public void CheckIfNextButtonCanBeEnabled_ValidInputFields_EnablesNextButton()
        {
            // Arrange
            _logInPage.Username.Text = "validUser";
            _logInPage.Password.Text = "validPassword";

            // Act
            _logInPage.CheckIfNextButtonCanBeEnabled();

            // Assert
            Assert.True(_logInPage.NextButton.IsEnabled);
        }

        [Fact]
        public async Task OnRegisterButtonClicked_NavigatesToRegisterPage()
        {
            // Act
            await _logInPage.OnRegisterButtonClicked(null, null);

            // Assert
            _mockNavigation.Verify(nav => nav.PushAsync(It.IsAny<RegisterPage>()));
        }
    }
}