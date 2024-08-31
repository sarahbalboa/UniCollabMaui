using Microsoft.Maui.Controls;

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
        MyProgressBar.Progress = 0.5;
        var percentageProgress = (MyProgressBar.Progress * 100) + "% Done";
        ProgressLbl.Text = ProgressLbl.Text  + " "  + percentageProgress.ToString();
    }
}