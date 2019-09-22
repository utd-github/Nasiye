using Nasiye.CustomRenderers;
using Nasiye.Models;
using Nasiye.Services;
using Nasiye.Views.Hotels;
using Nasiye.Views.Taxi;
using Nasiye.Views.Tickets;
using Nasiye.Views.User;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Nasiye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        public readonly IFirebaseAuthService _firebaseAuth;

        public readonly IFirebaseDatabaseService _firebaseDatabase;

        List<HotelModel> hotelsList = new List<HotelModel>();

        Models.Location userlocation = new Models.Location();
        ObservableCollection<DriverModel> DriversList { get; set; } = new ObservableCollection<DriverModel>();
        RequestModel requestModel = new RequestModel();

        List<TicketModel> ticketsList = new List<TicketModel>();

        bool cairline = false, cto = false, cfrom = false;

        string state = "N";


        public MainPage()
        {
            InitializeComponent();
            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();

            Device.StartTimer(new TimeSpan(0, 0, 3), () =>
            {
                Geo_Clicked(null, null);
                return true;
            });


        }

        private void GetHotels()
        {

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
                                        hotelsList.Add(item.Value);
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
                userlocation = local;
                GetUserLocation(local);
            });
        }

        private async void GetUserLocation(Models.Location location)
        {
            try
            {
                if (location != null)
                {
                    mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Xamarin.Forms.Maps.Position(location.Lat, location.Lng),
                        Distance.FromMiles(1)).WithZoom(2));

                    loader.IsVisible = false;
                    mainmap.IsVisible = true;

                    geo.IsEnabled = true;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await DisplayAlert("Location", "Sorry, Maps not supported!", "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await DisplayAlert("Location", "Sorry, Maps not enabled!", "OK");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                await DisplayAlert("Location", "Sorry, Maps need permission", "OK");
            }
            catch (Exception ex)
            {
                // Unable to get location
                await DisplayAlert(ex.Source, "ERROR: " + ex.Message, "OK");
            }
        }

        private void GetDrivers()
        {

            Action<Dictionary<string, DriverModel>> onValueEvent = null;
            onValueEvent = (Dictionary<string, DriverModel> drivers) =>
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

                    if (drivers == null)
                    {
                        mainmap.Pins.Clear();
                        DriversList.Clear();
                        hailbtn.IsEnabled = false;
                    }
                    else
                    {
                        if (drivers.Count != 0 && drivers.Count != DriversList.Count)
                        {
                            mainmap.Pins.Clear();
                            DriversList.Clear();

                            foreach (KeyValuePair<string, DriverModel> item in drivers.ToList())
                            {
                                if (DriversList.All(d => d.Key != item.Value.Key))
                                {
                                    DriversList.Add(item.Value);
                                }
                            }

                            if (DriversList.Count > 0)
                            {
                                ShowDrivers(DriversList);
                                _ = StartTrackingAsync();
                                hailbtn.IsEnabled = true;
                            }
                            else
                            {
                                hailbtn.IsEnabled = false;
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("---> error GetDataFromFirebase " + ex.Message);
                    throw;
                }
            };

            _firebaseDatabase.GetDrivers("drivers", onValueEvent);
        }

        private void ShowDrivers(ObservableCollection<DriverModel> driverslist)
        {
            mainmap.Pins.Clear();

            foreach (DriverModel driver in driverslist)
            {
                var pin = new CustomPin
                {
                    Position = new Xamarin.Forms.Maps.Position(driver.Location.Lat, driver.Location.Lng),
                    Label = "Xamarin San Francisco Office",
                    Address = "394 Pacific Ave, San Francisco CA",
                    MarkerId = "Xamarin",
                    Icon = "driver"
                };
                mainmap.CustomPins = new List<CustomPin> { pin };
                mainmap.Pins.Add(pin);
            }

        }

        private void Geo_Clicked(object sender, EventArgs e)
        {
            geo.IsEnabled = false;
            mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Xamarin.Forms.Maps.Position(userlocation.Lat, userlocation.Lng),
                Distance.FromMiles(1)).WithZoom(2));
            geo.IsEnabled = true;
        }

        private void Hailbtn_Clicked(object sender, EventArgs e)
        {
            if (DriversList.Count > 0)
            {
                reqloader.IsVisible = true;

                state = "H";
                // Hide hail button and show cancel btn
                cancelbtn.IsEnabled = true;
                hailbtn.IsVisible = false;
                cancelbtn.IsVisible = true;

                // Get Drivers
                GetNearestDriver();
            }
        }

        private async void HailTaxi(DriverModel driver)
        {
            state = "H";
            string uid = await _firebaseAuth.GetCurrentUser();

            Action<Dictionary<string, UserModel>> onValueEvent = null;
            onValueEvent = (Dictionary<string, UserModel> user) =>
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

                    if (user != null)
                    {
                        foreach (KeyValuePair<string, UserModel> item in user)
                        {
                            if (item.Value.Key == uid)
                            {
                                HailWithUserInfo(item.Value, driver);
                            }
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("---> error GetDataFromFirebase " + ex.Message);
                    throw;
                }
            };


            _firebaseDatabase.GetProfile("users", onValueEvent);

        }

        private void HailWithUserInfo(UserModel User, DriverModel driver)
        {
            RequestModel req = new RequestModel();

            Models.User user = new Models.User();

            Driver d = new Driver();

            req.Key = "00";

            user.Name = User.Name;
            user.Phone = User.Phone;
            user.Rating = User.Rating.ToString();
            user.Image = User.Image.ToString();
            user.Key = User.Key;
            user.Location = userlocation;

            req.User = user;

            req.Type = "User";
            req.Status = "Pending";


            req.Date = DateTime.Today.ToShortDateString();


            d.Name = driver.Name;
            d.Key = driver.Key;
            d.Phone = driver.Phone;
            d.Rating = driver.Rating.ToString();
            d.Location = driver.Location;
            d.Image = driver.Image;
            d.Vehicle = driver.Vehicle;

            req.Driver = d;

            if (req.User.Key != null)
            {
                _firebaseDatabase.SetRequest(req.Driver.Key, req);
                requestModel = req;
                ListenRequest(req);
            }


        }

        private void ListenRequest(RequestModel req)
        {

            state = "L";

            Action<Dictionary<string, RequestModel>> onValueEvent = (Dictionary<string, RequestModel> request) =>
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

                    if (request != null)
                    {
                        foreach (KeyValuePair<string, RequestModel> item in request)
                        {
                            if (item.Key == req.Driver.Key)
                            {
                                SetRequest(item.Value);
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

            _firebaseDatabase.GetRequests("requests", onValueEvent);

        }

        private async void SetRequest(RequestModel req)
        {

            if (req != null)
            {
                if (req.Status == "Accepted")
                {
                    reqloader.IsVisible = false;
                    cancelbtn.IsEnabled = true;

                    // Navigate TO trip Page
                    await Navigation.PushAsync(new TripPage(req.Key));

                }
                else if (req.Status == "Canceled")
                {
                    Cancel_Clicked();
                }
            }
        }

        private async void GetNearestDriver()
        {
            Xamarin.Essentials.Location userlocal = new Xamarin.Essentials.Location(userlocation.Lat, userlocation.Lng);

            var unit = DistanceUnits.Kilometers;

            double Adistance = 0;

            if (DriversList.Count == 1)
            {
                Xamarin.Essentials.Location dlocation = new Xamarin.Essentials.Location(DriversList[0].Location.Lat, DriversList[0].Location.Lng);

                double distance = Xamarin.Essentials.Location.CalculateDistance(userlocal, dlocation, unit);

                if (distance < 10)
                {
                    //HailTaxi(User, drivers[0].Key);
                    HailTaxi(DriversList[0]);
                }
                else
                {
                    // no near driver found
                    await DisplayAlert("Taxi", "Sorry no drivers near you at the moment", "OK");
                    reqloader.IsVisible = false;
                    // Hide Cancel and show haild
                    cancelbtn.IsVisible = false;
                    hailbtn.IsVisible = true;
                    return;
                }
            }
            else
            {
                DriverModel fdriver = new DriverModel();
                foreach (var ldriver in DriversList)
                {
                    // Driver location

                    var driverlocal = new Xamarin.Essentials.Location(ldriver.Location.Lat, ldriver.Location.Lng);

                    var distance = Xamarin.Essentials.Location.CalculateDistance(userlocal, driverlocal, unit);

                    if (Adistance == 0)
                    {
                        Adistance = distance;
                        fdriver = ldriver;
                    }
                    else
                    {
                        if (distance < Adistance)
                        {
                            Adistance = distance;
                            fdriver = ldriver;
                        }
                        else
                        {
                            Adistance = distance;
                            fdriver = ldriver;
                        }

                    }

                    if (Adistance >= 10)
                    {
                        // no near driver found
                        await DisplayAlert("Hail Taxi", "Sorry no drivers near you at the moment", "OK");
                        reqloader.IsVisible = false;
                        // Hide Cancel and show haild
                        cancelbtn.IsVisible = false;
                        hailbtn.IsVisible = true;
                    }
                    else
                    {
                        //HailTaxi(User, drivers[0].Key);
                        HailTaxi(fdriver);
                    }

                }


            }

        }

        private void GetTickets()
        {

            Action<Dictionary<string, TicketModel>> onValueEvent = (Dictionary<string, TicketModel> tickets) =>
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
                        ticketinfo.IsVisible = false;

                        if (tickets.Count != 0 && tickets.Count != ticketsList.Count)
                        {

                            ticketsList.Clear();

                            foreach (KeyValuePair<string, TicketModel> item in tickets)
                            {
                                if (ticketsList.All(d => d.Key != item.Value.Key))
                                {
                                   

                                    ticketsList.Add(item.Value);
                                }
                            }

                        }
                    }
                    else
                    {
                        ticketsList.Clear();
                    }

                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("---> error GetDataFromFirebase " + ex.Message + " , Source" + ex.Source);
                    throw;
                }
            };

            _firebaseDatabase.GetTickets("tickets", onValueEvent);
        }

        private async void Ticketslist_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            TicketModel item = e.SelectedItem as TicketModel;
            if (item != null)
            {
                ticketslist.SelectedItem = null;
                await Navigation.PushAsync(new BookTicket(item));
            }
        }

        private void Cancel_Clicked()
        {
            // Cancel Request
            _firebaseDatabase.SetRequest(requestModel.Driver.Key, null);
            reqloader.IsVisible = false;
            // Hide Cancel and show haild
            cancelbtn.IsVisible = false;
            hailbtn.IsVisible = true;
        }

        private void Cancelbtn_Clicked(object sender, EventArgs e)
        {
            Cancel_Clicked();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            GetDrivers();
            GetHotels();
            GetTickets();
            await StartTrackingAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (state != "N")
            {
                _firebaseDatabase.RemoveGetRequests("requests");

                state = "N";
            }
            _firebaseDatabase.RemoveGetHotels("hotels");
            _firebaseDatabase.RemoveGetTickets("tickets");
            _firebaseDatabase.RemoveGetDrivers("drivers");
        }

        protected override bool OnBackButtonPressed()
        {
            App.Current.MainPage = new NavigationPage(new MainPage());

            return true;
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

        private void Sairline_SelectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            cairline = true;
            if (cairline && cto && cfrom)
            {
                ticketsearch.IsEnabled = true;
            }

        }

        private void Sfrom_SelectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            cfrom = true;
            if (cairline && cto && cfrom)
            {
                ticketsearch.IsEnabled = true;
            }
        }

        private void Sto_SelectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            cto = true;
            if (cairline && cto && cfrom)
            {
                ticketsearch.IsEnabled = true;
            }
        }

        private void AutoComplete_SelectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {


            List<HotelModel> res = new List<HotelModel>();

            foreach (HotelModel hotel in hotelsList)
            {
                if(searchitem.SelectedItem.ToString() != null & hotel.City == searchitem.SelectedItem.ToString())
                {
                    res.Add(hotel);
                }
            }

            hotellist.ItemsSource = res;
        }

        private void Ticketsearch_Clicked(object sender, EventArgs e)
        {

            List<TicketModel> tlist = new List<TicketModel>();


            if (cairline && cto && cfrom)
            {

                foreach (TicketModel ticket in ticketsList)
                {
                    if (ticket.Airline == sairline.SelectedItem.ToString() && ticket.From == sfrom.SelectedItem.ToString() && ticket.To == sto.SelectedItem.ToString())
                    {
                        tlist.Add(ticket);
                    }
                }
                if (tlist != null) 
                {
                    ticketslist.ItemsSource = tlist;
                }
                else
                {
                    ticketinfo.IsVisible = true;
                    ticketslist.IsVisible = false;
                }
            }
        }
        
    }
}