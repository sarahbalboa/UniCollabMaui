

using Microcharts;
using SkiaSharp;
using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views;

public partial class UserInsights : ContentPage
{

    public UserInsights()
    {
        InitializeComponent();

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateUserTasksChart();
        UpdateUserDoneTasksChart();
    }

    private async void UpdateUserTasksChart()
    {
        var tasks = await DatabaseService.GetAppTasks();
        var users = await DatabaseService.GetUsers();
        var userTaskCount = new Dictionary<string, int>();
        List<ChartEntry> assignedTaskChartEntries = new List<ChartEntry>();

        foreach (var user in users)
        {
            userTaskCount[user.Name] = 0;
        }
        if (tasks != null)
        {
            // Loop through each task and update the userTaskCount if the task is done and assigned to a user
            foreach (var task in tasks)
            {
                // Find the user associated with the task (assuming task has a UserId or similar)
                var matchingUser = users.FirstOrDefault(u => u.Id == task.AssignedToUserId);  // Or task.UserId == u.Id, depending on your data structure

                // If the task is marked as "Done" and a matching user is found
                if (matchingUser != null)
                {
                    // Increment the task count for that user
                    userTaskCount[matchingUser.Name]++;
                }
            }
        }


        // Create chart entries based on the userTaskCount dictionary
        foreach (var userTask in userTaskCount)
        {
            assignedTaskChartEntries.Add(
                new ChartEntry(userTask.Value)
                {
                    Label = userTask.Key,
                    ValueLabel = userTask.Value.ToString(),
                    Color = SKColor.Parse("#0091C4"),
                }
            );
        }

        //create chart
        userAssignedTasksView.Chart = new BarChart
        {
            ShowYAxisLines = true,
            BackgroundColor = SKColor.Parse("#DEEBEE"),
            Entries = assignedTaskChartEntries,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 20,
            ValueLabelOrientation = Orientation.Horizontal,
            MaxValue = 1,
        };
    }
    
    private async void UpdateUserDoneTasksChart()
    {
        var tasks = await DatabaseService.GetAppTasks();
        var users = await DatabaseService.GetUsers();
        var userTaskCount = new Dictionary<string, int>();
        List<ChartEntry> doneChartEntries = new List<ChartEntry>();

        foreach (var user in users)
        {
            userTaskCount[user.Name] = 0;
        }
        if (tasks != null)
        {
            // Loop through each task and update the userTaskCount if the task is done and assigned to a user
            foreach (var task in tasks)
            {

                // Find the user associated with the task (assuming task has a UserId or similar)
                var matchingUser = users.FirstOrDefault(u => u.Id == task.AssignedToUserId);  // Or task.UserId == u.Id, depending on your data structure


                // If the task is marked as "Done" and a matching user is found
                if (matchingUser != null && task.Column == "Done")
                {
                    // Increment the task count for that user
                    userTaskCount[matchingUser.Name]++;
                }
            }
        }
        

        // Create chart entries based on the userTaskCount dictionary
        foreach (var userTask in userTaskCount)
        {
            doneChartEntries.Add(
                new ChartEntry(userTask.Value)
                {
                    Label = userTask.Key,
                    ValueLabel = userTask.Value.ToString(),
                    Color = SKColor.Parse("#0091C4"),
                }
            );
        }

        //create chart

        userDoneTasksView.Chart = new BarChart
        {
            ShowYAxisLines = true,
            Entries = doneChartEntries,
            BackgroundColor = SKColor.Parse("#DEEBEE"),
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 20,
            ValueLabelOrientation = Orientation.Horizontal,
            MaxValue = 1,
        };
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        //show Refreshing... label to let teh customer know if action behind the button
        RefreshLbl.IsVisible = true;
        await RefreshLbl.FadeTo(1, 500);
        UpdateUserTasksChart();
        UpdateUserDoneTasksChart();
        await RefreshLbl.FadeTo(0, 500);
        RefreshLbl.IsVisible = false;
    }
}

