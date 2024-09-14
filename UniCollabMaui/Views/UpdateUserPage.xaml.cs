using System.Threading.Tasks;
using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

public partial class UpdateUserPage : ContentPage
{
    private int? userId;
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
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a session ID in AppSession
        var userId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
        if (userId.HasValue)
        {
            var userRole = await DatabaseService.GetUserRole(userId.Value);

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
        var roles = await DatabaseService.GetRoles();
        List<Role> roleList = new List<Role>(roles);
        RolePicker.ItemsSource = roleList;
    }

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

        //logger for saved/updated Role
        Logger.Log("Role [#" + userId + "] " + userNewName + " is Updated: \n" +
            "-Active: " + isActive +
            "\n-User role: " + userNewRole);

        await Navigation.PopAsync();
    }


}