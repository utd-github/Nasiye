using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using GoogleGson;
using Java.Util;
using Nasiye.Droid.Services;
using Nasiye.Models;
using Nasiye.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

[assembly: Dependency(typeof(AFirebaseDatabaseService))]
namespace Nasiye.Droid.Services
{
    class AFirebaseDatabaseService : IFirebaseDatabaseService
    {
        private const string FullUrl = "gs://nasiyebooking.appspot.com/";

        Dictionary<string, DatabaseReference> DatabaseReferences;

        Dictionary<string, IValueEventListener> ValueEventListeners;

        Dictionary<string, IValueEventListener> HValueEventListeners;
        Dictionary<string, IValueEventListener> PValueEventListeners;
        Dictionary<string, IValueEventListener> CPValueEventListeners;
        Dictionary<string, IValueEventListener> DValueEventListeners;
        Dictionary<string, IValueEventListener> RValueEventListeners;
        Dictionary<string, IValueEventListener> TValueEventListeners;
        Dictionary<string, IValueEventListener> STValueEventListeners;
        Dictionary<string, IValueEventListener> TKValueEventListeners;
        Dictionary<string, IValueEventListener> ROValueEventListeners;

        Dictionary<string, IValueEventListener> BTValueEventListeners;
        Dictionary<string, IValueEventListener> BHValueEventListeners;

        FirebaseAuth mAuth = FirebaseAuth.GetInstance(MainActivity.app);

        FirebaseStorage storage = FirebaseStorage.Instance;

        StorageReference storageRef;

        public AFirebaseDatabaseService()
        {
            DatabaseReferences = new Dictionary<string, DatabaseReference>();

            ValueEventListeners = new Dictionary<string, IValueEventListener>();
            HValueEventListeners = new Dictionary<string, IValueEventListener>();
            DValueEventListeners = new Dictionary<string, IValueEventListener>();
            RValueEventListeners = new Dictionary<string, IValueEventListener>();

            TValueEventListeners = new Dictionary<string, IValueEventListener>();
            STValueEventListeners = new Dictionary<string, IValueEventListener>();

            PValueEventListeners = new Dictionary<string, IValueEventListener>();
            CPValueEventListeners = new Dictionary<string, IValueEventListener>();

            ROValueEventListeners = new Dictionary<string, IValueEventListener>();
            TKValueEventListeners = new Dictionary<string, IValueEventListener>();

            BTValueEventListeners = new Dictionary<string, IValueEventListener>();
            BHValueEventListeners = new Dictionary<string, IValueEventListener>();

            storageRef = storage.GetReferenceFromUrl(FullUrl);
        }

        private DatabaseReference GetDatabaseReference(string nodeKey)
        {
            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                return DatabaseReferences[nodeKey];
            }
            else
            {
                DatabaseReference dr = FirebaseDatabase.Instance.GetReference(nodeKey);
                DatabaseReferences[nodeKey] = dr;
                return dr;
            }
        }


        public void GetProfile<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);


            if (dr != null)
            {
                var user = mAuth.CurrentUser;

                if (user != null)
                {
                    PValueEventListener<T> listener = new PValueEventListener<T>(action);

                    dr.AddValueEventListener(listener);
                    PValueEventListeners.Add(nodeKey, listener);
                }
            }
        }

        public void GetCheckProfile<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);


            if (dr != null)
            {
                var user = mAuth.CurrentUser;

                if (user != null)
                {
                    CPValueEventListener<T> listener = new CPValueEventListener<T>(action);

                    dr.AddValueEventListener(listener);
                    CPValueEventListeners.Add(nodeKey, listener);
                }
            }
        }

        public void GetHotels<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
           
            if (dr != null)
            {
                HValueEventListener<T> listener = new HValueEventListener<T>(action);
                dr.AddValueEventListener(listener);

                HValueEventListeners.Add(nodeKey, listener);
            }
        }

        public void GetTickets<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
          
            if (dr != null)
            {
                TKValueEventListener<T> listener = new TKValueEventListener<T>(action);

                dr.OrderByChild("Status").EqualTo(true).AddValueEventListener(listener);

                TKValueEventListeners.Add(nodeKey, listener);
            }
        }

        public void GetRooms<T>(string nodeKey, string key, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
            
            if (dr != null)
            {
                ROValueEventListener<T> listener = new ROValueEventListener<T>(action);
                dr.OrderByChild("HotelKey").EqualTo(key).AddValueEventListener(listener);

                ROValueEventListeners.Add(nodeKey, listener);
            }
        }

        public void GetDrivers<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
           
            if (dr != null)
            {
                DValueEventListener<T> listener = new DValueEventListener<T>(action);
                dr.OrderByChild("Status")
                    .EqualTo("Online")
                    .AddValueEventListener(listener);

               

                DValueEventListeners.Add(nodeKey, listener);
            }
        }
 
        public void GetRequests<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
            if (dr != null)
            {
                RValueEventListener<T> listener = new RValueEventListener<T>(action);
                dr.AddValueEventListener(listener);
               
                    RValueEventListeners.Add(nodeKey, listener);
               
            }
        }

        public void GetTrips<T>(string nodeKey, Action<T> OnValueEvent = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
           
            if (dr != null)
            {
                var user = mAuth.CurrentUser;

                if (user != null)
                {
                    TValueEventListener<T> listener = new TValueEventListener<T>(OnValueEvent);

                    dr.AddValueEventListener(listener);

                    TValueEventListeners.Add(nodeKey, listener);
                }
            }
        }

        public void GetSavedTrips<T>(string nodeKey, Action<T> OnValueEvent = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
           
            if (dr != null)
            {
                var user = mAuth.CurrentUser;

                if (user != null)
                {
                    TValueEventListener<T> listener = new TValueEventListener<T>(OnValueEvent);

                    dr.OrderByChild("User/Key").EqualTo(user.Uid).AddValueEventListener(listener);

                    TValueEventListeners.Add(nodeKey, listener);
                }
            }
        }


        public void GetBookedTickets<T>(string nodeKey, Action<T> OnValueEvent = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
            string uid = mAuth.CurrentUser.Uid;

            if (dr != null)
            {
                var user = mAuth.CurrentUser;

                if (user != null)
                {
                    BTValueEventListener<T> listener = new BTValueEventListener<T>(OnValueEvent);

                    dr.OrderByChild("UserUID").EqualTo(uid).AddValueEventListener(listener);

                    BTValueEventListeners.Add(nodeKey, listener);
                }
            }
        }

        public void GetBookedHotels<T>(string nodeKey, Action<T> OnValueEvent = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
            string uid = mAuth.CurrentUser.Uid;

            if (dr != null)
            {
                var user = mAuth.CurrentUser;

                if (user != null)
                {
                    BHValueEventListener<T> listener = new BHValueEventListener<T>(OnValueEvent);

                    dr.OrderByChild("UserUID").EqualTo(uid).AddValueEventListener(listener);

                    BHValueEventListeners.Add(nodeKey, listener);
                }
            }
        }


        public void CreateProfile(string nodeKey, object obj, Action onSuccess = null, Action<string> onError = null)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference(nodeKey);

            string uid = mAuth.CurrentUser.Uid;

            if (dr != null)
            {
                string objJsonString = JsonConvert.SerializeObject(obj);

                Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

                HashMap dataHashMap = new HashMap();

                Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

                dataHashMap = jsonObj.JavaCast<HashMap>();

                dr.Child(uid).SetValue(dataHashMap);
                dr.Child(uid).Child("Key").SetValue(uid);

            }

        }

        public void SetValue(string nodeKey, object obj, Action onSuccess = null, Action<string> onError = null)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference(nodeKey);
            if (dr != null)
            {
                string objJsonString = JsonConvert.SerializeObject(obj);

                Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

                HashMap dataHashMap = new HashMap();

                Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

                dataHashMap = jsonObj.JavaCast<HashMap>();

                dr.SetValue(dataHashMap);
            }

        }

        public void SetRequest(string nodeKey, object obj, Action onSuccess = null, Action<string> onError = null)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("requests");
            var req = dr.Child(nodeKey);

            if (dr != null)
            {
                if (obj != null)
                {
                    string objJsonString = JsonConvert.SerializeObject(obj);

                    Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

                    HashMap dataHashMap = new HashMap();

                    Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

                    dataHashMap = jsonObj.JavaCast<HashMap>();


                    req.SetValue(dataHashMap);
                }
                else
                {
                    req.Child("Status").SetValue("Canceled");
                }

            }

        }


        public void SubmitFeedback(object feedback)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("feedbacks");

            if (dr != null)
            {
                string objJsonString = JsonConvert.SerializeObject(feedback);

                Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

                HashMap dataHashMap = new HashMap();

                Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

                dataHashMap = jsonObj.JavaCast<HashMap>();

                var Key = dr.Push().Key;

                var feedbackref = dr.Child(Key);

                feedbackref.SetValue(dataHashMap);
            }

        }

        public void UpdateUserPhone(string uid, string phone)
        {
            DatabaseReference dr = GetDatabaseReference("users");

            if (dr != null)
            {
                var userref = dr.Child(uid);

                userref.Child("Phone").SetValue(phone);
                userref.Child("PhoneVerified").SetValue(true);
            }
        }

        public void UpdateUserName(string uid, string name)
        {
            DatabaseReference dr = GetDatabaseReference("users");

            if (dr != null)
            {
                var userref = dr.Child(uid);

                userref.Child("Name").SetValue(name);
            }
        }


        public void CancelTrip(string key)
        {
            DatabaseReference dr = GetDatabaseReference("trips");

            if (dr != null)
            {
                var tripref = dr.Child(key);

                tripref.Child("Status").SetValue("Canceled");
            }
        }



        public void SetBooking(string nodeKey, object obj, Action onSuccess = null, Action<string> onError = null)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference(nodeKey);
            if (dr != null)
            {
                string objJsonString = JsonConvert.SerializeObject(obj);

                Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

                HashMap dataHashMap = new HashMap();

                Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

                dataHashMap = jsonObj.JavaCast<HashMap>();

                var key = dr.Push().Key;

                dr.Child(key).SetValue(dataHashMap);
                dr.Child(key).Child("Key").SetValue(key);
            }

        }

        public void UpdateUserImage(string uid, string imagelocation)
        {
            DatabaseReference dr = GetDatabaseReference("users");

            if (dr != null)
            {
                var userref = dr.Child(uid);
                userref.Child("Image").SetValue(imagelocation);
            }
        }

        public void SubmitTripRating(string key, double rating, string payment)
        {
            // Get Ref
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("trips");


            if (dr != null)
            {
                var trip = dr.Child(key);
                var user = trip.Child("Driver");
                var paymentref = trip.Child("Payment");

                paymentref.SetValue(payment);
                user.Child("Rating").Child("Driver").SetValue(rating);
            }
        }


        // Remove listeners and clear dictionary

        public void RemoveGetHotels(string nodeKey)
        {
            DatabaseReference dr = DatabaseReferences[nodeKey];

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                if (dr != null)
                {
                    dr.RemoveEventListener(HValueEventListeners[nodeKey]);
                    if (HValueEventListeners.ContainsKey(nodeKey))
                    {
                        HValueEventListeners.Remove(nodeKey);
                    }
                }
            }

        }

        public void RemoveGetProfile(string nodeKey)
        {
            DatabaseReference dr = DatabaseReferences[nodeKey];

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                if (dr != null)
                {
                    dr.RemoveEventListener(PValueEventListeners[nodeKey]);
                    if (PValueEventListeners.ContainsKey(nodeKey))
                    {
                        PValueEventListeners.Remove(nodeKey);
                    }
                }
            }
        }

        public void RemoveGetCheckProfile(string nodeKey)
        {
            DatabaseReference dr = DatabaseReferences[nodeKey];

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                if (dr != null)
                {
                    dr.RemoveEventListener(CPValueEventListeners[nodeKey]);
                    if (CPValueEventListeners.ContainsKey(nodeKey))
                    {
                        CPValueEventListeners.Remove(nodeKey);
                    }
                }
            }
        }

        public void RemoveGetDrivers(string nodeKey)
        {
            DatabaseReference dr = DatabaseReferences[nodeKey];

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                if (dr != null)
                {
                    dr.RemoveEventListener(DValueEventListeners[nodeKey]);

                    if (DValueEventListeners.ContainsKey(nodeKey))
                    {
                        DValueEventListeners.Remove(nodeKey);
                    }
                }
            }

        }

        public void RemoveGetRequests(string nodeKey)
        {


            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(RValueEventListeners[nodeKey]);
                   
                    if (RValueEventListeners.ContainsKey(nodeKey))
                    {
                        RValueEventListeners.Remove(nodeKey);

                    }
                }
            }

        }

        public void RemoveGetTrips(string nodeKey)
        {

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(TValueEventListeners[nodeKey]);
                    if (TValueEventListeners.ContainsKey(nodeKey))
                    {
                        TValueEventListeners.Remove(nodeKey);

                    }
                }
            }
        }

        public void RemoveGetSavedTrips(string nodeKey)
        {

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(TValueEventListeners[nodeKey]);
                    if (TValueEventListeners.ContainsKey(nodeKey))
                    {
                        TValueEventListeners.Remove(nodeKey);

                    }
                }
            }
        }

        public void RemoveGetTickets(string nodeKey)
        {

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(TKValueEventListeners[nodeKey]);
                    
                    if (TKValueEventListeners.ContainsKey(nodeKey))
                    {
                        TKValueEventListeners.Remove(nodeKey);
                    }
                }
            }
        }

        public void RemoveGetRooms(string nodeKey = "rooms")
        {

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(ROValueEventListeners[nodeKey]);

                    if (ROValueEventListeners.ContainsKey(nodeKey))
                    {
                        ROValueEventListeners.Remove(nodeKey);
                    }
                }
            }
        }


        public void RemoveGetBookedHotels(string nodeKey)
        {

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(BHValueEventListeners[nodeKey]);
                    if (BHValueEventListeners.ContainsKey(nodeKey))
                    {
                        BHValueEventListeners.Remove(nodeKey);

                    }
                }
            }
        }

        public void RemoveGetBookedTickets(string nodeKey)
        {

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(BTValueEventListeners[nodeKey]);

                    if (BTValueEventListeners.ContainsKey(nodeKey))
                    {
                        BTValueEventListeners.Remove(nodeKey);
                    }
                }
            }
        }


        ////
        /// Upload Image Image
        /// 
        public async Task<string> UploadFIle()
        {
            var result = "";
            int timeWait = 250;
            int countWait = 0;
            int maxCountWait = 20;


            try
            {
                var activity = Forms.Context as Activity;

                PickFileActivity.OnFinishAction = async (path) =>
                {
                    result = await SaveFileToStorage(path);
                };

                activity?.StartActivity(new Intent(Forms.Context, typeof(PickFileActivity)));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("---> Error UploadFiles " + ex.Message);
                countWait = maxCountWait;
            }

            do
            {
                countWait++;
                await System.Threading.Tasks.Task.Delay(timeWait);
            }
            while (result == "" || countWait < maxCountWait);

            return result;
        }

        /// <summary>
        /// Get Url to external(firebase storage) file 
        /// </summary>
        /// <param name="filename">name file</param>
        /// <returns>Url to external file</returns>
        public async Task<string> GetFileUrl(string filename)
        {
            try
            {
                var storage = FirebaseStorage.Instance;

                var spaceRef = storageRef.Child($"userimages/{filename}");

                var url = await spaceRef.DownloadUrl;

                filename = url.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("---> Error UploadFiles " + ex.Message);
            }

            return filename;
        }


        #region Helpers

        /// <summary>
        ///  Save File To Firebase Storage and return url to file
        /// </summary>
        /// <param name="localPath">url to local file</param>
        /// <returns>url to external file</returns>
        private async Task<string> SaveFileToStorage(string localPath)
        {
            try
            {
                

                var bytes = File.ReadAllBytes(localPath);

                var metadata = new StorageMetadata.Builder()
                    .SetContentType("image/jpeg")
                    .Build();

                var child = storageRef.Child("userimages/" + Path.GetFileName(localPath));

                await child.PutBytes(bytes, metadata);

                localPath = Path.GetFileName(localPath);


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("---> Error SaveFileToStorage " + ex.Message);
            }

            return localPath;
        }

        #endregion

    }


    public class ValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public ValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class DValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public DValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class CPValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public CPValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class PValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public PValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class HValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public HValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class TValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public TValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class ROValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public ROValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class TKValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public TKValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class RValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public RValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }


    public class BTValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public BTValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class BHValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public BHValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);

                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

}