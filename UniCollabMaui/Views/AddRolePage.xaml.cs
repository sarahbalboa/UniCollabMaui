using UniCollabMaui.Service;
using UniCollabMaui.Models;
namespace UniCollabMaui.Views;

public partial class AddRolePage : ContentPage
{
    private int? roleId;
    public AddRolePage()
	{
		InitializeComponent();
        this.roleId = roleId;
    }

    private async void OnSaveRoleButtonClicked(object sender, EventArgs e)
    {
        //check that all required fields are entered
        if (string.IsNullOrEmpty(RoleNameEntry.Text))
        {
            await DisplayAlert("Error", "Please fill in all the Role details.", "OK");
            return;
        }
        var role = new Role();

        role.RoleName = RoleNameEntry.Text;
        role.Active = ActiveCheckbox.IsChecked;
        role.IsSystemRole = IsSystemRoleCheckbox.IsChecked;
        role.IsRoleAdmin = IsRoleAdminCheckbox.IsChecked;
        role.IsTaskEditor = IsTaskEditorCheckbox.IsChecked;
        role.IsTaskViewer = IsTaskViewerCheckbox.IsChecked;
        role.IsProgressEditor = IsProgressEditorCheckbox.IsChecked;
        role.IsProgressViewer = IsProgressViewerCheckbox.IsChecked;

        await DatabaseService.AddRole(role);

        //logger for saved/updated Role
        Logger.Log("Role [#" + roleId + "] " + role.RoleName + " is Updated: \n" +
            "-Description: " + role.Active +
            "\n-RoleAdmin: " + role.IsRoleAdmin +
            "\n-TaskEditor: " + role.IsTaskEditor +
            "\n-TaskViewer: " + role.IsTaskViewer +
            "\n-ProgressEditor: " + role.IsProgressEditor +
            "\n-ProgressViewer: " + role.IsProgressViewer);

        await Navigation.PopAsync();

    }
}