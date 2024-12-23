using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Models.UserManagement;

namespace App.Models.Blog
{
    //bảng bài viết, tin tức
    [Table("Post")]
    public class Post
    {
        [Key]
        public int PostId { set; get; }

        [Required(ErrorMessage = "Phải có tiêu đề bài viết")]
        [Display(Name = "Tiêu đề")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
        public string Title { set; get; }


        [Required(ErrorMessage = "Phải có mô tả ngắn")]
        [Display(Name = "Mô tả ngắn")]
        public string? Description { set; get; }


        [Required(ErrorMessage = "Phải có URL")]
        [Display(Name = "Chuỗi định danh (url)")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        public string? Slug { set; get; }

        [Display(Name = "Nội dung")]
        public string? Content { set; get; }



        //hình đại diện
        [Display(Name = "Hình ảnh đại diện")]
        [StringLength(255, ErrorMessage = "{0} không được dài quá {1} ký tự")]
        public string? Avatar { get; set; }

        [NotMapped]
        // [Required(ErrorMessage = "Phải chọn file upload")]
        [DataType(DataType.Upload)]
        // [FileExtensions(Extensions = "png,jpg,jpeg,gif", ErrorMessage = "Chỉ chấp nhận các định dạng: png, jpg, jpeg, gif")]
        [Display(Name = "Chọn file upload")]
        public IFormFile? FileUpload { get; set; }

        //seo
        // Từ khóa bài viết
        [Required(ErrorMessage = "Phải có từ khóa trang")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [Display(Name = "Từ khóa trang")]
        public string Keyword { get; set; }

        // Tiều đề 
        [Required(ErrorMessage = "Phải có tiêu đề seo")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [Display(Name = "Tiêu đề SEO (<title>)")]
        public string SeoTitle { get; set; }

        // Mô tả ngắn 
        [Required(ErrorMessage = "Phải có mô tả")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [Display(Name = "Mô Tả Trang ( <meta Description> )")]
        public string SeoDescription { get; set; }


        [Display(Name = "Xuất bản (Ẩn/Hiện)")]
        public bool Published { set; get; }

        [Display(Name = "Index, Follow")]
        public bool IndexFollow { set; get; }

        public List<PostCategory>? PostCategories { get; set; }

        // [Required]
        [Display(Name = "Tác giả")]
        public string? AuthorId { set; get; } = "abc";
        [ForeignKey("AuthorId")]
        [Display(Name = "Tác giả")]
        public AppUser? Author { set; get; }



        [Display(Name = "Ngày tạo")]
        public DateTime? DateCreated { set; get; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? DateUpdated { set; get; }
    }
}