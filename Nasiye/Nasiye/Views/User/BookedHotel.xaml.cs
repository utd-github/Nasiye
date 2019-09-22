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
    public partial class BookedHotel : ContentPage
    {

        public readonly IFirebaseAuthService _firebaseAuth;

        public readonly IFirebaseDatabaseService _firebaseDatabase;


        public BookedHotel()
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();
        }

        private void GetBookings()
        {
            List<BookRoomModel> bookedTickets = new List<BookRoomModel>();

            Action<Dictionary<string, BookRoomModel>> onValueEvent = (Dictionary<string, BookRoomModel> tickets) =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("---> EVENT GetDataFromFirebase ");

                    Action onSetValueSuccess = () =>
                    {

                    };

                    Action<string> onSetValueError = (string errorDesc) =>
                    {

                    };

                    if (tickets != null)
                    {
                        info.IsVisible = false;

                        if (tickets.Count != 0 && tickets.Count != bookedTickets.Count)
                        {

                            bookedTickets.Clear();

                            foreach (KeyValuePair<string, BookRoomModel> item in tickets)
                            {
                                if (bookedTickets.All(d => d.Key != item.Value.Key))
                                {
                                    bookedTickets.Add(item.Value);
                                }
                            }

                            bookingsList.IsRefreshing = false;
                            bookingsList.ItemsSource = bookedTickets;
                        }
                    }
                    else
                    {
                        bookedTickets.Clear();
                        bookingsList.IsRefreshing = false;
                        bookingsList.IsVisible = false;
                        info.IsVisible = true;
                    }

                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("---> error GetDataFromFirebase " + ex.Message + " , Source" + ex.Source);
                    throw;
                }
            };

            _firebaseDatabase.GetBookedHotels("hotelbookings", onValueEvent);
        }


        private void BookingsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            bookingsList.SelectedItem = null;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            GetBookings();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _firebaseDatabase.RemoveGetBookedHotels("hotelbookings");

        }
    }
}