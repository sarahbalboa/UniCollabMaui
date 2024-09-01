using Microsoft.Maui.Controls;
using UniCollabMaui.Service;
using UniCollabMaui.Models;
using System.Data;
using Microsoft.Maui.Platform;
using System;
using System.Threading.Tasks;

namespace UniCollabMaui.Views;

public partial class ProgressPage : ContentPage
{
    public ProgressPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateProgressBar();
        UpdateTaskStatusCount();
        
    }

    private async void UpdateTaskStatusCount()
    {
        var tasks = await DatabaseService.GetAppTasks();
        List<AppTask> allTasks = new List<AppTask>(tasks);
        
        
        int todoTasks = 0;
        int inProgressTasks = 0;
        int doneTasks = 0;

        foreach (var task in allTasks)
        {
            switch (task.Column)
            {
                case "ToDo":
                    todoTasks++;
                    break;
                case "InProgress":
                    inProgressTasks++;
                    break;
                case "Done":
                    doneTasks++;
                    break;
            }
        }

        ToDoLbl.Text = "To do tasks: " + todoTasks.ToString();
        InProgressLbl.Text = "In Progress tasks: " + inProgressTasks.ToString();
        DoneLbl.Text = "Done tasks: " + doneTasks.ToString();
        TotalTasksLbl.Text = "Total: " + allTasks.Count.ToString();
    }
    private async void UpdateProgressBar()
    {
        var tasks = await DatabaseService.GetAppTasks();
        List<AppTask> allTasks = new List<AppTask>(tasks);
        double doneTasks = 0.0;
        double progress = 0.0;

        if(tasks.Count() != 0)
        {
            foreach (var task in tasks)
            {
                if (task.Column == "Done")
                {
                    doneTasks++;
                }
                progress = Math.Round(doneTasks / allTasks.Count, 2);
            }
        }

        MyProgressBar.Progress = progress;
        var percentageProgress = (progress * 100) + "% Done";
        ProgressLbl.Text = "Project current progress: " + percentageProgress.ToString();

    }

}