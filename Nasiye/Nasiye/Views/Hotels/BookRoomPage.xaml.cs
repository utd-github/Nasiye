using Nasiye.Models;
using Nasiye.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nasiye.Views.Hotels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookRoomPage : ContentPage
    {
        public readonly IFirebaseDatabaseService _firebaseDatabase;
        public readonly IFirebaseAuthService _firebaseAuth;

        RoomModel Room = new RoomModel();
        HotelModel BHotel;

        public BookRoomPage(object room, object hotel)
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();

            Room = room as RoomModel;

            BHotel = hotel as HotelModel;

            ShowRoom(Room, BHotel);
        }

        private void ShowRoom(RoomModel room, HotelModel Hotel)
        {
            image.Source = new UriImageSource
            {
                Uri = room.Image
            };

            amount.Text = room.Amount;
            hotel.Text = room.Hotel;
            address.Text = room.Address;
            roomtype.Text = room.Type;
            BHotel = Hotel;
        }

        private async void Book_Clicked(object sender, EventArgs e)
        {
            BookRoomModel bookRoom = new BookRoomModel();
            

            bookRoom.UserUID = await _firebaseAuth.GetCurrentUser();

            bookRoom.Room = Room;
            bookRoom.Key = "00";
            bookRoom.Date = getTodayDate();
            bookRoom.Amount = Room.Amount;
            bookRoom.Status = "Pending";
            bookRoom.From = from.Date.ToLocalTime().ToShortDateString();
            bookRoom.To = to.Date.ToLocalTime().ToShortDateString();
            bookRoom.Hotel = BHotel;

            _firebaseDatabase.SetBooking("hotelbookings", bookRoom);

            await DisplayAlert("Booking", "Your room is booked", "OK");

            await Navigation.PopAsync();
        }

        private string getTodayDate()
        {
            return DateTime.Today.Date.ToString();
        }
    }
}