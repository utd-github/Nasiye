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
    public partial class SigninPage : ContentPage
    {
        public readonly IFirebaseAuthService _firebaseAuth;

        public readonly IFirebaseDatabaseService _firebaseDatabase;


        public SigninPage()
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();


            MessagingCenter.Subscribe<object, string>(this, "INFO", (sender, data) =>
            {
                if (data != null)
                {
                    ShowInfo(data);
                }
            });


            MessagingCenter.Subscribe<object, string>(this, "seccuss", (sender, data) =>
            {
                if (data != null)
                {
                    UpdateUINavigate(data);
                }
            });


        }


        private void ShowInfo(string data)
        {
           
            if(data == "SENT")
            {
                info.Text = "Waiting for  OTP verfication";
                info.IsVisible = true;
            }
            else
            {
                loader.IsVisible = false;
                container.IsVisible = true;
                ephone.Focus();
                DisplayAlert("Sigin", data, "OK");

            }
        }

        private void UpdateUINavigate(string data)
        {
            loader.IsVisible = false;
            container.IsVisible = true;
            ephone.Focus();
            App.Current.MainPage = new NavigationPage(new SignupPage());
        }

        private void Signin_Clicked(object s, EventArgs e)
        {
            if (ephone.Text.Trim() != null && ephone.Text.Count() == 9)
            {
                MessagingCenter.Subscribe<object, string>(this, "INFO", (sender, data) =>
                {
                    if (data != null)
                    {
                        ShowInfo(data);
                    }
                });


                MessagingCenter.Subscribe<object, string>(this, "seccuss", (sender, data) =>
                {
                    if (data != null)
                    {
                        UpdateUINavigate(data);
                    }
                });

                _firebaseAuth.LoginWithPhoneAsync("+252" + ephone.Text.Trim());

                // Show Loader
                container.IsVisible = false;
                loader.IsVisible = true;
            }
            else
            {
                DisplayAlert("Sing ing", "Please entry a valid number!", "OK");
            }


        }
    }
}