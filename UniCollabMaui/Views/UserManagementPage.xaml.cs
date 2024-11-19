using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

public partial class UserManagementPage : ContentPage
{
	public UserManagementPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadUsers(); // Load roles when the page appears
    }
    private async Task OnUserTapped(int userId)
    {
        await Navigation.PushAsync(new UpdateUserPage(userId));
        //implement an update role page
    }
    private static Color GetUserColor(User user)
    {
        // Example logic to determine the color based on task priority
        // You can modify this logic to fit your requirements
        switch (user.Active)
        {
            case true:
                return Colors.DarkBlue;
            case false:
                return Colors.DarkGrey;
        }
    }
    private async void LoadUsers()
    {
        var users = await DatabaseService.GetUsers();

        ActiveUsersColumn.Children.Clear();
        InactiveUsersColumn.Children.Clear();

        foreach (var user in users)
        {
            //do not display unassigned user
            if (user.Id == 0) { continue; }
            var backgroundColor = GetUserColor(user);

            // Create an Image for the icon
            var editIcon = new Image
            {
                Source = "edit_simple.png", // Replace with your icon file
                WidthRequest = 20,
                HeightRequest = 20,
                VerticalOptions = LayoutOptions.Center
            };

            // Create a Label for the role name
            var userLabel = new Label
            {
                Text = $"{user.Name} ({user.Email})",
                VerticalOptions = LayoutOptions.Center
            };

            // Create a horizontal StackLayout to hold the icon and label
            var userContent = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
                Children = { editIcon, userLabel }
            };

            var userView = new Frame
            {
                Padding = 10,
                Margin = new Thickness(5, 5, 5, 20), // Add more space at the bottom,
                BackgroundColor = backgroundColor,
                BorderColor = Colors.White,
                Content = userContent
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => await OnUserTapped(user.Id);
            userView.GestureRecognizers.Add(tapGestureRecognizer);

            switch (user.Active)
            {
                case true:
                    ActiveUsersColumn.Children.Add(userView);
                    break;
                case false:
                    InactiveUsersColumn.Children.Add(userView);
                    break;
            }
        }
    }
    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        //show Refreshing... label to let teh customer know if action behind the button
        RefreshLbl.IsVisible = true;
        await RefreshLbl.FadeTo(1, 500);
        LoadUsers();
        await RefreshLbl.FadeTo(0, 500);
        RefreshLbl.IsVisible = false;
    }
}