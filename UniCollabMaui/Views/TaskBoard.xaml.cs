using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views
{
    public partial class TaskBoard : ContentPage
    {
        public TaskBoard()
        {
            InitializeComponent();
            LoadTasks();
        }

        private async void OnAddTaskButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTaskPage());
        }

        private async void LoadTasks()
        {
            var tasks = await TaskBoardService.GetAppTasks();
            var users = await TaskBoardService.GetUsers();
            var userDictionary = new Dictionary<int, User>();

            foreach (var user in users)
            {
                userDictionary[user.Id] = user;
            }

            ToDoColumn.Children.Clear();
            InProgressColumn.Children.Clear();
            DoneColumn.Children.Clear();

            foreach (var task in tasks)
            {
                var userName = userDictionary.ContainsKey(task.AssignedToUserId) ? userDictionary[task.AssignedToUserId].Name : "Unknown";
                var taskView = new Frame
                {
                    Padding = 10,
                    Margin = 5,
                    BackgroundColor = Colors.Blue,
                    Content = new Label { Text = $"{task.Title} (Assigned to: {userName})" },
                    
                };

                switch (task.Column)
                {
                    case "ToDo":
                        ToDoColumn.Children.Add(taskView);
                        break;
                    case "InProgress":
                        InProgressColumn.Children.Add(taskView);
                        break;
                    case "Done":
                        DoneColumn.Children.Add(taskView);
                        break;
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTasks();
        }
    }
}
