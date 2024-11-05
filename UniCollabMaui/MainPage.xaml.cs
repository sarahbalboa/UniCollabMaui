﻿//using AVFoundation;
using UniCollabMaui.Views;
using UniCollabMaui.Service;


namespace UniCollabMaui
{
    public partial class MainPage : ContentPage
    {
        private readonly IDatabaseService _databaseService;
        private readonly IPageDialogService _dialogService;

        public MainPage(IDatabaseService databaseService, IPageDialogService dialogService)
        {
            InitializeComponent();
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

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
            await Navigation.PushAsync(new LogIn(_databaseService, _dialogService));

        }
    }

}
