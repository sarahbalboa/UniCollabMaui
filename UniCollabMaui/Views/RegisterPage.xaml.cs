using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            // Validate input fields
            if (string.IsNullOrEmpty(NameEntry.Text) || string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Please fill out all fields.", "OK");
                return;
            }

            var name = NameEntry.Text;
            var username = UsernameEntry.Text;
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;

            // Attempt to log in
            var isUniqueUser = await DatabaseService.ValidateUniqueUser(username);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            if (isUniqueUser)
            {

                // Add the user to the database
                await DatabaseService.AddUser(name, true, username, email, password);
                //add toast of registration successful

                
                await Toast.Make("User registered",
                          ToastDuration.Short,
                          16)
                    .Show(cancellationTokenSource.Token);

                // Navigate back to the login page
                await Navigation.PopAsync();

            }
            else
            {
                await Toast.Make("Username must be unique, please use a different username",
                          ToastDuration.Short,
                          16)
                    .Show(cancellationTokenSource.Token);
            }
            
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable the register button if all entries are filled
            RegisterButton.IsEnabled = !string.IsNullOrEmpty(NameEntry.Text) &&
                                       !string.IsNullOrEmpty(UsernameEntry.Text) &&
                                       !string.IsNullOrEmpty(EmailEntry.Text) &&
                                       !string.IsNullOrEmpty(PasswordEntry.Text);
        }

        private async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        }
}
