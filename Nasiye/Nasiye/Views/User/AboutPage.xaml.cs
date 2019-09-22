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
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            loader.IsVisible = false;
            webview.IsVisible = true;

        }

        private void Webview_Navigating(object sender, WebNavigatingEventArgs e)
        {
            loader.IsVisible = true;
            webview.IsVisible = false;

        }
    }
}