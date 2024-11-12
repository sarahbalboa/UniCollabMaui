using UniCollabMaui.Service;

namespace UniCollabMaui.Views;

/// <summary>
/// UpdateRolePage that allows the user to update an existing role in the database.
/// </summary>
public partial class UpdateRolePage : ContentPage
{
    private int? roleId;

    /// <summary>
    /// Contructor that takes in the roleId to load the role data.
    /// </summary>
    /// <param name="roleId"></param>
    public UpdateRolePage(int? roleId = null)
    {
		InitializeComponent();
        this.roleId = roleId;
        if (roleId.HasValue)
        {
            LoadRole(roleId.Value);
        }
        
    }

    /// <summary>
    /// Load teh role data from the database using the roleId
    /// </summary>
    /// <param name="roleId"></param>
    private async void LoadRole(int roleId)
    {
        var role = await DatabaseService.GetRoleById(roleId);
        if (role != null)
        {
            RoleNameEntry.Text = role.RoleName;
            ActiveCheckbox.IsChecked = role.Active;
            IsSystemRoleCheckbox.IsChecked = role.IsSystemRole;
            IsRoleAdminCheckbox.IsChecked = role.IsRoleAdmin;
            IsTaskAdminCheckbox.IsChecked = role.IsTaskAdmin;
            IsTaskViewerCheckbox.IsChecked = role.IsTaskViewer;
            IsProgressViewerCheckbox.IsChecked = role.IsProgressViewer;
            
        }

        if (role.IsSystemRole)
        {
            IsRoleAdminCheckbox.IsEnabled = false;
            IsTaskAdminCheckbox.IsEnabled = false;
            IsTaskViewerCheckbox.IsEnabled = false;
            IsProgressViewerCheckbox.IsEnabled = false;
            RoleNameEntry.IsEnabled = false;

            //user should be unable to set to Inactive the Admin Role
            //so that there is aways at least one active role that can manage roles/users
            if (role.RoleName == "Administrator")
            {
                ActiveCheckbox.IsEnabled = false;
            }
        }


    }

    /// <summary>
    /// Check that all teh mandatory fields are entered and update the role record.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSaveRoleButtonClicked(object sender, EventArgs e)
    {
        //check that all required fields are entered
        if (string.IsNullOrEmpty(RoleNameEntry.Text))
        {
            await DisplayAlert("Error", "Please fill in all the Role details.", "OK");
            return;
        }
        var roleName = RoleNameEntry.Text;
        var isActive = ActiveCheckbox.IsChecked;
        var isRoleAdmin = IsRoleAdminCheckbox.IsChecked;
        var isTaskAdmin = IsTaskAdminCheckbox.IsChecked;
        var isTaskViewer = IsTaskViewerCheckbox.IsChecked;
        var isProgressViewer = IsProgressViewerCheckbox.IsChecked;

        var sessionUserId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
        var sessionUser = await DatabaseService.GetUserById((int)sessionUserId);

        if(roleId != null)
        {
            await DatabaseService.UpdateRole(roleId.Value, roleName, isActive, isRoleAdmin, isTaskAdmin, isTaskViewer, isProgressViewer);
        }

        //logger for saved/updated Role
        Logger.Log("Changed by " + sessionUser.Username + " \nRole [#" + roleId + "] " + roleName + " is Updated: \n" +
            "-Description: " + isActive +
            "\n-RoleAdmin: " + isRoleAdmin +
            "\n-TaskEditor: " + isTaskAdmin +
            "\n-TaskViewer: " + isTaskViewer +
            "\n-ProgressViewer: " + isProgressViewer + "\n");

        await Navigation.PopAsync();
    }
     
}