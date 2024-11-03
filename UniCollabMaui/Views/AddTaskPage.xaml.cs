using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
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
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Assuming you have a session ID in AppSession
            
        }
        private async void LoadUsers()
        {
            var users = await DatabaseService.GetUsers();
            var userId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
            List<User> userList = new List<User>(users);
            List<User> activeUsersList = new List<User>();
            foreach (var user in userList)
            {
                if (user.Active)
                {
                    activeUsersList.Add(user);
                }
            }
            UserPicker.ItemsSource = activeUsersList;

        }

        private async void OnSaveTaskButtonClicked(object sender, EventArgs e)
        {
            

            //check that all required fields are entered
            if ( string.IsNullOrEmpty(TaskTitleEntry.Text) || string.IsNullOrEmpty(TaskDescriptionEditor.Text) || TaskColumnPicker.SelectedItem == null || TaskPriorityPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please fill in all the task details.", "OK");
                return;
            }
            

            var selectedUser = (User)UserPicker.SelectedItem;
            var title = TaskTitleEntry.Text;
            var description = TaskDescriptionEditor.Text;
            var column = TaskColumnPicker.SelectedItem.ToString();
            var priority = TaskPriorityPicker.SelectedItem.ToString();


            if ((User)UserPicker.SelectedItem == null) //if user is not selected, assign it to Unassigned user
            {
                await DatabaseService.AddAppTask(title, description, column, priority, 0);
            }
            else
            {
                await DatabaseService.AddAppTask(title, description, column, priority, selectedUser.Id);
            }

            

            await Navigation.PopAsync();

            //logger for saved/updated task
            Logger.Log("Task [#" +taskId+ "] "+ title + " is Created: \n" +
                "-Description: " + description +
                "\n-Column: " + column +
                "\n-Priority: " + priority);

        }
    }
}
