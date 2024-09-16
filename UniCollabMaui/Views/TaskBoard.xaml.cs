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
        private void ReloadPage()
        {
            // Assuming you are within a ContentPage
            var currentPage = new TaskBoard(); // Create a new instance of the current page
            Navigation.InsertPageBefore(currentPage, this);
            Navigation.PopAsync();
        }


        private async void OnAddTaskButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTaskPage());
        }

        private async void LoadTasks()
        {
            var tasks = await DatabaseService.GetAppTasks();
            var users = await DatabaseService.GetUsers();
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

                // Determine the background color based on the task property
                var backgroundColor = GetTaskColor(task);

                // Create an Image for the icon
                var taskIcon = new Image
                {
                    Source = "unicollablogo1.png", // Replace with your icon file
                    WidthRequest = 20,
                    HeightRequest = 20,
                    VerticalOptions = LayoutOptions.Center
                };

                // Create a Label for the task details
                var taskLabel = new Label
                {
                    Text = $" [#{task.Id}] {task.Title} (Assigned to: {userName})",
                    VerticalOptions = LayoutOptions.Center
                };

                // Create a horizontal StackLayout to hold the icon and label
                var taskContent = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 10,
                    Children = { taskIcon, taskLabel }
                };

                var taskView = new Frame
                {
                    Padding = 10,
                    Margin = 5,
                    BackgroundColor = backgroundColor,
                    Content = taskContent
                };

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += async (s, e) => await OnTaskTapped(task.Id);
                taskView.GestureRecognizers.Add(tapGestureRecognizer);

                switch (task.Column)
                {
                    case "ToDo":
                        ToDoColumn.Children.Add(taskView);
                        break;
                    case "In Progress":
                        InProgressColumn.Children.Add(taskView);
                        break;
                    case "Done":
                        DoneColumn.Children.Add(taskView);
                        break;
                }
            }
        }


        private Color GetTaskColor(AppTask task)
        {
            // Example logic to determine the color based on task priority
            // You can modify this logic to fit your requirements
            switch (task.Priority)
            {
                case "High":
                    return Colors.Red;
                case "Medium":
                    return Colors.Orange;
                case "Low":
                    return Colors.Green;
                default:
                    return Colors.Blue;
            }
        }

        private async Task OnTaskTapped(int taskId)
        {
            await Navigation.PushAsync(new UpdateTaskPage(taskId));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTasks();
        }
    }
}
