using System.Threading.Tasks;
using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

/// <summary>
/// UpdateUserPage that allows to update the user name and role as well as viewing their current role.
/// </summary>
public partial class UpdateUserPage : ContentPage
{
    private int? userId;

    /// <summary>
    /// Constructor that takes in the user id to load the user record data.
    /// </summary>
    /// <param name="userId"></param>
    public UpdateUserPage(int? userId = null)
    {
        InitializeComponent();
        this.userId = userId;
        if (userId.HasValue)
        {
            LoadUser(userId.Value);
        }
        LoadRoles();
    }

    /// <summary>
    /// Override the OnAppearing() to check the logged in user access level and determine if they can change the user record.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a session ID in AppSession
        var loggedInUserId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
        if (loggedInUserId.HasValue)
        {
            var userRole = await DatabaseService.GetUserRole(loggedInUserId.Value);

            if (userRole.IsRoleAdmin != true)
            {
                UserNameEntry.IsEnabled = false;
                ActiveCheckbox.IsEnabled = false;
                RolePicker.IsEnabled = false;
                SaveButton.IsEnabled = false;
            }
        }
    }

    /// <summary>
    /// Load the active records from teh database so that they are available on the Role picker.
    /// </summary>
    private async void LoadRoles()
    {
        var roles = await DatabaseService.GetRoles();
        List<Role> activeRoleList = [];

        //list only active roles
        foreach (Role role in roles) {
            if (role.Active) { 
                activeRoleList.Add(role);
            }
        }

        RolePicker.ItemsSource = activeRoleList;
    }

    /// <summary>
    /// Load the user record data
    /// </summary>
    /// <param name="userId"></param>
    private async void LoadUser(int userId)
    {
        var user = await DatabaseService.GetUserById(userId);
        var userRole = await DatabaseService.GetUserRole(userId);

        if (user != null)
        {
            UserNameEntry.Text = user.Name;
            ActiveCheckbox.IsChecked = user.Active;
            CurrentRoleEntry.Text = userRole.RoleName.ToString();
            RolePicker.SelectedItem = ((List<Role>)RolePicker.ItemsSource).Find(u => u.Id == user.RoleId);

        }
    }

    /// <summary>
    /// Update the user record if all mandatory fields are entered.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSaveUserButtonClicked(object sender, EventArgs e)
    {
        //check that all required fields are entered
        if (string.IsNullOrEmpty(UserNameEntry.Text))
        {
            await DisplayAlert("Error", "Please enter a Name.", "OK");
            return;
        }
        var userNewName = UserNameEntry.Text;
        var isActive = ActiveCheckbox.IsChecked;
        var userNewRole = (Role)RolePicker.SelectedItem;

        await DatabaseService.UpdateUser(userId.Value, userNewName, isActive, userNewRole.Id);

        var sessionUserId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
        var sessionUser = await DatabaseService.GetUserById((int)sessionUserId);

        //logger for saved/updated Role
        Logger.Log("Changed by " + sessionUser.Username + " \nUser [#" + userId + "] " + userNewName + " is Updated: \n" +
            "-Active: " + isActive +
            "\n-User role: " + userNewRole);

        await Navigation.PopAsync();
    }


}