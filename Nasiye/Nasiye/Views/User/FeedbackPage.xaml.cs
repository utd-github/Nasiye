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
    public partial class FeedbackPage : ContentPage
    {
        public readonly IFirebaseAuthService _firebaseAuth;

        public readonly IFirebaseDatabaseService _firebaseDatabase;


        public FeedbackPage()
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDatabaseService>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthService>();
        }

        private async  void Submit_Clicked(object sender, EventArgs e)
        {
            FeedbackModel feedback = new FeedbackModel();
            var uid = await _firebaseAuth.GetCurrentUser();
            // Get input
            feedback.Key = "00";
            feedback.Subject = subject.SelectedItem.ToString();
            feedback.Body = body.Text;
            feedback.Date = DateTime.Today.Date.ToString();
            feedback.Status = false;
            feedback.UserUID = uid;

            _firebaseDatabase.SubmitFeedback(feedback);

            await Navigation.PopAsync();
        }

        private void Body_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (body.Text.Length > 10)
            {
                submit.IsEnabled = true;
            }
            else
            {
                submit.IsEnabled = false;
            }
        }
    }
}