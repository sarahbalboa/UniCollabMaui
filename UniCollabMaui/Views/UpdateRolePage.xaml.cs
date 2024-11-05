using Supabase.Gotrue;
using System.Threading.Tasks;
using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views;

public partial class UpdateRolePage : ContentPage
{
    private readonly IDatabaseService _databaseService;
    private readonly IPageDialogService _dialogService;

    private int? roleId;
    public UpdateRolePage(IDatabaseService databaseService, IPageDialogService dialogService, int? roleId = null)
    {
		InitializeComponent();
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));


        this.roleId = roleId;
        if (roleId.HasValue)
        {
            LoadRole(roleId.Value);
        }
        
    }

    
    private async void LoadRole(int roleId)
    {
        var role = await _databaseService.GetRoleById(roleId);
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

        if (role.IsSystemRole == true)
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


    private async void OnSaveRoleButtonClicked(object sender, EventArgs e)
    {
        //check that all required fields are entered
        if (string.IsNullOrEmpty(RoleNameEntry.Text))
        {
            await _dialogService.ShowAlertAsync("Error", "Please fill in all the Role details.", "OK");
            return;
        }
        var roleName = RoleNameEntry.Text;
        var isActive = ActiveCheckbox.IsChecked;
        var isRoleAdmin = IsRoleAdminCheckbox.IsChecked;
        var isTaskAdmin = IsTaskAdminCheckbox.IsChecked;
        var isTaskViewer = IsTaskViewerCheckbox.IsChecked;
        var isProgressViewer = IsProgressViewerCheckbox.IsChecked;

        var sessionUserId = await _databaseService.GetUserIdFromSession(AppSession.SessionId);
        var sessionUser = await _databaseService.GetUserById((int)sessionUserId);

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