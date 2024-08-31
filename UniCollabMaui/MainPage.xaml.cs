//using AVFoundation;
using UniCollabMaui.Views;

namespace UniCollabMaui
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            ShowLogo();
            StartBtnColourAnimation();
        }

        public async void StartBtnColourAnimation()
        {
            while (true)
            {
                await StartBtn.FadeTo(0, 1000); // Fade out over 1 second
                await Task.Delay(200);
                await StartBtn.FadeTo(1, 1000); // Fade in over 1 second
            }
        }
        
        public async void ShowLogo()
        {
            while(Logo.Opacity < 1) {
                await Task.Delay(100);
                Logo.Opacity = Logo.Opacity + 0.1;
            }
        }
        

        private async void OnStartClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserInsights());

        }
    }

}
