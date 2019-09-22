using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nasiye.Models
{
    public class TicketModel
    {
        [JsonProperty("Airline")]
        public string Airline { get; set; }

        [JsonProperty("Booked")]
        public long Booked { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Date")]
        public string Date { get; set; }

        [JsonProperty("Days")]
        public string Days { get; set; }

        [JsonProperty("From")]
        public string From { get; set; }

        [JsonProperty("Image")]
        public Uri Image { get; set; }

        [JsonProperty("Key")]
        public string Key { get; set; }

        [JsonProperty("Price")]
        public long Price { get; set; }

        [JsonProperty("Status")]
        public bool Status { get; set; }

        [JsonProperty("To")]
        public string To { get; set; }
    }
}
