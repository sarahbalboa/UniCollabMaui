using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

public partial class UserManagementPage : ContentPage
{
    private readonly IDatabaseService _databaseService;
    private readonly IPageDialogService _dialogService;


    public UserManagementPage(IDatabaseService databaseService, IPageDialogService dialogService)
    {
		InitializeComponent();
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

    }
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
        await Navigation.PushAsync(new UpdateUserPage(_databaseService, _dialogService, userId));
        //implement an update role page
    }
    private Color GetUserColor(User user)
    {
        // Example logic to determine the color based on task priority
        // You can modify this logic to fit your requirements
        switch (user.Active)
        {
            case true:
                return Colors.DarkBlue;
            case false:
                return Colors.DarkGrey;
            default:
                return Colors.Blue;
        }
    }
    private async void LoadUsers()
    {
        var users = await _databaseService.GetUsers();

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
}