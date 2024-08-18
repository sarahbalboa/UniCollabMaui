using Microsoft.Maui.Controls;
using System;
using System.Text.RegularExpressions;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views
{
    public partial class LogIn : ContentPage
    {
        private const int MaxPasswordLength = 15;

        public LogIn()
        {
            InitializeComponent();
        }

        private async void OnLogInButtonClicked(object sender, EventArgs e)
        {
            string username = Username.Text;
            string password = Password.Text;

            // Validate input fields
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
                return;
            }

            // Attempt to log in
            var user = await DatabaseService.ValidateUser(username, password);

            if (user != null)
            {
                AppSession.SessionId = await DatabaseService.CreateSession(user.Id);
                // Navigate to another view, e.g., HomePage
                await Shell.Current.GoToAsync("TaskBoard");
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
            await Navigation.PushAsync(new RegisterPage());
        }

        private async void OnEraseUsersButtonClicked(object sender, EventArgs e)
        {
            await DatabaseService.EraseAllUsersData();
        }
    }
}
