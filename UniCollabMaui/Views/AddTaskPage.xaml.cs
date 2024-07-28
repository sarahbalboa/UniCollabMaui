using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views
{
    public partial class AddTaskPage : ContentPage
    {
        public AddTaskPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private async void LoadUsers()
        {
            var users = await TaskBoardService.GetUsers();
            UserPicker.ItemsSource = new List<User>(users);
        }

        private async void OnSaveTaskButtonClicked(object sender, EventArgs e)
        {
            var selectedUser = (User)UserPicker.SelectedItem;
            if (selectedUser == null)
            {
                await DisplayAlert("Error", "Please select a user.", "OK");
                return;
            }

            var task = new AppTask
            {
                Title = TaskTitleEntry.Text,
                Description = TaskDescriptionEditor.Text,
                Column = TaskColumnPicker.SelectedItem.ToString(),
                AssignedToUserId = selectedUser.Id
            };

            await TaskBoardService.AddAppTask(task.Title, task.Description, task.Column, task.AssignedToUserId);
            await Navigation.PopAsync();
        }
    }
}
