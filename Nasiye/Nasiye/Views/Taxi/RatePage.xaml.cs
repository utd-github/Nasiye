using Nasiye.Models;
using Nasiye.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nasiye.Views.Taxi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RatePage : ContentPage
    {
        public readonly IFirebaseAuthService _firebaseAuth;
        public readonly IFirebaseDatabaseService _firebaseDatabase;

        TripModel Trip;

        public RatePage(object trip)
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();

            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();

            Trip = new TripModel();
            Trip = trip as TripModel;


            ShowTripInfo(trip as TripModel);
        }

        private void ShowTripInfo(TripModel tripModel)
        {

            Driver user = tripModel.Driver;

            driverImage.Source = ImageSource.FromUri(new Uri(user.Image));

            drivername.Text = user.Name;
            price.Text = "$ " + tripModel.Amount;
            duration.Text = tripModel.Duration + " min";
            distance.Text = tripModel.Distance + " KM";
        }

        private void Submit_Clicked(object sender, EventArgs e)
        {
            // Submit Comment and stars
            // Get Rating
            // Get Any comments
            submit.IsEnabled = false;
            double rated = rating.Value;

            string payment = payments.SelectedItem.ToString();

            _firebaseDatabase.SubmitTripRating(Trip.Key, rated, payment);
            _firebaseDatabase.RemoveGetProfile("users");

            App.Current.MainPage = new NavigationPage(new MainPage());


        }
    }
}