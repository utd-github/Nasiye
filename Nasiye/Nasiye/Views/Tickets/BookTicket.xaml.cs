using Nasiye.Models;
using Nasiye.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nasiye.Views.Tickets
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookTicket : ContentPage
    {
        TicketModel ticketModel;
        public readonly IFirebaseDatabaseService _firebaseDatabase;
        public readonly IFirebaseAuthService _firebaseAuth;

        public BookTicket(object ticket)
        {
            InitializeComponent();
            ticketModel = ticket as TicketModel;

            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();


            ViewTicket(ticketModel);
        }

        private void ViewTicket(TicketModel ticketModel)
        {
            airline.Text = ticketModel.Airline;
            image.Source = ticketModel.Image;
            to.Text = ticketModel.To;
            from.Text = ticketModel.From;
            code.Text = ticketModel.Code;

            loader.IsVisible = false;
            container.IsVisible = true;

        }

        private async void Book_Clicked(object sender, EventArgs e)
        {
            // Get Data
            BookTicketModel bookTicket = new BookTicketModel();

            bookTicket.Key = "00";
            bookTicket.Ticket = ticketModel;

            bookTicket.Type = type.SelectedItem.ToString();
            bookTicket.Date = date.Date.ToString();

            long total = ticketModel.Price * long.Parse(quantity.SelectedItem.ToString());

            bookTicket.Amount = total.ToString();
            bookTicket.Status = "Pending";
            bookTicket.Quantity = quantity.SelectedItem.ToString();

            string uid = await _firebaseAuth.GetCurrentUser();

            bookTicket.UserUID = uid;


            _firebaseDatabase.SetBooking("ticketbookings", bookTicket);

            await DisplayAlert("Booking", "Your ticket is booked", "OK");
            await Navigation.PopAsync();
        }

        private void Quantity_SelectedIndexChanged(object sender, EventArgs e)
        {
            book.IsEnabled = true;
        }
    }
}