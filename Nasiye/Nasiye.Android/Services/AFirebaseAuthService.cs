using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Java.Util.Concurrent;
using Nasiye.Droid.Services;
using Nasiye.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AFirebaseAuthService))]
namespace Nasiye.Droid.Services
{
    class AFirebaseAuthService : IFirebaseAuthService
    {

        FirebaseAuth mAuth;

        PhoneAuthProvider PhoneAuthProvider;

      

        public AFirebaseAuthService()
        {
            mAuth = FirebaseAuth.GetInstance(MainActivity.app);
            PhoneAuthProvider = PhoneAuthProvider.Instance;
        }
        
        public async Task<string> CreateUserAsync(string email, string password)
        {

            try
            {
                IAuthResult res;

                res = await  mAuth.CreateUserWithEmailAndPasswordAsync(email, password);
                if(res.User.Uid != null)
                {
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        public Task<string> GetCurrentUser()
        {
            var user = mAuth.CurrentUser;

            if (user != null)
            {
                // User is signed in
                return Task.FromResult(user.Uid.ToString());
            }
            else
            {
                return null;
            }
        }
        public Task<string> GetUserPhone()
        {
            var user = mAuth.CurrentUser;

            if (user != null)
            {
                // User is signed in
                return Task.FromResult(user.PhoneNumber.ToString());
            }
            else
            {
                return null;
            }
        }

        public async Task<string> LoginWithEmailPasswordAsync(string email, string password)
        {
            IAuthResult res;

            try
            {
                res = await mAuth.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch (Exception ex)
            {
                return await Task.FromResult("false:" + ex.Message);
            }
            return null;
        }

        public  Task<bool> LoginWithPhoneAsync(string phone)
        {
            var activity = Forms.Context as Activity;

            PhoneAuthProvider.VerifyPhoneNumber(
                phone, 
                60, 
                TimeUnit.Seconds, 
                activity, new PhoneAuthCallBacks());

            return Task.FromResult(true);
        }

        public void Singout()
        {
            mAuth.SignOut();
        }

        private class PhoneAuthCallBacks : PhoneAuthProvider.OnVerificationStateChangedCallbacks
        {
            private PhoneAuthProvider.ForceResendingToken mResendToken;

            string mVerificationId;


            public async override void OnVerificationCompleted(PhoneAuthCredential credential)
            {
                // This callback will be invoked in two situations:
                // 1 - Instant verification. In some cases the phone number can be instantly
                //     verified without needing to send or enter a verification code.
                // 2 - Auto-retrieval. On some devices Google Play services can automatically
                //     detect the incoming verification SMS and perform verification without
                //     user action.

                if (credential != null)
                {
                    var user = await FirebaseAuth.Instance.SignInWithCredentialAsync(credential);

                    if(user.User.Uid != null)
                    {
                        UpdateUser(user.User.Uid);
                    }
                }

            }

            

            public override void OnVerificationFailed(FirebaseException e)
            {

                // This callback is invoked in an invalid request for verification is made,
                // for instance if the the phone number format is not valid.
                UpdateInfo("onVerificationFailed" +  e.Message);

                if (e.GetType() == typeof(FirebaseAuthInvalidCredentialsException))
                {
                    // Invalid request
                    // [START_EXCLUDE]

                    UpdateInfo("INVALID");

                    // [END_EXCLUDE]
                }
                else if (e.GetType() == typeof(FirebaseTooManyRequestsException)) {
                    // The SMS quota for the project has been exceeded
                    // [START_EXCLUDE]

                    UpdateInfo("TIME");

                    // [END_EXCLUDE]
                }
            }

            public override void OnCodeSent(string verificationId, PhoneAuthProvider.ForceResendingToken forceResendingToken)
            {
                base.OnCodeSent(verificationId, forceResendingToken);

                // The SMS verification code has been sent to the provided phone number, we
                // now need to ask the user to enter the code and then construct a credential
                // by combining the code with a verification ID.


                // Notify user !!
               

                // Save verification ID and resending token so we can use them later
                mVerificationId = verificationId;
                mResendToken = forceResendingToken;

                UpdateInfo("SENT");
            }

            private void UpdateInfo(string v)
            {
                MessagingCenter.Send<object, string>(this, "INFO", v);

            }

            private void UpdateUser(string uid)
            {
                MessagingCenter.Send<object, string>(this, "seccuss", uid);
            }



        }
    }
}