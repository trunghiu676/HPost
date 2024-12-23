using System.ComponentModel.DataAnnotations;

namespace App.Models.Orders
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } // COD, VNPay, Credit Card
        public string PaymentStatus { get; set; } // Pending, Completed, Cancelled
        public DateTime? PaidAt { get; set; }

        // Navigation Property
        public Order Order { get; set; }
    }

}