using System;
using System.Collections.Generic;
using System.Text;

namespace Nasiye.Models
{
    class RoomModel
    {
        public string Key { get; set; }
        public Uri Image { get; set; }
        public string Type { get; set; }
        public string Hotel { get; set; }
        public string HotelKey { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Amount { get; set; }
        public string Info { get; set; }
        public bool Status { get; set; }
    }
}
