using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

/// <summary>
/// Accoun page of the mobile app with the puporse of allowing the user to Log Out and view their Tasks summary.
/// </summary>
public partial class AccountPage : ContentPage
{
    /// <summary>
    /// Contructor
    /// </summary>
	public AccountPage()
	{
		InitializeComponent();
	}

    /// <summary>
    /// Override the OnAppearing() so that the session of teh logged in user is used to identify it and update the data diplayed accordingly.
    /// </summary>
    protected override async void OnAppearing()
    {
        if (AppSession.SessionId != null)
        {
            var userId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
            var userRole = await DatabaseService.GetUserRole((int)userId);
            var user = await DatabaseService.GetUserById((int)userId);

            AccountName.Text = user.Name;
            EmailLbl.Text = user.Email;
            CurrentRoleLbl.Text = $"Role: {userRole.RoleName}";

           await DisplayUserTaskCount((int)userId);
        }
    }

    /// <summary>
    /// Display teh task summary
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task DisplayUserTaskCount(int? userId)
    {
        var tasks = await DatabaseService.GetAppTasks();
        List<AppTask> allTasks = new(tasks);


        int todoTasks = 0;
        int inProgressTasks = 0;
        int doneTasks = 0;

        foreach (var task in allTasks) 
        { 
        
            if(task.AssignedToUserId == userId)
            {
                switch (task.Column)
                {
                    case "ToDo":
                        todoTasks++;
                        break;
                    case "In Progress":
                        inProgressTasks++;
                        break;
                    case "Done":
                        doneTasks++;
                        break;
                }

            }
            
        }

        int totalUserTasks = todoTasks + inProgressTasks + doneTasks;

        ToDoLbl.Text = "To do tasks: " + todoTasks.ToString();
        InProgressLbl.Text = "In Progress tasks: " + inProgressTasks.ToString();
        DoneLbl.Text = "Done tasks: " + doneTasks.ToString();
        TotalTasksLbl.Text = "Total: " + totalUserTasks.ToString();
    }

    /// <summary>
    /// Click lister for Log out button. Delete the session and redirect user to Main page.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static async void OnLogoutButtonClicked(object sender, EventArgs e)
    {
        if (AppSession.SessionId != null)
        {
            // Log out the user by deleting the session
            await DatabaseService.Logout(AppSession.SessionId);
            // Clear the session ID stored in AppSession
            AppSession.SessionId = null;
        }

        // Clear the navigation stack by setting the login page as the new root
        Application.Current.MainPage = new NavigationPage(new MainPage());

    }

}