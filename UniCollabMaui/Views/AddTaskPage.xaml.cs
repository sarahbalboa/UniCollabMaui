using Microsoft.Maui.Controls;
using System;
using UniCollabMaui.Service;
using UniCollabMaui.Models;

namespace UniCollabMaui.Views
{
    public partial class AddTaskPage : ContentPage
    {
        public AddTaskPage()
        {
            InitializeComponent();
        }

        private async void OnSaveTaskButtonClicked(object sender, EventArgs e)
        {
            var task = new AppTask
            {
                Title = TaskTitleEntry.Text,
                Description = TaskDescriptionEditor.Text,
                AssignedToUser = TaskColumnPicker.SelectedItem.ToString()
            };

            await TaskBoardService.AddAppTask(0, task.Title, task.Description, task.AssignedToUser);
            await Navigation.PopAsync();
        }
    }
}
