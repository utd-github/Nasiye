using Nasiye.CustomRenderers;
using Nasiye.Models;
using Nasiye.Services;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Nasiye.Views.Taxi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TripPage : ContentPage
    {
        private TripModel Trip = new TripModel();

       

        public readonly IFirebaseDatabaseService _firebaseDatabase;
        public readonly IFirebaseAuthService _firebaseAuth;

        public TripPage(string key)
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);

            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();


            // Get Trip Info
            GetTrip(key);
            _ = StartTrackingAsync();


        }

        private void GetTrip(string key)
        {

         


            Action<Dictionary<string, TripModel>> onValueEvent = (Dictionary<string, TripModel> trips) =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("---> EVENT Get Request Data FromFirebase ");

                    Action onSetValueSuccess = () =>
                    {

                    };

                    Action<string> onSetValueError = (string errorDesc) =>
                    {

                    };

                    if (trips != null && trips.Count > 0)
                    {
                        foreach (KeyValuePair<string, TripModel> item in trips)
                        {
                            if (item.Value.Key == key)
                            {
                                SetTrip(item.Value);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("---> error Get Request Data FromFirebase " + ex.Message);
                    throw;
                }
            };

            _firebaseDatabase.GetTrips("trips", onValueEvent);

        }


        private void ShowMap(Models.Location location, Driver driver)
        {
            mainmap.Pins.Clear();

            mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(location.Lat, location.Lng), Distance.FromMiles(1)).WithZoom(2));

            var pin = new CustomPin
            {
                Position = new Xamarin.Forms.Maps.Position(driver.Location.Lat, driver.Location.Lng),
                Label = "",
                Address = "",
                MarkerId = "",
                Icon = "driver"
            };

            mainmap.CustomPins = new List<CustomPin> { pin };

            mainmap.Pins.Add(pin);


            mainmap.IsVisible = true;
        }


        private async Task StartTrackingAsync(bool tracking = true)
        {
            try
            {
                PermissionStatus hasPermission = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (hasPermission != PermissionStatus.Denied)
                {
                    // Permission Granted
                    // check for req
                    if (tracking)
                    {
                        // Check is listening...
                        if (CrossGeolocator.Current.IsListening)
                        {
                            // Start
                            CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                            CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;
                        }
                        else
                        {
                            CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                            CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;

                            object aType = "Automotive Navigation";
                            if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 50,
                                true, new ListenerSettings
                                {
                                    ActivityType = ActivityType.AutomotiveNavigation,
                                    AllowBackgroundUpdates = true,
                                    DeferLocationUpdates = true,
                                    DeferralDistanceMeters = 100,
                                    DeferralTime = TimeSpan.FromSeconds(10),
                                    ListenForSignificantChanges = true,
                                    PauseLocationUpdatesAutomatically = false
                                }))
                            {
                                tracking = true;
                            }
                        }

                    }
                    else
                    {
                        CrossGeolocator.Current.PositionChanged -= CrossGeolocator_Current_PositionChanged;
                        CrossGeolocator.Current.PositionError -= CrossGeolocator_Current_PositionError;

                        await CrossGeolocator.Current.StopListeningAsync();

                        tracking = false;
                    }
                }
                else
                {
                    // Permission Denied
                    await DisplayAlert("Location Error", "Error Getting Location", "OK");
                }
            }
            catch (Exception ex)
            {
                // Error Accured
                await DisplayAlert("Location Error", "Error Getting Location: " + ex.Message, "OK");
            }
        }

        private void CrossGeolocator_Current_PositionError(object sender, PositionErrorEventArgs e)
        {

        }

        private void CrossGeolocator_Current_PositionChanged(object sender, PositionEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Models.Location local = new Models.Location();

                var position = e.Position;

                mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Xamarin.Forms.Maps.Position(
                        position.Latitude, position.Longitude),
                        Distance.FromMiles(1)).WithZoom(2)
                        );

                local.Lat = position.Latitude;
                local.Lng = position.Longitude;
            });
        }




        private async void SetTrip(TripModel trip)
        {

            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                ShowMap(Trip.Location, Trip.Driver);
                return true;
            });

            Driver driver = trip.Driver;

            status.Text = trip.Status;
            ShowMap(trip.Location, driver);
            Trip = trip;

            if (trip.Status == "Accepted") {
                status.Text = trip.Status;

                // SHow driver info
                drivername.Text = driver.Name;
                drivervehicle.Text = driver.Vehicle.Type;
                drivervehicleplate.Text = driver.Vehicle.Plate;
                driverrating.Value = long.Parse(driver.Rating);
                driverimage.Source = new UriImageSource
                {
                    Uri = new Uri(driver.Image)
                };
                status.Text = "Waiting";

                iloader.IsVisible = false;
                infobox.IsVisible = true;

                // Show Map
                ShowMap(trip.Location, trip.Driver);
                //
                loader.IsVisible = false;
                cancelbtn.IsVisible = true;
            }
            else if (trip.Status == "Started") 
            {
                mainmap.IsShowingUser = false;
                ShowMap(trip.Location, trip.Driver);

                status.Text = trip.Status;

                status.Text = "On Going";

                amount.Text = "$ " + trip.Amount;
                distance.Text = trip.Distance + " KM";
                duration.Text = trip.Duration + " min";

                infobox.IsVisible = false;
                cancelbtn.IsVisible = true;
                ftripcon.IsVisible = true;

            }
            else if (trip.Status == "End")
            {
                // Navigate to Rate page
                await Navigation.PushAsync(new RatePage(trip));
            }
            else if (trip.Status == "Canceled")
            {
                // Navigate to root
                await DisplayAlert("Trip", "Trip is canceled by driver", "OK");
                App.Current.MainPage = new NavigationPage(new MainPage());
            }
            else if (trip.Status == "Paused")
            {
                ShowMap(trip.Location, trip.Driver);
                mainmap.IsShowingUser = true;
            }
        }

        private async void Cancelbtn_Clicked(object sender, EventArgs e)
        {
            
            // Confirm
            if(await DisplayAlert("Trip", "Do you want Cancel trip?", "YES", "NO"))
            {
                _firebaseDatabase.CancelTrip(Trip.Key);
            }

        }

        private void Calldriver_Clicked(object sender, EventArgs e)
        {
            string phonenumber = Trip.Driver.Phone;

            try
            {
                PhoneDialer.Open(phonenumber);
            }
            catch (ArgumentNullException anEx)
            {
                DisplayAlert("Error", "Phone number not found", "OK");
            }
            catch (FeatureNotSupportedException ex)
            {
                DisplayAlert("Error", "Sorry this feature is not supported!", "OK");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                DisplayAlert("Error", "Sorry Error accured try calling manually!", "OK");

            }

        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _firebaseDatabase.RemoveGetTrips("trips");

        }
    }
}