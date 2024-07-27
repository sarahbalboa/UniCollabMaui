using Microsoft.Maui.Controls;
namespace UniCollabMaui.Views
{
    public partial class TaskDetailsPage : ContentPage
    {
        public TaskDetailsPage()
        {
            InitializeComponent();
        }
        public string TaskName { get; set; }
        public TaskDetailsPage(string taskName)
        {
            InitializeComponent();
            TaskName = taskName;
            BindingContext = this;
        }
    }

}

