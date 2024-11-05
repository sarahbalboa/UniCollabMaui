using Microsoft.Maui.Controls;
using System;
using System.Text.RegularExpressions;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views
{
    public partial class LogIn : ContentPage
    {
        private readonly IDatabaseService _databaseService;
        private readonly IPageDialogService _dialogService;

        private const int MaxPasswordLength = 15;

        public LogIn(IDatabaseService databaseService, IPageDialogService dialogService)
        {
            InitializeComponent();
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        }
        protected override bool OnBackButtonPressed()
        {
            // Return true to disable the back button functionality
            return true;
        }

        private async void OnLogInButtonClicked(object sender, EventArgs e)
        {
            string username = Username.Text;
            string password = Password.Text;

            // Validate input fields
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await _dialogService.ShowAlertAsync("Error", "Please enter both username and password.", "OK");
                return;
            }

            // Attempt to log in
            var user = await _databaseService.ValidateUser(username, password);
            
            if (user != null)
            {
                    if (user.Active)
                    {
                        AppSession.SessionId = await _databaseService.CreateSession(user.Id);
                        // Navigate to another view, e.g., HomePage
                        await Navigation.PushAsync(new MainTabbedPage(_databaseService));

                    }
                    else
                    {
                        await DisplayAlert("Error", "Your account is Inactive. Please contact a system administrator.", "OK");
                    }
                
            }
            else
            {
                await DisplayAlert("Error", "Invalid username or password.", "OK");
            }
        }

        private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            // Ensure only alphanumeric characters are allowed
            string newText = Regex.Replace(entry.Text, @"[^a-zA-Z0-9]", "");

            // Enforce the maximum length
            if (newText.Length > MaxPasswordLength)
            {
                newText = newText.Substring(0, MaxPasswordLength);
            }

            // Update the text if it has changed
            if (entry.Text != newText)
            {
                entry.Text = newText;
            }

            CheckIfNextButtonCanBeEnabled();
        }

        private void OnUsernameTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            // Ensure only alphanumeric characters are allowed
            string newText = Regex.Replace(entry.Text, @"[^a-zA-Z0-9]", "");

            // Enforce the maximum length
            if (newText.Length > MaxPasswordLength)
            {
                newText = newText.Substring(0, MaxPasswordLength);
            }

            // Update the text if it has changed
            if (entry.Text != newText)
            {
                entry.Text = newText;
            }

            CheckIfNextButtonCanBeEnabled();
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            CheckIfNextButtonCanBeEnabled();
        }

        private void CheckIfNextButtonCanBeEnabled()
        {
            NextButton.IsEnabled = !string.IsNullOrEmpty(Username.Text) && !string.IsNullOrEmpty(Password.Text);
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            // Navigate to the Register page (implement registration page navigation)
            await Navigation.PushAsync(new RegisterPage(_databaseService, _dialogService));
        }
    }
}
