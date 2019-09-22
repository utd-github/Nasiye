using Nasiye.Models;
using Nasiye.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nasiye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainContainerPage : MasterDetailPage
    {
        public readonly IFirebaseAuthService _firebaseAuth;

        public readonly IFirebaseDatabaseService _firebaseDatabase;

        UserModel user;

        public MainContainerPage()
        {
            InitializeComponent();


            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();


            user = new UserModel();
        }

        public void SetProfile(UserModel userModel)
        {
            user = userModel;
            userimage.Source = new UriImageSource
            {
                Uri = userModel.Image
            };
            username.Text = userModel.Name;
            
            userphone.Text = userModel.Phone;

            loader.IsVisible = false;
            profilecon.IsVisible = true;
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
                            if (item.Key == uid)
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

        private void MenuItemList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListItemModel item = e.SelectedItem as ListItemModel;
            if (item != null)
            {
                NavigationPage nav = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));

                IsPresented = false;


                Detail = nav;

                MenuItemList.SelectedItem = null;


            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetProfile();
        }


        protected override void OnDisappearing()
        {

            base.OnDisappearing();
            _firebaseDatabase.RemoveGetProfile("users");
            
        }
    }
}