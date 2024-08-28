using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views;

public partial class UpdateTaskPage : ContentPage
{
    private int? taskId;
    public UpdateTaskPage(int? taskId = null)
	{
		InitializeComponent();

        this.taskId = taskId;
        LoadUsers();
        if (taskId.HasValue)
        {
            LoadTask(taskId.Value);
        }
    }
    private async void LoadUsers()
    {
        var users = await DatabaseService.GetUsers();
        UserPicker.ItemsSource = new List<User>(users);
    }

    private async void LoadTask(int id)
    {
        var task = await DatabaseService.GetAppTaskById(id);
        if (task != null)
        {
            TaskTitleEntry.Text = task.Title;
            TaskDescriptionEditor.Text = task.Description;
            TaskColumnPicker.SelectedItem = task.Column;
            TaskPriorityPicker.SelectedItem = task.Priority;
            UserPicker.SelectedItem = ((List<User>)UserPicker.ItemsSource).Find(u => u.Id == task.AssignedToUserId);
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

        await DatabaseService.UpdateAppTask(taskId.Value, title, description, column, priority, selectedUser.Id);

        await Navigation.PopAsync();

        //logger for saved/updated task
        Logger.Log("Task [#" + taskId + "] " + title + " is Updated: \n" +
            "-Description: " + description +
            "\n-Column: " + column +
            "\n-Priority: " + priority);

    }

    private async void OnDeleteTaskButtonClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Deletion Confirmation", "Are you sure you want to delete this task", "Yes", "No");
        if(answer){ 
            await DatabaseService.RemoveAppTask(taskId.Value);
            await Navigation.PopAsync();
        }
    }

}