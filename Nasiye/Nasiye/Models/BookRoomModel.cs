using System;
using System.Collections.Generic;
using System.Text;

namespace Nasiye.Models
{
    class BookRoomModel
    {

        // User information
        public string Key { get; set; }
        public RoomModel Room { get; set; }

        public string Amount { get; set; }


        public HotelModel Hotel { get; set; }
        public string UserUID { get; set; }

        public string Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Status { get; set; }
    }
}
