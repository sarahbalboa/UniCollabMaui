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
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

    }

  

    private async void LoadUser(int userId)
    {
        var user = await DatabaseService.GetUserById(userId);

        if (user != null)
        {
            UserNameEntry.Text = user.Name;
            ActiveCheckbox.IsChecked = user.Active;

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

        await DatabaseService.UpdateUser(userId.Value, userNewName, isActive);
        var sessionUserId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
        var sessionUser = await DatabaseService.GetUserById((int)sessionUserId);

        //logger for saved/updated Role
        Logger.Log("Changed by " + sessionUser.Username + " \nUser [#" + userId + "] " + userNewName + " is Updated: \n" +
            "-Active: " + isActive);

        await Navigation.PopAsync();
    }


}