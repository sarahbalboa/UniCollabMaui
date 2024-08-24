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

    private async Task OnRoleTapped(int taskId)
    {
        //await Navigation.PushAsync(new UpdateRole(roleId));
        //implement an update role page
    }
    private Color GetRoleColor(Role role)
    {
        // Example logic to determine the color based on task priority
        // You can modify this logic to fit your requirements
        switch (role.Active)
        {
            case 1:
                return Colors.LightBlue;
            case 0:
                return Colors.LightGray;
            default:
                return Colors.Blue;
        }
    }

    private async void OnClearRoleBoardClicked(object sender, EventArgs e)
    {
        ActiveRolesColumn.Children.Clear();
    }
    private async void LoadRoles()
    {
        var roles = await DatabaseService.GetRoles();

        ActiveRolesColumn.Children.Clear();
        InactiveRolesColumn.Children.Clear();

        foreach (var role in roles)
        {
            var backgroundColor = GetRoleColor(role);

            var roleView = new Frame
            {
                Padding = 10,
                Margin = 5,
                BackgroundColor = backgroundColor,
                Content = new Label { Text = $"{role.RoleName}" }
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => await OnRoleTapped(role.Id);
            roleView.GestureRecognizers.Add(tapGestureRecognizer);

            switch (role.Active)
            {
                case 1:
                    ActiveRolesColumn.Children.Add(roleView);
                    break;
                case 0:
                    InactiveRolesColumn.Children.Add(roleView);
                    break;
            }
        }

    }
}