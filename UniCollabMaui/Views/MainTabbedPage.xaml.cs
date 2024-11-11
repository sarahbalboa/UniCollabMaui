using Microsoft.Maui.Controls;
using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

/// <summary>
/// Main tabbed page that contains all the views tabs.
/// </summary>
public partial class MainTabbedPage : TabbedPage
{

    /// <summary>
    /// Conctructor
    /// </summary>
	public MainTabbedPage()
	{
		InitializeComponent();
	}
    

    /// <summary>
    /// Disable the back button
    /// </summary>
    /// <returns></returns>
    protected override bool OnBackButtonPressed()
    {
        // Return true to disable the back button functionality
        return true;
    }

    /// <summary>
    /// Change the base OnAppearing to check for the logged in user access level and determine what they are allowed to see (what child tabs they can interact with)
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a session ID in AppSession
        var userId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
        if (userId.HasValue)
        {
            var userRole = await DatabaseService.GetUserRole(userId.Value);

            // Check if the user role is system role
            if (!userRole.IsRoleAdmin)
            {
                // Find the RoleManagementPage tab and remove it if it exists
                var manageRolesTab = this.Children.FirstOrDefault(c => c is RoleManagementPage);
                if (manageRolesTab != null)
                {
                    this.Children.Remove(manageRolesTab);
                }
            }
            if (!userRole.IsTaskViewer && !userRole.IsTaskAdmin)
            {
                // Find the RoleManagementPage tab and remove it if it exists
                var taskBoardTab = this.Children.FirstOrDefault(c => c is TaskBoard);
                if (taskBoardTab != null)
                {
                    this.Children.Remove(taskBoardTab);
                }
            }
            if (!userRole.IsProgressViewer)
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