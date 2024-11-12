using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

/// <summary>
/// RoleManagementPage view to manage and create custom roles.
/// </summary>
public partial class RoleManagementPage : ContentPage
{
    /// <summary>
    /// Constructor
    /// </summary>
	public RoleManagementPage()
	{
		InitializeComponent();
	}
    /// <summary>
    /// Execute the funtion that all teh database roles
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadRoles(); // Load roles when the page appears
    }

    /// <summary>
    /// If a role is tabbed redirect to the UpdateRolePage sending the role id as parameter.
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    private async Task OnRoleTapped(int roleId)
    {
        await Navigation.PushAsync(new UpdateRolePage(roleId));
        //implement an update role page
    }

    /// <summary>
    /// Set the role item colour depending on its Active status
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    private static Color GetRoleColor(Role role)
    {
        // Example logic to determine the color based on task priority
        // You can modify this logic to fit your requirements
        switch (role.Active)
        {
            case true:
                return Colors.DarkBlue;
            case false:
                return Colors.DarkGrey;
        }
    }

    /// <summary>
    /// Load all the roles on teh corresponding active/inactive scroll columns
    /// </summary>
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
                BorderColor = Colors.White,
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

    /// <summary>
    /// On click lister for the add role button on the active column. Redirects the user to the AddRolePage view passing the active status as a parameter.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnActiveAddRoleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddRolePage(true));
    }

    /// <summary>
    /// On click lister for the add role button on the inactive column. Redirects the user to the AddRolePage view passing the active status as a parameter.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnInactiveAddRoleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddRolePage(false));
    }
}