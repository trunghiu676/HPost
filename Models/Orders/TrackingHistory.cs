using System.ComponentModel.DataAnnotations;

namespace App.Models.Orders
{
    public class TrackingHistory
    {
        [Key]
        public int TrackingId { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Location { get; set; }
        public string Note { get; set; }

        // Navigation Property
        public Order Order { get; set; }
    }

}