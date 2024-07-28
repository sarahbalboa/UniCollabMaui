using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views
{
    public partial class AddTaskPage : ContentPage
    {
        private int? taskId;

        public AddTaskPage(int? taskId = null)
        {
            InitializeComponent();
            this.taskId = taskId;
            LoadUsers();
            if (taskId.HasValue)
            {
                LoadTask(taskId.Value);
            }
        }

        private async void LoadUsers()
        {
            var users = await DatabaseService.GetUsers();
            UserPicker.ItemsSource = new List<User>(users);
        }

        private async void LoadTask(int id)
        {
            var task = await DatabaseService.GetAppTaskById(id);
            if (task != null)
            {
                TaskTitleEntry.Text = task.Title;
                TaskDescriptionEditor.Text = task.Description;
                TaskColumnPicker.SelectedItem = task.Column;
                TaskPriorityPicker.SelectedItem = task.Priority;
                UserPicker.SelectedItem = ((List<User>)UserPicker.ItemsSource).Find(u => u.Id == task.AssignedToUserId);
            }
        }

        private async void OnSaveTaskButtonClicked(object sender, EventArgs e)
        {
            var selectedUser = (User)UserPicker.SelectedItem;
            if (selectedUser == null)
            {
                await DisplayAlert("Error", "Please select a user.", "OK");
                return;
            }

            var title = TaskTitleEntry.Text;
            var description = TaskDescriptionEditor.Text;
            var column = TaskColumnPicker.SelectedItem.ToString();
            var priority = TaskPriorityPicker.SelectedItem?.ToString(); // Correctly get the priority value

            // Logging for debugging
            System.Diagnostics.Debug.WriteLine($"Updating Task: {taskId.Value}, Title: {title}, Description: {description}, Column: {column}, Priority: {priority}, UserId: {selectedUser.Id}");

            if (taskId.HasValue)
            {
                await DatabaseService.UpdateAppTask(taskId.Value, title, description, column, priority, selectedUser.Id);
            }
            else
            {
                await DatabaseService.AddAppTask(title, description, column, priority, selectedUser.Id);
            }

            await Navigation.PopAsync();
        }
    }
}
