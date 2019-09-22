using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nasiye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Payment : ContentPage
    {
        string sprice;
        public Payment(string price)
        {
            InitializeComponent();
            amount.Text = price;
            sprice = price;
        }

        private void Edahab_Clicked(object sender, EventArgs e)
        {
            string number = "*110*661232438*" + sprice;
            try
            {
                PhoneDialer.Open(number);
            }
            catch (ArgumentNullException anEx)
            {
                // Number was null or white space
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private async void Cash_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Payment", "To pay cash please visit our office, thank you", "OK");
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}