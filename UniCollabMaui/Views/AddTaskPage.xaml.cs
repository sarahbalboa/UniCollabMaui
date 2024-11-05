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
        private readonly IDatabaseService _databaseService;
        private readonly IPageDialogService _dialogService;

        private int? taskId;

        public AddTaskPage(IDatabaseService databaseService, IPageDialogService dialogService,  int? taskId = null)
        {
            InitializeComponent();
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

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
            var users = await _databaseService.GetUsers();
            var userId = await _databaseService.GetUserIdFromSession(AppSession.SessionId);
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

            //check if the user is a task admin, othewise default the assigned to user to themself and readonly
            if (userId.HasValue)
            {
                var userRole = await _databaseService.GetUserRole(userId.Value);

                if (userRole.IsTaskAdmin != true)
                {
                    UserPicker.IsEnabled = false;
                    foreach (User user in users)
                    {
                        if (user.Id == userId)
                        {
                            UserPicker.SelectedItem = user;
                            break; // Exit the loop once the user is found
                        }
                    }
                }
            }
        }

        private async void OnSaveTaskButtonClicked(object sender, EventArgs e)
        {
            

            //check that all required fields are entered
            if ( string.IsNullOrEmpty(TaskTitleEntry.Text) || string.IsNullOrEmpty(TaskDescriptionEditor.Text) || TaskColumnPicker.SelectedItem == null || TaskPriorityPicker.SelectedItem == null)
            {
                await _dialogService.ShowAlertAsync("Error", "Please fill in all the task details.", "OK");
                return;
            }
            

            var selectedUser = (User)UserPicker.SelectedItem;
            var title = TaskTitleEntry.Text;
            var description = TaskDescriptionEditor.Text;
            var column = TaskColumnPicker.SelectedItem.ToString();
            var priority = TaskPriorityPicker.SelectedItem.ToString();


            if ((User)UserPicker.SelectedItem == null) //if user is not selected, assign it to Unassigned user
            {
                await _databaseService.AddAppTask(title, description, column, priority, 0);
            }
            else
            {
                await _databaseService.AddAppTask(title, description, column, priority, selectedUser.Id);
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
