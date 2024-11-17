using CommunityToolkit.Maui.Alerts; //toast
using CommunityToolkit.Maui.Core; // toast duration
using UniCollabMaui.Models;
using UniCollabMaui.Service;

namespace UniCollabMaui.Views
{
    /// <summary>
    /// RegisterPage view to register a new user on the database.
    /// </summary>
    public partial class RegisterPage : ContentPage
    {
        /// <summary>
        /// Contructor that loads the system roles on the Roles picker.
        /// </summary>
        public RegisterPage()
        {
            InitializeComponent();

            LoadRoles();
        }

        /// <summary>
        /// Load the system roles (active ones) on the role picker.
        /// </summary>
        private async void LoadRoles()
        {
            var roles = await DatabaseService.GetRoles();
            List<Role> roleList = new(roles);
            List<Role> systemRoleList = [];
            //remove non system default roles
            foreach (Role role in roleList) {   
                if(role.IsSystemRole && role.Active)
                {
                    systemRoleList.Add(role);
                }
            }
            RolePicker.ItemsSource = systemRoleList;

        }

        /// <summary>
        /// Register button listener that validates that the user is unique and all mandatory fields are entered. If all is correct then create the new user record and
        /// redirect to the log in page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrEmpty(NameEntry.Text) || string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text) || (Role)RolePicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please fill out all fields.", "OK");
                return;
            }

            var name = NameEntry.Text;
            var username = Int32.Parse(UsernameEntry.Text);
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;
            var role = (Role)RolePicker.SelectedItem;

            // Attempt to log in
            var isUniqueUser = await DatabaseService.ValidateUniqueUser(username);
            if (isUniqueUser)
            {

                // Add the user to the database
                await DatabaseService.AddUser(name, true, username, email, password, role.Id);
                //add toast of registration successful
                await DisplayAlert("Error", "Please fill out all fields.", "OK");


                await Toast.Make("User registered",
                          ToastDuration.Short,
                          16)
                    .Show(new CancellationTokenSource().Token);

                // Navigate back to the login page
                await Navigation.PopAsync();

            }
            else
            {
                await Toast.Make("Username must be unique, please use a different username",
                          ToastDuration.Short,
                          16)
                    .Show(new CancellationTokenSource().Token);
            }

        }

        /// <summary>
        /// Enable the register button if all entries are filled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable the register button if all entries are filled
            RegisterButton.IsEnabled = !string.IsNullOrEmpty(NameEntry.Text) &&
                                       !string.IsNullOrEmpty(UsernameEntry.Text) &&
                                       !string.IsNullOrEmpty(EmailEntry.Text) &&
                                       !string.IsNullOrEmpty(PasswordEntry.Text) &&
                                       ((Role)RolePicker.SelectedItem != null);
        }

        /// <summary>
        /// Cancel buttin click listener to redirec to the Log In page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        }
}
