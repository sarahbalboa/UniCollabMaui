using Microsoft.Maui.Controls;
using System;
using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text;
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;
            string role = RoleEntry.Text;

            // Validate input fields
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please fill out all fields.", "OK");
                return;
            }

            // Create a new user
            var user = new User
            {
                Name = name,
                Username = username,
                Password = password,
                Role = role,
            };

            // Add the user to the database
            await DatabaseService.AddUser(user);

            await DisplayAlert("Success", "Registration successful.", "OK");

            // Navigate back to the login page
            await Navigation.PopAsync();
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable the register button if all entries are filled
            RegisterButton.IsEnabled = !string.IsNullOrEmpty(NameEntry.Text) &&
                                       !string.IsNullOrEmpty(UsernameEntry.Text) &&
                                       !string.IsNullOrEmpty(PasswordEntry.Text);
        }

        private async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        }
}
