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

        // Assuming you have a session ID in AppSession
        var userId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
        
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

        //logger for saved/updated Role
        Logger.Log("Role [#" + userId + "] " + userNewName + " is Updated: \n" +
            "-Active: " + isActive);

        await Navigation.PopAsync();
    }


}