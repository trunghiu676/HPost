using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Models.Orders;
using App.Models.Provinces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace App.Models.UserManagement
{
    public class AppUser : IdentityUser
    {
        [Column(TypeName = "nvarchar")]
        [StringLength(250)]
        public string? FullName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(400)]
        public string? AddressDetail { get; set; } // cho phép giá trị null

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        //Các trường địa chỉ
        [ForeignKey(nameof(Province))]
        public string? ProvinceCode { get; set; }
        public Province? Province { get; set; }

        [ForeignKey(nameof(District))]
        public string? DistrictCode { get; set; }
        public District? District { get; set; }

        [ForeignKey(nameof(Ward))]
        public string? WardCode { get; set; }
        public Ward? Ward { get; set; }

        // Các trường khác
        // public ICollection<Order> Orders { get; set; }

    }

}