using Nasiye.Models;
using Nasiye.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nasiye.Views.Hotels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HotelPage : ContentPage
    {
        public readonly IFirebaseDatabaseService _firebaseDatabase;

        HotelModel Hotel;

        public HotelPage(object hotel)
        {
            InitializeComponent();

            Hotel = hotel as HotelModel;

            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();

            ViewHotelInfo(Hotel);


        }

        private void ViewHotelInfo(HotelModel hotel)
        {

            image.Source = new UriImageSource
            {
                Uri = new Uri(hotel.Image)
            };

            container.Title = hotel.Name;
            info.Text = hotel.Info;
            name.Text = hotel.Name;
            address.Text = hotel.Address;
            city.Text = hotel.City;


            GetRooms(hotel.Key);
        }

        private void GetRooms(string key)
        {
            List<RoomModel> roomsList = new List<RoomModel>();

            Action<Dictionary<string, RoomModel>> onValueEvent = null;

            onValueEvent = (Dictionary<string, RoomModel> rooms) =>
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

                    if (rooms != null)
                    {
                        if (rooms.Count == 0)
                        {
                            roomlist.IsVisible = false;
                            info.IsVisible = true;
                        }

                        if (rooms.Count != 0 && rooms.Count != roomsList.Count)
                        {
                            roomlist.IsVisible = true;
                            info.IsVisible = false;
                            roomsList.Clear();

                            foreach (KeyValuePair<string, RoomModel> item in rooms)
                            {
                                if (roomsList.All(d => d.Key != item.Value.Key))
                                {
                                    if(item.Value.Status)
                                    {
                                        roomsList.Add(item.Value);
                                    }
                                }
                            }
                            if (roomsList.Count > 0)
                            {
                                roomlist.ItemsSource = roomsList;
                                roomlist.IsRefreshing = false;
                            }
                            else
                            {
                                roomlist.IsRefreshing = false;
                            }
                        }
                    }
                    else
                    {
                        roomlist.IsVisible = false;
                        info.IsVisible = true;
                    }
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("---> error GetDataFromFirebase " + ex.Message);
                }
            };

            _firebaseDatabase.GetRooms("rooms",key, onValueEvent);
        }


        private async void Roomlist_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            RoomModel item = e.SelectedItem as RoomModel;
            if (item != null)
            {
                roomlist.SelectedItem = null;

                await Navigation.PushAsync(new BookRoomPage(item, Hotel));
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _firebaseDatabase.RemoveGetRooms("rooms");
        }
    }
}