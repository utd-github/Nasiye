using System;
using System.Collections.Generic;
using System.Text;

namespace Nasiye.Models
{
    public class UserModel
    {
        public string Key { get; set; }

        public string Email { get; set; }

        public Uri Image { get; set; }


        public string Jdate { get; set; }

        public bool PhoneVerified { get; set; }

        public string Name { get; set; }


        public string Phone { get; set; }


        public long Rating { get; set; }


        public string Status { get; set; }

        public string TripKey { get; set; }

        public string City { get; internal set; }

    }
}
