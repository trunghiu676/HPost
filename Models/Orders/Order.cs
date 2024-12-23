using System.ComponentModel.DataAnnotations;
using App.Models.UserManagement;

namespace App.Models.Orders
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string SenderAddress { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientAddress { get; set; }
        public int ServiceTypeId { get; set; }
        public double Weight { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } // Pending, In Transit, Delivered, Cancelled
        public DateTime CreatedAt { get; set; }
        public int? PromotionId { get; set; }

        // Navigation Properties
        public AppUser User { get; set; }
        public ICollection<TrackingHistory> TrackingHistories { get; set; }
        public Payment Payment { get; set; }
    }

}