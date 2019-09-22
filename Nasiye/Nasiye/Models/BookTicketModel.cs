using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nasiye.Models
{
    class BookTicketModel
    {
        public string Key { get; set; }

        public string UserUID { get; set; }

        public TicketModel Ticket { get; set; }

        public string Amount { get; set; }

        public string Date { get; set; }

        public string Quantity { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }


    }
}
