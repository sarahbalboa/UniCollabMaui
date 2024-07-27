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
            var tasks = await TaskBoardService.GetAppTask();

            ToDoColumn.Children.Clear();
            InProgressColumn.Children.Clear();
            DoneColumn.Children.Clear();

            foreach (var task in tasks)
            {
                var taskView = new Frame
                {
                    Padding = 10,
                    Margin = 5,
                    BackgroundColor = Colors.LightGray,
                    Content = new Label { Text = "#" + task.Id + "  " + task.Title }
                };

                switch (task.AssignedToUser)
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
