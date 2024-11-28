using System.ComponentModel.DataAnnotations;

namespace App.Models.Locations
{
    public class BuuCuc
    {
        [Key]
        public int BuuCucId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Province { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

}