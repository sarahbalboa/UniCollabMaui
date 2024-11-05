using System.Threading.Tasks;
using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

public partial class UpdateUserPage : ContentPage
{
    private readonly IDatabaseService _databaseService;
    private readonly IPageDialogService _dialogService;

    private int? userId;
    public UpdateUserPage(IDatabaseService databaseService, IPageDialogService dialogService, int? userId = null)
    {
        InitializeComponent();
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        this.userId = userId;
        if (userId.HasValue)
        {
            LoadUser(userId.Value);
        }
        LoadRoles();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a session ID in AppSession
        var userId = await _databaseService.GetUserIdFromSession(AppSession.SessionId);
        if (userId.HasValue)
        {
            var userRole = await _databaseService.GetUserRole(userId.Value);

            if (userRole.IsRoleAdmin != true)
            {
                UserNameEntry.IsEnabled = false;
                ActiveCheckbox.IsEnabled = false;
                RolePicker.IsEnabled = false;
                SaveButton.IsEnabled = false;
            }
        }
    }

    private async void LoadRoles()
    {
        var roles = await _databaseService.GetRoles();
        List<Role> roleList = new List<Role>(roles);
        List<Role> activeRoleList = new List<Role>();

        //list only active roles
        foreach (Role role in roles) {
            if (role.Active) { 
                activeRoleList.Add(role);
            }
        }

        RolePicker.ItemsSource = activeRoleList;
    }

    private async void LoadUser(int userId)
    {
        var user = await _databaseService.GetUserById(userId);
        var userRole = await _databaseService.GetUserRole(userId);

        if (user != null)
        {
            UserNameEntry.Text = user.Name;
            ActiveCheckbox.IsChecked = user.Active;
            CurrentRoleEntry.Text = userRole.RoleName.ToString();
            RolePicker.SelectedItem = ((List<Role>)RolePicker.ItemsSource).Find(u => u.Id == user.RoleId);

        }
    }
    private async void OnSaveUserButtonClicked(object sender, EventArgs e)
    {
        //check that all required fields are entered
        if (string.IsNullOrEmpty(UserNameEntry.Text))
        {
            await _dialogService.ShowAlertAsync("Error", "Please enter a Name.", "OK");
            return;
        }
        var userNewName = UserNameEntry.Text;
        var isActive = ActiveCheckbox.IsChecked;
        var userNewRole = (Role)RolePicker.SelectedItem;

        await _databaseService.UpdateUser(userId.Value, userNewName, isActive, userNewRole.Id);

        var sessionUserId = await _databaseService.GetUserIdFromSession(AppSession.SessionId);
        var sessionUser = await _databaseService.GetUserById((int)sessionUserId);

        //logger for saved/updated Role
        Logger.Log("Changed by " + sessionUser.Username + " \nUser [#" + userId + "] " + userNewName + " is Updated: \n" +
            "-Active: " + isActive +
            "\n-User role: " + userNewRole);

        await Navigation.PopAsync();
    }


}