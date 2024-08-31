
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
        UpdateUserContributionChart();
    }

    public SKColor GetRandomColor()
    {
        Random random = new Random();
        byte r = (byte)random.Next(0, 256);  // Red
        byte g = (byte)random.Next(0, 256);  // Green
        byte b = (byte)random.Next(0, 256);  // Blue
        return new SKColor(r, g, b);
    }
    private async void UpdateUserContributionChart()
    {
        var tasks = await DatabaseService.GetAppTasks();
        List<AppTask> allTasks = new List<AppTask>(tasks);
        List<ChartEntry> chartEntries = new List<ChartEntry>();

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
            chartEntries.Add(
                new ChartEntry(userTask.Value)
                {
                    Label = userTask.Key,
                    ValueLabel = userTask.Value.ToString(),
                    Color = GetRandomColor(),
                }
            );
        }

        // Assign the entries to the chart view
        userContributionChartView.Chart = new BarChart
        {
            ShowYAxisLines = true,
            Entries = chartEntries,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 20,
            ValueLabelOrientation = Orientation.Horizontal,
            MaxValue = allTasks.Count(),
        };
    }
}