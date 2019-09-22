using Nasiye.CustomRenderers;
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
    public partial class HotelMainPage : ContentPage
    {
        public readonly IFirebaseDatabaseService _firebaseDatabase;

        public HotelMainPage(string city)
        {
            InitializeComponent();
            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();


            GetHotels(city);

        }

        private void GetHotels(string city)
        {
            List<HotelModel> hotelsList = new List<HotelModel>();

            Action<Dictionary<string, HotelModel>> onValueEvent = null;

            onValueEvent = (Dictionary<string, HotelModel> hotels) =>
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

                    if (hotels != null)
                    {
                        if (hotels.Count == 0)
                        {
                            hotellist.IsVisible = false;
                            info.IsVisible = true;
                        }

                        if (hotels.Count != 0 && hotels.Count != hotelsList.Count)
                        {
                            hotellist.IsVisible = true;
                            info.IsVisible = false;
                            hotelsList.Clear();

                            foreach (KeyValuePair<string, HotelModel> item in hotels)
                            {
                                if (hotelsList.All(d => d.Key != item.Value.Key))
                                {
                                    if (item.Value.Status)
                                    {
                                        if(city == null)
                                        {
                                            hotelsList.Add(item.Value);
                                        }
                                        else
                                        {
                                            if (item.Value.City == city.Trim())
                                            {
                                                hotelsList.Add(item.Value);
                                            }
                                        }
                                    }
                                }
                            }
                            if (hotelsList.Count > 0)
                            {
                                hotellist.ItemsSource = hotelsList;
                                hotellist.IsRefreshing = false;
                                info.IsVisible = false;
                            }
                            else
                            {
                                hotellist.IsRefreshing = false;
                                info.IsVisible = true;
                            }
                        }
                    }
                    else
                    {
                        hotellist.IsVisible = false;
                        info.IsVisible = true;
                    }
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("---> error GetDataFromFirebase " + ex.Message);
                }
            };

            _firebaseDatabase.GetHotels("hotels", onValueEvent);
        }

        private async void Hotellist_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            HotelModel item = e.SelectedItem as HotelModel;
            if (item != null)
            {
                hotellist.SelectedItem = null;

                await Navigation.PushAsync(new HotelPage(item));
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _firebaseDatabase.RemoveGetHotels("hotels");
        }
    }
}