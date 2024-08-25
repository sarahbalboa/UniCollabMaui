using System.Threading.Tasks;
using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views;

public partial class UpdateRolePage : ContentPage
{
    private int? roleId;
    public UpdateRolePage(int? roleId = null)
    {
		InitializeComponent();
        this.roleId = roleId;
        if (roleId.HasValue)
        {
            LoadRole(roleId.Value);
        }
        
    }

    
    private async void LoadRole(int roleId)
    {
        var role = await DatabaseService.GetRoleById(roleId);
        if (role != null)
        {
            RoleNameEntry.Text = role.RoleName;
            ActiveCheckbox.IsChecked = role.Active;
            IsSystemRoleCheckbox.IsChecked = role.IsSystemRole;
            IsRoleAdminCheckbox.IsChecked = role.IsRoleAdmin;
            IsTaskEditorCheckbox.IsChecked = role.IsTaskEditor;
            IsTaskViewerCheckbox.IsChecked = role.IsTaskViewer;
            IsProgressEditorCheckbox.IsChecked = role.IsProgressEditor;
            IsProgressViewerCheckbox.IsChecked = role.IsProgressViewer;
            
        }

        if (role.IsSystemRole == true)
        {
            IsRoleAdminCheckbox.IsEnabled = false;
            IsTaskEditorCheckbox.IsEnabled = false;
            IsTaskViewerCheckbox.IsEnabled = false;
            IsProgressEditorCheckbox.IsEnabled = false;
            IsProgressViewerCheckbox.IsEnabled = false;
            RoleNameEntry.IsEnabled = false;
        }


    }


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
        var isTaskEditor = IsTaskEditorCheckbox.IsChecked;
        var isTaskViewer = IsTaskViewerCheckbox.IsChecked;
        var isProgressEditor = IsProgressEditorCheckbox.IsChecked;
        var isProgressViewer = IsProgressViewerCheckbox.IsChecked;

        await DatabaseService.UpdateRole(roleId.Value, roleName, isActive, isRoleAdmin, isTaskEditor, isTaskViewer, isProgressEditor, isProgressViewer);

        //logger for saved/updated Role
        Logger.Log("Role [#" + roleId + "] " + roleName + " is Updated: \n" +
            "-Description: " + isActive +
            "\n-RoleAdmin: " + isRoleAdmin +
            "\n-TaskEditor: " + isTaskEditor +
            "\n-TaskViewer: " + isTaskViewer +
            "\n-ProgressEditor: " + isProgressEditor+
            "\n-ProgressViewer: " + isProgressViewer );

        await Navigation.PopAsync();
    }
     
}