using Nasiye.Services;
using Nasiye.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nasiye
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTI1OTgzQDMxMzcyZTMyMmUzMFhldFRrMmk3SWdoY3g2bEFBd29yNWJRNlRib3RWK3lITWszbW1tRlozdW89");

            InitializeComponent();

            NetworkAccess current = Connectivity.NetworkAccess;


            if (current == NetworkAccess.Internet)
            {

                var auth = DependencyService.Get<IFirebaseAuthService>().GetCurrentUser();

                if (auth != null)
                {
                    MainPage = new MainContainerPage();
                }
                else
                {
                    MainPage = new NavigationPage(new GettingStartedPage());
                }


            }
            else
            {
                MainPage = new NavigationPage(new CheckPage());
            }


        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
