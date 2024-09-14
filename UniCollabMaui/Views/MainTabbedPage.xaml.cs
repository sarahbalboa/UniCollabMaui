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

            // Check if the user role is system role
            if (userRole.IsRoleAdmin != true)
            {
                // Find the RoleManagementPage tab and remove it if it exists
                var manageRolesTab = this.Children.FirstOrDefault(c => c is RoleManagementPage);
                if (manageRolesTab != null)
                {
                    this.Children.Remove(manageRolesTab);
                }
            }
            if (userRole.IsTaskViewer != true && userRole.IsTaskAdmin != true)
            {
                // Find the RoleManagementPage tab and remove it if it exists
                var taskBoardTab = this.Children.FirstOrDefault(c => c is TaskBoard);
                if (taskBoardTab != null)
                {
                    this.Children.Remove(taskBoardTab);
                }
            }
            if (userRole.IsProgressViewer != true)
            {
                // Find the RoleManagementPage tab and remove it if it exists
                var progressPageTab = this.Children.FirstOrDefault(c => c is ProgressPage);
                var userInsightsTab = this.Children.FirstOrDefault(c => c is UserInsights);
                if (progressPageTab != null)
                {
                    this.Children.Remove(progressPageTab);
                }
                if (userInsightsTab != null)
                {
                    this.Children.Remove(userInsightsTab);
                }
            }
        }
        else
        {
            // Handle session expired or invalid session
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }


}