
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
        UpdateUserAssignTasksChart();
        UpdateUserDoneAssignTasksChart();
    }

    public SKColor GetRandomColor()
    {
        Random random = new Random();
        byte r = (byte)random.Next(0, 256);  // Red
        byte g = (byte)random.Next(0, 256);  // Green
        byte b = (byte)random.Next(0, 256);  // Blue
        return new SKColor(r, g, b);
    }
    private async void UpdateUserAssignTasksChart()
    {
        var tasks = await DatabaseService.GetAppTasks();
        List<ChartEntry> assignedChartEntries = new List<ChartEntry>();
        if (tasks.Count() != 0 ) {
            List<AppTask> allTasks = new List<AppTask>(tasks);
            
            // Dictionary to store the task count per user
            var userTaskCount = new Dictionary<string, int>();

            // Count tasks per user
            foreach (AppTask task in allTasks)
            {
                var user = await DatabaseService.GetUserById(task.AssignedToUserId);
                string userLabel = user.Name;

                if (userTaskCount.ContainsKey(userLabel))
                {
                    userTaskCount[userLabel]++;

                }
                else
                {
                    userTaskCount[userLabel] = 1;
                }
            }

            // Create chart entries based on the userTaskCount dictionary
            foreach (var userTask in userTaskCount)
            {
                assignedChartEntries.Add(
                    new ChartEntry(userTask.Value)
                    {
                        Label = userTask.Key,
                        ValueLabel = userTask.Value.ToString(),
                        Color = GetRandomColor(),
                    }
                );
            }
        }
        else
        {
            var allUsers = await DatabaseService.GetUsers();
            List<User> allUsersList = new List<User>(allUsers);
            foreach (var user in allUsersList) {
                assignedChartEntries.Add(
                    new ChartEntry(0)
                    {
                        Label = user.Name,
                        ValueLabel = "0",
                        Color = GetRandomColor(),
                    }
                    );
            }
            
        }
        

        // Assign the entries to the chart view
        userAssignedTasksView.Chart = new BarChart
        {
            ShowYAxisLines = true,
            Entries = assignedChartEntries,
            BackgroundColor = SKColor.Parse("#DEEBEE"),
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 20,
            ValueLabelOrientation = Orientation.Horizontal,
            MaxValue = 1,
        };
    }

    /**private async void UpdateUserDoneTasksChart()
    {
        var tasks = await DatabaseService.GetAppTasks();
        List<ChartEntry> doneChartEntries = new List<ChartEntry>();
        if (tasks.Count() != 0)
        {
            List<AppTask> allTasks = new List<AppTask>(tasks);


            // Dictionary to store the task count per user
            var userTaskCount = new Dictionary<string, int>();

            // Count tasks per user
            foreach (AppTask task in allTasks)
            {
                if (task.Column != "Done")
                    continue;
                var user = await DatabaseService.GetUserById(task.AssignedToUserId);
                string userLabel = user.Name;

                if (userTaskCount.ContainsKey(userLabel))
                {
                    userTaskCount[userLabel]++;
                }
            }

            if(userTaskCount.Count != 0)
            {
                // Create chart entries based on the userTaskCount dictionary
                foreach (var userTask in userTaskCount)
                {
                    doneChartEntries.Add(
                        new ChartEntry(userTask.Value)
                        {
                            Label = userTask.Key,
                            ValueLabel = userTask.Value.ToString(),
                            Color = GetRandomColor(),
                        }
                    );
                }
            }
            else
            {
                var allUsers = await DatabaseService.GetUsers();
                List<User> allUsersList = new List<User>(allUsers);
                foreach (var user in allUsersList)
                {
                    doneChartEntries.Add(
                        new ChartEntry(0)
                        {
                            Label = user.Name,
                            ValueLabel = "0",
                            Color = GetRandomColor(),
                        }
                        );
                }

            }

            
        }
        else
        {
            var allUsers = await DatabaseService.GetUsers();
            List<User> allUsersList = new List<User>(allUsers);
            foreach (var user in allUsersList)
            {
                doneChartEntries.Add(
                    new ChartEntry(0)
                    {
                        Label = user.Name,
                        ValueLabel = "0",
                        Color = GetRandomColor(),
                    }
                    );
            }

        }


        // Assign the entries to the chart view
        userDoneTasksView.Chart = new BarChart
        {
            ShowYAxisLines = true,
            BackgroundColor = SKColor.Parse("#DEEBEE"),
            Entries = doneChartEntries,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 20,
            ValueLabelOrientation = Orientation.Horizontal,
            MaxValue = 1,
        };
    }*/

    private async void UpdateUserDoneAssignTasksChart()
    {
        var tasks = await DatabaseService.GetAppTasks();
        List<ChartEntry> assignedChartEntries = new List<ChartEntry>();

        if (tasks.Count() != 0)
        {
            List<AppTask> allTasks = new List<AppTask>(tasks);

            // Dictionary to store the task count per user
            var userTaskCount = new Dictionary<string, int>();

            // Count tasks per user, filtering for tasks marked as "Done"
            foreach (AppTask task in allTasks)
            {
                if (task.Column == "Done")  // Only include tasks that are marked as "Done"
                {
                    var user = await DatabaseService.GetUserById(task.AssignedToUserId);
                    string userLabel = user.Name;

                    if (userTaskCount.ContainsKey(userLabel))
                    {
                        userTaskCount[userLabel]++;
                    }
                    else
                    {
                        userTaskCount[userLabel] = 1;
                    }
                }
            }

            // Create chart entries based on the userTaskCount dictionary
            foreach (var userTask in userTaskCount)
            {
                assignedChartEntries.Add(
                    new ChartEntry(userTask.Value)
                    {
                        Label = userTask.Key,
                        ValueLabel = userTask.Value.ToString(),
                        Color = GetRandomColor(),
                    }
                );
            }
        }
        else
        {
            var allUsers = await DatabaseService.GetUsers();
            List<User> allUsersList = new List<User>(allUsers);
            foreach (var user in allUsersList)
            {
                assignedChartEntries.Add(
                    new ChartEntry(0)
                    {
                        Label = user.Name,
                        ValueLabel = "0",
                        Color = GetRandomColor(),
                    }
                );
            }
        }

        // Assign the entries to the chart view
        userDoneTasksView.Chart = new BarChart
        {
            ShowYAxisLines = true,
            Entries = assignedChartEntries,
            BackgroundColor = SKColor.Parse("#DEEBEE"),
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 20,
            ValueLabelOrientation = Orientation.Horizontal,
            MaxValue = 1,
        };
    }

}
