using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views
{
    /// <summary>
    /// AddTaskPage view to add new Task records on teh Task table.
    /// </summary>
    public partial class AddTaskPage : ContentPage
    {
        private int? taskId;

        /// <summary>
        /// Contructor that runs the funtion that loads the users so that they can be selected on teh User dropdown.
        /// </summary>
        /// <param name="taskId"></param>
        public AddTaskPage(int? taskId = null)
        {
            InitializeComponent();
          
            this.taskId = taskId;
            LoadUsers();
        }
        
        /// <summary>
        /// Load users and and add the to the User Picker item list, check for the logged in user access level to see if they can select other users on th User picker.
        /// </summary>
        /// <returns></returns>
        private async Task LoadUsers()
        {
            var users = await DatabaseService.GetUsers();
            var userId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
            List<User> userList = new(users);
            List<User> activeUsersList = new();
            foreach (var user in userList)
            {
                if (user.Active)
                {
                    activeUsersList.Add(user);
                }
            }
            UserPicker.ItemsSource = activeUsersList;

            //Check if the user is a task admin, othewise default the assigned to user to themself and readonly
            if (userId.HasValue)
            {
                var userRole = await DatabaseService.GetUserRole(userId.Value);

                if (!userRole.IsTaskAdmin)
                {
                    UserPicker.IsEnabled = false;
                    foreach (User user in users)
                    {
                        if (user.Id == userId)
                        {
                            UserPicker.SelectedItem = user;
                            // Exit the loop once the user is found
                            break; 
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save button listener to add the new task record and check that all mandatory fields have been entered. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnSaveTaskButtonClicked(object sender, EventArgs e)
        {
            

            //Check that all required fields are entered
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
                if(column != null && priority != null)
                {
                    await DatabaseService.AddAppTask(title, description, column, priority, 0);
                }
                
            }
            else
            {
                if (column != null && priority != null)
                {
                    await DatabaseService.AddAppTask(title, description, column, priority, selectedUser.Id);
                }
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
