using System;
using System.Collections.Generic;
using System.Text;

namespace Nasiye.Models
{
    class DriverModel
    {
        public string Key { get; set; }

        public string Email { get; set; }


        public string Image { get; set; }


        public string Jdate { get; set; }


        public Location Location { get; set; }


        public string Name { get; set; }


        public string Phone { get; set; }


        public long Rating { get; set; }

        public Vehicle Vehicle { get; set; }

        public string Status { get; set; }
    }

    public partial class Location
    {

        public double Lat { get; set; }


        public double Lng { get; set; }
    }

    public partial class Vehicle
    {
        public Vehicle()
        {

        }


        public Vehicle(string key, string imageUri, string model, string plate, string type, string createdAt)
        {
            Key = key;
            ImageUri = imageUri;
            Model = model;
            Plate = plate;
            Type = type;
            CreatedAt = createdAt;
        }

        public string Key { get; set; }
        public string ImageUri { get; set; }
        public string Model { get; set; }
        public string Plate { get; set; }
        public string Type { get; set; }
        public string CreatedAt { get; set; }
    }
}