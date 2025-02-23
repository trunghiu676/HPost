//loại dịch vụ vận chuyển - trong nước, quốc tế
using System.ComponentModel.DataAnnotations;

namespace App.Models.Services
{
    public class ServiceType
    {
        //Phát sinh controller và view
        //dotnet aspnet-codegenerator controller -name ServiceTypeController -m App.Models.Services.ServiceType -dc App.Models.AppDbContext -udl -outDir Areas/Service/Controllers -namespace App.Areas.Service.Controllers
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Phải nhập tên loại dịch vụ")]
        [Display(Name = "Loại dịch vụ")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phải nhập mô tả loại dịch vụ")]
        [Display(Name = "Mô tả dịch vụ")]
        public string Description { get; set; }
        //Danh sach cac dich vu
        public ICollection<Service>? Services { get; set; }
    }
}
