using Microsoft.Maui.Controls;
using UniCollabMaui.Views;

namespace UniCollabMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            MainPage = new AppShell();
            //MainPage = new NavigationPage(new TaskBoard());
        }
    }
}
