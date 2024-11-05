using UniCollabMaui.Service;
using UniCollabMaui.Models;
namespace UniCollabMaui.Views;

public partial class AddRolePage : ContentPage
{
    private readonly IDatabaseService _databaseService;
    private readonly IPageDialogService _dialogService;

    private int? roleId;
    public AddRolePage(IDatabaseService databaseService, IPageDialogService dialogService, bool column)
	{
		InitializeComponent();
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        this.roleId = roleId;
        setActiveStatusPerColumn(column);
    }
    private void setActiveStatusPerColumn(bool column)
    {
       ActiveCheckbox.IsChecked = column;
    }

    private async void OnSaveRoleButtonClicked(object sender, EventArgs e)
    {
        //check that all required fields are entered
        if (string.IsNullOrEmpty(RoleNameEntry.Text))
        {
            await _dialogService.ShowAlertAsync("Error", "Please fill in all the Role details.", "OK");
            return;
        }
        var role = new Role();

        role.RoleName = RoleNameEntry.Text;
        role.Active = ActiveCheckbox.IsChecked;
        role.IsSystemRole = IsSystemRoleCheckbox.IsChecked;
        role.IsRoleAdmin = IsRoleAdminCheckbox.IsChecked;
        role.IsTaskAdmin = IsTaskAdminCheckbox.IsChecked;
        role.IsTaskViewer = IsTaskViewerCheckbox.IsChecked;
        role.IsProgressViewer = IsProgressViewerCheckbox.IsChecked;

        await _databaseService.AddRole(role);

        //logger for saved/updated Role
        Logger.Log("Role [#" + roleId + "] " + role.RoleName + " is Updated: \n" +
            "-Description: " + role.Active +
            "\n-RoleAdmin: " + role.IsRoleAdmin +
            "\n-TaskEditor: " + role.IsTaskAdmin +
            "\n-TaskViewer: " + role.IsTaskViewer +
            "\n-ProgressViewer: " + role.IsProgressViewer);

        await Navigation.PopAsync();

    }
}