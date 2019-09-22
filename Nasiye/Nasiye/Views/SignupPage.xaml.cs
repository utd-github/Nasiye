using Nasiye.Models;
using Nasiye.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nasiye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPage
    {
        public readonly IFirebaseDatabaseService _firebaseDatabase;
        public readonly IFirebaseAuthService _firebaseAuth;

        public SignupPage()
        {
            InitializeComponent();


            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();
        }

        private async void Signup_Clicked(object sender, EventArgs e)
        {
            container.IsVisible = false;
            loader.IsVisible = true;
            NetworkAccess current = Connectivity.NetworkAccess;

            // Get Email and stuff
            var name = efname.Text;
            var email = eemail.Text;

            // Show loading
            if (CheckAllFieldsAsync(name,email))
            {
                if (await ValidateFormAsync(email))
                {
                    // Check for >> Connectivity
                    if (current == NetworkAccess.Internet)
                    {
                        SignupUser(name,email);
                    }
                    else
                    {
                        container.IsVisible = true;
                        loader.IsVisible = false;
                        // No COnnectivity Toast
                        await DisplayAlert("Connections",
                            "Sorry did not go through! Please check your network connection and try again",
                            "OK");
                    }
                }
                else
                {
                    container.IsVisible = true;
                    loader.IsVisible = false;
                    // Error Validating
                    await DisplayAlert("Error", "Enter a valid email address and password", "OK");
                }
            }
            else
            {
                container.IsVisible = true;
                loader.IsVisible = false;
                await DisplayAlert("Error", "Please fill all fields first", "OK");
            }
        }

        private async void SignupUser(string name, string email)
        {
            UserModel user = new UserModel();
            try
            {
                user.Key = "00";

                user.Name = name;
                user.Email = email;

                user.Phone = "No Assigned";

                user.Jdate = getTodayDate();

                user.Image = new Uri("https://firebasestorage.googleapis.com/v0/b/nasiyebooking.appspot.com/o/image.png?alt=media&token=8a11ada6-4ee9-43fe-a5e3-f23b9c1f3e71");

                user.Rating = 0;

                user.Status = "Offline";

                string phone = await _firebaseAuth.GetUserPhone();

                user.Phone = phone;

                user.TripKey = "00";

                string res = await _firebaseAuth.GetCurrentUser();

                if (res != null)
                {
                    _firebaseDatabase.CreateProfile("users", user);
                    App.Current.MainPage =  new MainContainerPage();

                }

            }
            catch (Exception ex)
            {
                container.IsVisible = true;
                loader.IsVisible = false;
                await DisplayAlert("Signup", "Something went wrong, " + ex.Message, "OK");
            }
        }

        private bool CheckAllFieldsAsync(string name, string emailtxt)
        {
            if (String.IsNullOrWhiteSpace(emailtxt) && String.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        private async Task<bool> ValidateFormAsync(string email)
        {

            if (String.IsNullOrWhiteSpace(email)  )
            {
                /// TODO: Error email field
                /// 

                await DisplayAlert("Error", "Sorry - Invalid Email, try again", "OK");

                return false;
            }
            else
            {
                return CheckIfEmail(email);
            }
        }

        private string getTodayDate()
        {
            return DateTime.Today.Date.ToString();
        }

        private bool CheckIfEmail(string email)
        {

            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
      
    }
}