using System.Threading.Tasks;
using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

public partial class RoleManagementPage : ContentPage
{
	public RoleManagementPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadRoles(); // Load roles when the page appears
    }

    private async Task OnRoleTapped(int roleId)
    {
        await Navigation.PushAsync(new UpdateRolePage(roleId));
        //implement an update role page
    }
    private Color GetRoleColor(Role role)
    {
        // Example logic to determine the color based on task priority
        // You can modify this logic to fit your requirements
        switch (role.Active)
        {
            case true:
                return Colors.DarkBlue;
            case false:
                return Colors.DarkGrey;
            default:
                return Colors.Blue;
        }
    }

    private async void LoadRoles()
    {
        var roles = await DatabaseService.GetRoles();

        ActiveRolesColumn.Children.Clear();
        InactiveRolesColumn.Children.Clear();

        foreach (var role in roles)
        {
            var backgroundColor = GetRoleColor(role);

            // Create an Image for the icon
            var editIcon = new Image
            {
                Source = "edit_simple.png", // Replace with your icon file
                WidthRequest = 20,
                HeightRequest = 20,
                VerticalOptions = LayoutOptions.Center
            };

            // Create a Label for the role name
            var roleLabel = new Label
            {
                Text = $"{role.RoleName}",
                VerticalOptions = LayoutOptions.Center
            };

            // Create a horizontal StackLayout to hold the icon and label
            var roleContent = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
                Children = { editIcon, roleLabel }
            };

            var roleView = new Frame
            {
                Padding = 10,
                Margin = new Thickness(5, 5, 5, 20), // Add more space at the bottom
                BackgroundColor = backgroundColor,
                Content = roleContent
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => await OnRoleTapped(role.Id);
            roleView.GestureRecognizers.Add(tapGestureRecognizer);

            switch (role.Active)
            {
                case true:
                    ActiveRolesColumn.Children.Add(roleView);
                    break;
                case false:
                    InactiveRolesColumn.Children.Add(roleView);
                    break;
            }
        }
    }

    private async void OnActiveAddRoleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddRolePage(true));
    }

    private async void OnInactiveAddRoleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddRolePage(false));
    }
}