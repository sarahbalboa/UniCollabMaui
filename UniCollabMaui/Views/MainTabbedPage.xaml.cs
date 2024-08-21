using Microsoft.Maui.Controls;
using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

public partial class MainTabbedPage : TabbedPage
{
	public MainTabbedPage()
	{
		InitializeComponent();
	}
    
/** Function to disable the back button
 */

    protected override bool OnBackButtonPressed()
    {
        // Return true to disable the back button functionality
        return true;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a session ID in AppSession
        var userId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
        if (userId.HasValue)
        {
            var userRole = await DatabaseService.GetUserRole(userId.Value);

            // Check if the user role is not "Admin"
            if (userRole != "Admin")
            {
                // Find the RoleManagementPage tab and remove it if it exists
                var manageRolesTab = this.Children.FirstOrDefault(c => c is RoleManagementPage);
                if (manageRolesTab != null)
                {
                    this.Children.Remove(manageRolesTab);
                }
            }
            // If user is Admin, do nothing since the tab is already added in XAML
        }
        else
        {
            // Handle session expired or invalid session
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }


}