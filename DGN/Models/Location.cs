using System.Collections.Generic;

namespace DGN.Models
{
    public class Location
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location(int branchId, double latitude, double longitude)
        {
            this.Id = branchId;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }

    public class LocationsList
    {
        public IEnumerable<Location> LocationList { get; set; }
    }
}