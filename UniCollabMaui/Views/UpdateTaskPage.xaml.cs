using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views;

/// <summary>
/// UpdateTaskPage to update an existing task record from teh AppTask table database
/// </summary>
public partial class UpdateTaskPage : ContentPage
{
    private int? taskId;
    /// <summary>
    /// Contructor that takes in teh taskId to load the task data
    /// </summary>
    /// <param name="taskId"></param>
    public UpdateTaskPage(int? taskId = null)
	{
		InitializeComponent();

        this.taskId = taskId;
        if (taskId.HasValue)
        {
            LoadTask(taskId.Value);
        }

    }

    /// <summary>
    /// Override the OnAppearing() so that the user access level is checked to determine if they can update the task.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a session ID in AppSession
        var userId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
        if (userId.HasValue)
        {
            var userRole = await DatabaseService.GetUserRole(userId.Value);
            var task = await DatabaseService.GetAppTaskById(taskId.Value);

            // Check the user role to determine what they can edit
            if (!userRole.IsTaskAdmin )
            {
                if (task.AssignedToUserId != userId)
                {
                    UserPicker.IsEnabled = false;
                    TaskTitleEntry.IsEnabled = false;
                    TaskDescriptionEditor.IsEnabled = false;
                    TaskColumnPicker.IsEnabled = false;
                    TaskPriorityPicker.IsEnabled = false;
                    SaveButton.IsVisible = false;
                    DeleteButton.IsVisible = false;
                }
                else
                {
                    UserPicker.IsEnabled = false;
                }
            }
        }
    }

    /// <summary>
    /// load the users into the User picker.
    /// </summary>
    /// <returns></returns>
    private async Task LoadUsers()
    {
        var users = await DatabaseService.GetUsers();
        UserPicker.ItemsSource = new List<User>(users);
    }

    /// <summary>
    /// Load the task data using its id
    /// </summary>
    /// <param name="id"> task id</param>
    private async void LoadTask(int id)
    {
        await LoadUsers();
        var task = await DatabaseService.GetAppTaskById(id);
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
                    // Exit the loop once the user is found
                    break; 
                }
            }

            //Set the SelectedItem of the UserPicker to the found user
            UserPicker.SelectedItem = selectedUser;
        }


    }

    /// <summary>
    /// Update the task record if all the mandartory details are entered.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

        await DatabaseService.UpdateAppTask(taskId.Value, title, description, column, priority, selectedUser.Id);

        await Navigation.PopAsync();

        var sessionUserId = await DatabaseService.GetUserIdFromSession(AppSession.SessionId);
        var sessionUser = await DatabaseService.GetUserById((int)sessionUserId);

        //logger for saved/updated Role
        Logger.Log("Changed by " + sessionUser.Username + " \nTask [#" + taskId + "] " + title + " is Updated: \n" +
            "-Description: " + description +
            "\n-Column: " + column +
            "\n-Priority: " + priority);

    }

    /// <summary>
    /// Delete the task record from the database after confirmation is recieved from the user.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnDeleteTaskButtonClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Deletion Confirmation", "Are you sure you want to delete this task", "Yes", "No");
        if(answer){ 
            await DatabaseService.RemoveAppTask(taskId.Value);
            await Navigation.PopAsync();
        }
    }

}