using System.Text.RegularExpressions;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views
{
    /// <summary>
    /// LogIn view to log in the system.
    /// </summary>
    public partial class LogIn : ContentPage
    {
        private const int MaxPasswordLength = 15;

        /// <summary>
        /// Log In page constructor.
        /// </summary>
        public LogIn()
        {
            InitializeComponent();
        }
        

        /// <summary>
        /// Disable the back button
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            // Return true to disable the back button functionality
            return true;
        }

        /// <summary>
        /// Log In buttin click listener that validates the user to check if the login credentials are correct and create a session for them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnLogInButtonClicked(object sender, EventArgs e)
        {
            string username = Username.Text;
            string password = Password.Text;

            // Validate input fields
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
                return;
            }

            // Attempt to log in
            var user = await DatabaseService.ValidateUser(username, password);
            
            if (user is not null)
            {
                    if (user.Active)
                    {
                        AppSession.SessionId = await DatabaseService.CreateSession(user.Id);
                        // Navigate to another view, e.g., HomePage
                        await Navigation.PushAsync(new MainTabbedPage());

                    }
                    else
                    {
                        await DisplayAlert("Error", "Your account is Inactive. Please contact a system administrator.", "OK");
                    }
                
            }
            else
            {
                await DisplayAlert("Error", "Invalid username or password.", "OK");
            }
        }

        /// <summary>
        /// OnTextChanged listener to enable the log in button if all required fields are filled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if (entry is not null)
            {
                // Ensure only alphanumeric characters are allowed
                string newText = Regex.Replace(entry.Text, @"[^a-zA-Z0-9]", "");

                // Enforce the maximum length
                if (newText.Length > MaxPasswordLength)
                {
                    newText = newText[..MaxPasswordLength];
                }

                // Update the text if it has changed
                if (entry.Text != newText)
                {
                    entry.Text = newText;
                }

                CheckIfNextButtonCanBeEnabled();
            }
           
        }

        /// <summary>
        /// OnTextChanged listener to enable the log in button if all required fields are filled.
        /// </summary>
        private void CheckIfNextButtonCanBeEnabled()
        {
            NextButton.IsEnabled = !string.IsNullOrEmpty(Username.Text) && !string.IsNullOrEmpty(Password.Text);
        }

        /// <summary>
        /// Click button listener to navigate to the RegisterPage view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            // Navigate to the Register page (implement registration page navigation)
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}
