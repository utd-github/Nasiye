using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nasiye.Models;

namespace Nasiye.Services
{
    public interface IFirebaseDatabaseService
    {
        void SetValue(string nodeKey, object obj, Action onSuccess = null, Action<string> onError = null);
        void SetBooking(string nodeKey, object obj, Action onSuccess = null, Action<string> onError = null);
        void SetRequest(string nodeKey, object obj, Action onSuccess = null, Action<string> onError = null);
        void CreateProfile(string nodeKey, object obj, Action onSuccess = null, Action<string> onError = null);

        void SubmitFeedback(object feedback);
        void UpdateUserName(string uid, string name);
        void UpdateUserPhone(string uid, string phone);
        void UpdateUserImage(string uid, string imagelocation);
        void SubmitTripRating(string key, double rating, string payment);

        void CancelTrip(string key);


        void GetProfile<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetCheckProfile<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetHotels<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetTickets<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetRooms<T>(string nodeKey, string key, Action<T> OnValueEvent = null);
        void GetDrivers<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetRequests<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetTrips<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetSavedTrips<T>(string nodeKey, Action<T> OnValueEvent = null);

        void GetBookedTickets<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetBookedHotels<T>(string nodeKey, Action<T> OnValueEvent = null);



        // Remove Listeners
        void RemoveGetHotels(string nodeKey);
        void RemoveGetProfile(string nodeKey);
        void RemoveGetCheckProfile(string nodeKey);
        void RemoveGetDrivers(string nodeKey);
        void RemoveGetRequests(string nodeKey);
        void RemoveGetTrips(string nodeKey);
        void RemoveGetSavedTrips(string nodeKey);
        void RemoveGetTickets(string nodeKey);
        void RemoveGetRooms(string nodeKey);

        void RemoveGetBookedTickets(string nodeKey);
        void RemoveGetBookedHotels(string nodeKey);



        ///
        /// 
        ///UPload User Image
        ///

        Task<string> UploadFIle();
        Task<string> GetFileUrl(string filename);
    }
}
