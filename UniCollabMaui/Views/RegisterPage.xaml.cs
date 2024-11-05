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
        private readonly IDatabaseService _databaseService;
        private readonly IPageDialogService _dialogService;

        public RegisterPage(IDatabaseService databaseService, IPageDialogService dialogService)
        {
            InitializeComponent();
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));


            LoadRoles();
        }

        private async void LoadRoles()
        {
            var roles = await _databaseService.GetRoles();
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
                await _dialogService.ShowAlertAsync("Error", "Please fill out all fields.", "OK");
                return;
            }

            var name = NameEntry.Text;
            var username = Int32.Parse(UsernameEntry.Text);
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;
            var role = (Role)RolePicker.SelectedItem;

            // Attempt to log in
            var isUniqueUser = await _databaseService.ValidateUniqueUser(username);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            if (isUniqueUser)
            {

                // Add the user to the database
                await _databaseService.AddUser(name, true, username, email, password, role.Id);
                //add toast of registration successful


                await _dialogService.ShowToastAsync("User registered");

                // Navigate back to the login page
                await Navigation.PopAsync();

            }
            else
            {
                await _dialogService.ShowToastAsync("Username must be unique, please use a different username");
            }
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
