using UniCollabMaui.Service;
using UniCollabMaui.Models;

namespace UniCollabMaui.Views;

/// <summary>
/// ProgressPage that displays the overall project progress.
/// </summary>
public partial class ProgressPage : ContentPage
{

    /// <summary>
    /// Cobstructor
    /// </summary>
    public ProgressPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Run the required funtions to udpate the progress visualisation.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateProgressBar();
        UpdateTaskStatusCount();
        
    }

    /// <summary>
    /// Update the Task summary
    /// </summary>
    private async void UpdateTaskStatusCount()
    {
        var tasks = await DatabaseService.GetAppTasks();
        List<AppTask> allTasks = new(tasks);
        
        
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
                case "In Progress":
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

    /// <summary>
    /// Update the progress bar
    /// </summary>
    private async void UpdateProgressBar()
    {
        var tasks = await DatabaseService.GetAppTasks();
        List<AppTask> allTasks = new(tasks);
        double doneTasks = 0.0;
        double progress = 0.0;

        if(tasks.Any())
        {
            foreach (var task in tasks)
            {
                if (task.Column == "Done")
                {
                    doneTasks++;
                }
                
            }

            progress = Math.Round(doneTasks / allTasks.Count, 2);
            MyProgressBar.Progress = progress;
            var percentageProgress = (Math.Round((progress * 100), 2) + "% Done");
            ProgressLbl.Text = "Project current progress: " + percentageProgress.ToString();
        }
        else
        {
            MyProgressBar.Progress = 0;
            ProgressLbl.Text = "Project current progress: 0%";
        }

    }

    /// <summary>
    /// Click listener for the Refresh button to refresh the Task summary and the progress bar.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        //show Refreshing... label to let teh customer know if action behind the button
        RefreshLbl.IsVisible = true;
        await RefreshLbl.FadeTo(1, 500);

        UpdateProgressBar();
        UpdateTaskStatusCount();
        await RefreshLbl.FadeTo(0, 500);
        RefreshLbl.IsVisible = false;
    }

}