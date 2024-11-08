using UniCollabMaui.Models;
using UniCollabMaui.Service;
namespace UniCollabMaui.Views;

public partial class AccountPage : ContentPage
{
	public AccountPage()
	{
		InitializeComponent();
	}
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