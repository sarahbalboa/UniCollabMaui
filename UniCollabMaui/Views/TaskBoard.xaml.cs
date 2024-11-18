using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views
{
    /// <summary>
    /// TaskBoard view to dislay the kanban board and allow management of tasks
    /// </summary>
    public partial class TaskBoard : ContentPage
    {
        /// <summary>
        /// Constructor thet executes the LoadTasks()
        /// </summary>
        public TaskBoard()
        {
            InitializeComponent();
            LoadTasks();
        }

        /// <summary>
        /// Reload the page when the 
        /// </summary>
        private void ReloadPage()
        {
            // Assuming you are within a ContentPage
            var currentPage = new TaskBoard(); // Create a new instance of the current page
            Navigation.InsertPageBefore(currentPage, this);
            Navigation.PopAsync();
        }

        /// <summary>
        /// Click listener for the Add task button. Redirects the user to the Add Task view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnAddTaskButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTaskPage());
        }

        /// <summary>
        /// Load the existing tasks on the database to the corresponding column of teh Kanban taskboard.
        /// </summary>
        private async void LoadTasks()
        {
            var tasks = await DatabaseService.GetAppTasks();
            var users = await DatabaseService.GetUsers();
            var userDictionary = new Dictionary<int, User>();

            foreach (var user in users)
            {
                userDictionary[user.Id] = user;
            }

            ToDoColumn.Children.Clear();
            InProgressColumn.Children.Clear();
            DoneColumn.Children.Clear();

            foreach (var task in tasks)
            {
                //if task user is not in the list of users assign it to Unknown
                var userName = userDictionary.ContainsKey(task.AssignedToUserId.Value) ? userDictionary[task.AssignedToUserId.Value].Name : "Unknown";

                // Determine the background color based on the task property
                var backgroundColor = GetTaskColor(task);

                // Create a Label for the task details
                var taskLabel = new Label
                {
                    Text = $" [#{task.Id}] {task.Title} \n(Assigned to: {userName})",
                    VerticalOptions = LayoutOptions.Center
                };

                // Create a horizontal StackLayout to hold the icon and label
                var taskContent = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 10,
                    Children = { taskLabel }
                };

                //create task as a Frame item
                var taskView = new Frame
                {
                    Padding = 10,
                    Margin = 5,
                    BackgroundColor = backgroundColor,
                    BorderColor = Colors.White,
                    Content = taskContent
                };

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += async (s, e) => await OnTaskTapped(task.Id);
                taskView.GestureRecognizers.Add(tapGestureRecognizer);

                //add the task Frame to its corresponding Column child list
                switch (task.Column)
                {
                    case "ToDo":
                        ToDoColumn.Children.Add(taskView);
                        break;
                    case "In Progress":
                        InProgressColumn.Children.Add(taskView);
                        break;
                    case "Done":
                        DoneColumn.Children.Add(taskView);
                        break;
                }
            }
        }

        /// <summary>
        /// set a task colour depending on its priority
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private Color GetTaskColor(AppTask task)
        {
            // Example logic to determine the color based on task priority
            // You can modify this logic to fit your requirements
            switch (task.Priority)
            {
                case "High":
                    return Colors.Red;
                case "Medium":
                    return Colors.Orange;
                case "Low":
                    return Colors.Green;
                default:
                    return Colors.Blue;
            }
        }

        private async Task OnTaskTapped(int taskId)
        {
            await Navigation.PushAsync(new UpdateTaskPage(taskId));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTasks();
        }
        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            //show Refreshing... label to let teh customer know if action behind the button
            RefreshLbl.IsVisible = true;              
            await RefreshLbl.FadeTo(1, 500);          
            LoadTasks();                   
            await RefreshLbl.FadeTo(0, 500);          
            RefreshLbl.IsVisible = false;             
        }
    }
}
