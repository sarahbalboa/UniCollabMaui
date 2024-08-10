using UniCollabMaui.MSALClient;
using Microsoft.Identity.Client;
namespace UniCollabMaui.Views;


public partial class MauiLoginPage : ContentPage
{
	public MauiLoginPage()
	{
		InitializeComponent();
        IAccount cachedUserAccount = PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache().Result;

        _ = Dispatcher.DispatchAsync(async () =>
        {
            if (cachedUserAccount == null)
            {
                SignInButton.IsEnabled = true;
            }
            else
            {
                await Shell.Current.GoToAsync("claimsview");
            }
        });
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        await PublicClientSingleton.Instance.AcquireTokenSilentAsync();
        await Shell.Current.GoToAsync("claimsview");
    }
    protected override bool OnBackButtonPressed() { return true; }
}
}