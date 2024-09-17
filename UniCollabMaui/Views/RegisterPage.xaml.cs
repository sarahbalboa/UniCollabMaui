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

            LoadRoles();
        }

        private async void LoadRoles()
        {
            var roles = await DatabaseService.GetRoles();
            List<Role> roleList = new List<Role>(roles);
            List<Role> systemRoleList = new List<Role>();
            //remove non system default roles
            foreach (Role role in roleList) {   
                if(role.IsSystemRole == true && role.Active == true)
                {
                    systemRoleList.Add(role);
                }
            }
            RolePicker.ItemsSource = systemRoleList;

        }
        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrEmpty(NameEntry.Text) || string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text) || (Role)RolePicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please fill out all fields.", "OK");
                return;
            }

            var name = NameEntry.Text;
            var username = UsernameEntry.Text;
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;
            var role = (Role)RolePicker.SelectedItem;
            
            // Add the user to the database
            await DatabaseService.AddUser(name, true, username, email, password, role.Id);


            //add toast of registration successful

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await Toast.Make("User registered",
                      ToastDuration.Short,
                      16)
                .Show(cancellationTokenSource.Token);

            // Navigate back to the login page
            await Navigation.PopAsync();
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable the register button if all entries are filled
            RegisterButton.IsEnabled = !string.IsNullOrEmpty(NameEntry.Text) &&
                                       !string.IsNullOrEmpty(UsernameEntry.Text) &&
                                       !string.IsNullOrEmpty(EmailEntry.Text) &&
                                       !string.IsNullOrEmpty(PasswordEntry.Text) &&
                                       !((Role)RolePicker.SelectedItem == null);
        }

        private async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        }
}
