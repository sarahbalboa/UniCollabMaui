
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
		string userLabel = "";
		int userAssignedTaskCount = 0;

		foreach (AppTask task in allTasks)
		{
			userLabel = (await DatabaseService.GetUserById(task.AssignedToUserId)).Name;
			foreach(AppTask task2 in allTasks)
			{
				if(userLabel == (await DatabaseService.GetUserById(task2.AssignedToUserId)).Name){
					userAssignedTaskCount++;
				}
			}
            chartEntries.Add(
                    new ChartEntry(userAssignedTaskCount)
                    {
                        Label = userLabel,
                        ValueLabel = userAssignedTaskCount.ToString(),
                        Color = GetRandomColor(),
                    }
                    );

            userAssignedTaskCount = 0;

        }

        userContributionChartView.Chart = new BarChart
        {
            Entries = chartEntries,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 20,
            ValueLabelOrientation = Orientation.Horizontal,
            MaxValue = allTasks.Count(),
        };

		chartEntries.Clear();
    }
}