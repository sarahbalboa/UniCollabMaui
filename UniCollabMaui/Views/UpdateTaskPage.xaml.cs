using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views;

public partial class UpdateTaskPage : ContentPage
{
    private readonly IDatabaseService _databaseService;
    private int? taskId;
    public UpdateTaskPage(IDatabaseService databaseService, int? taskId = null)
	{
		InitializeComponent();
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        this.taskId = taskId;
        if (taskId.HasValue)
        {
            LoadTask(taskId.Value);
        }

    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a session ID in AppSession
        var userId = await _databaseService.GetUserIdFromSession(AppSession.SessionId);
        if (userId.HasValue)
        {
            var userRole = await _databaseService.GetUserRole(userId.Value);
            var task = await _databaseService.GetAppTaskById(taskId.Value);

            // Check if the user role is system role
            if (userRole.IsTaskAdmin != true && (task.AssignedToUserId != userId))
            {
                UserPicker.IsEnabled = false;
                TaskTitleEntry.IsEnabled = false;
                TaskDescriptionEditor.IsEnabled = false;
                TaskColumnPicker.IsEnabled = false;
                TaskPriorityPicker.IsEnabled = false;
                SaveButton.IsVisible = false;
                DeleteButton.IsVisible = false;
            }
        }
    }

    private async Task LoadUsers()
    {
        var users = await _databaseService.GetUsers();
        UserPicker.ItemsSource = new List<User>(users);
    }

    private async void LoadTask(int id)
    {
        await LoadUsers();
        var task = await _databaseService.GetAppTaskById(id);
        if (task != null)
        {
            TaskTitleEntry.Text = task.Title;
            TaskDescriptionEditor.Text = task.Description;
            TaskColumnPicker.SelectedItem = task.Column;
            TaskPriorityPicker.SelectedItem = task.Priority;
            
            List<User> users = (List<User>)UserPicker.ItemsSource;
            
            //Initialize a variable to hold the selected user
            User selectedUser = null;

            //Iterate through each user in the list to find the one with the matching Id
            foreach (User user in users)
            {
                if (user.Id == task.AssignedToUserId)
                {
                    selectedUser = user;
                    break; // Exit the loop once the user is found
                }
            }

            //Set the SelectedItem of the UserPicker to the found user
            UserPicker.SelectedItem = selectedUser;
        }


    }

    private async void OnSaveTaskButtonClicked(object sender, EventArgs e)
    {
        //check that all required fields are entered
        if ((User)UserPicker.SelectedItem == null || string.IsNullOrEmpty(TaskTitleEntry.Text) || string.IsNullOrEmpty(TaskDescriptionEditor.Text) || TaskColumnPicker.SelectedItem == null && TaskPriorityPicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Please fill in all the task details.", "OK");
            return;
        }
        

        var selectedUser = (User)UserPicker.SelectedItem;
        var title = TaskTitleEntry.Text;
        var description = TaskDescriptionEditor.Text;
        var column = TaskColumnPicker.SelectedItem.ToString();
        var priority = TaskPriorityPicker.SelectedItem.ToString();

        if (!selectedUser.Active) {
            await DisplayAlert("Error", "Assigned to User is Inactive. Please select an active user.", "OK");
            return;
        }

        await _databaseService.UpdateAppTask(taskId.Value, title, description, column, priority, selectedUser.Id);

        await Navigation.PopAsync();

        var sessionUserId = await _databaseService.GetUserIdFromSession(AppSession.SessionId);
        var sessionUser = await _databaseService.GetUserById((int)sessionUserId);

        //logger for saved/updated Role
        Logger.Log("Changed by " + sessionUser.Username + " \nTask [#" + taskId + "] " + title + " is Updated: \n" +
            "-Description: " + description +
            "\n-Column: " + column +
            "\n-Priority: " + priority);

    }

    private async void OnDeleteTaskButtonClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Deletion Confirmation", "Are you sure you want to delete this task", "Yes", "No");
        if(answer){ 
            await _databaseService.RemoveAppTask(taskId.Value);
            await Navigation.PopAsync();
        }
    }

}