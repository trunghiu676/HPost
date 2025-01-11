//dịch vụ vận chuyển: nhanh, châm, vv
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Services
{
    public class Service
    {
        //Phát sinh controller và view
        //dotnet aspnet-codegenerator controller -name ServiceController -m App.Models.Services.Service -dc App.Models.AppDbContext -udl -outDir Areas/Service/Controllers -namespace App.Areas.Service.Controllers
        public int Id { get; set; }
        [Required(ErrorMessage = "Phải nhập tên dịch vụ")]
        [Display(Name = "Tên dịch vụ")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phải nhập phần tổng quan")]
        [Display(Name = "Tổng quan")]
        public string Overview { get; set; }

        [Required(ErrorMessage = "Nội dung")]
        [Display(Name = "Nội dung không được để trống")]
        public string Content { get; set; }

        //Đường dẫn URL
        [Required(ErrorMessage = "Phải có URL")]
        [Display(Name = "Chuỗi định danh (url)")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        public string? Slug { set; get; }

        //hình đại diện
        [Display(Name = "Hình ảnh đại diện")]
        [StringLength(255, ErrorMessage = "{0} không được dài quá {1} ký tự")]
        public string? Avatar { get; set; }

        [Display(Name = "Loại dịch vụ")]
        [Required(ErrorMessage = "Phải chọn loại dịch vụ")]
        [ForeignKey(nameof(ServiceType))]
        public int? ServiceTypeId { get; set; }
        public ServiceType? ServiceType { get; set; }
    }
}