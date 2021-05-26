using System.Collections.Generic;

namespace DGN.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string BranchName { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location(int branchId, string name, string desc, double latitude, double longitude)
        {
            this.Id = branchId;
            this.BranchName = name;
            this.Description = desc;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }

    public class LocationsList
    {
        public IEnumerable<Location> LocationList { get; set; }
    }
}