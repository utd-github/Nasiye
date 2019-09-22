using Nasiye.Models;
using Nasiye.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nasiye.Views.User
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public readonly IFirebaseAuthService _firebaseAuth;

        public readonly IFirebaseDatabaseService _firebaseDatabase;

        public ProfilePage(Models.UserModel user)
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();

            SetProfile(user);

            GetProfile();

        }

        private void Editbtn_Clicked(object sender, EventArgs e)
        {
            username.IsEnabled = true;
            userimage.IsEnabled = true;
            editbtn.IsVisible = false;
            
            savebtn.IsVisible = true;
        }

        private async void Savebtn_Clicked(object sender, EventArgs e)
        {
            string auth = await _firebaseAuth.GetCurrentUser();

            if (auth != null)
            {
                string name = username.Text.Trim();

                _firebaseDatabase.UpdateUserName(auth,name);
            }

            username.IsEnabled = false;
            userimage.IsEnabled = false;
            editbtn.IsVisible = true;
            savebtn.IsVisible = false;
        }

        private async void Userimage_Clicked(object sender, EventArgs e)
        {
            userimage.IsEnabled = false;

            string auth = await _firebaseAuth.GetCurrentUser();

            if (auth != null)
            {
                var imagelocation = await _firebaseDatabase.UploadFIle();
                if (imagelocation != null)
                {
                   string imageurl = await _firebaseDatabase.GetFileUrl(imagelocation);
                    _firebaseDatabase.UpdateUserImage(auth, imageurl);
                }
            }



            userimage.IsEnabled = true;

        }

        public void SetProfile(UserModel userModel)
        {
            userimage.Source = new UriImageSource
            {
                Uri = userModel.Image
            };
            username.Text = userModel.Name;
            useremail.Text = userModel.Email;
            userphone.Text = userModel.Phone;
        }

        private async void GetProfile()
        {
            string uid = await _firebaseAuth.GetCurrentUser();

            Action<Dictionary<string, UserModel>> onValueEvent = (Dictionary<string, UserModel> users) =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("---> EVENT Get Profile Firebase ");

                    Action onSetValueSuccess = () =>
                    {

                    };

                    Action<string> onSetValueError = (string errorDesc) =>
                    {

                    };

                    if (users != null)
                    {
                        foreach (KeyValuePair<string, UserModel> item in users)
                        {
                            if (item.Value.Key == uid)
                            {
                                SetProfile(item.Value);
                            }
                        }


                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("---> error Get Profile Firebase " + ex.Message);
                    throw;
                }
            };

            _firebaseDatabase.GetProfile("users", onValueEvent);
        }

    }
}