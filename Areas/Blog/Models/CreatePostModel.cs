using System.ComponentModel.DataAnnotations;
using App.Models.Blog;

namespace AppMvc.Areas.Blog.Models
{
    public class CreatePostModel : Post
    {
        [Required(ErrorMessage = "Phải có tiêu đề bài viết")]
        [Display(Name = "Tiêu đề")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
        public string Title { get; set; }

        [Display(Name = "Mô tả ngắn")]
        public string Description { get; set; }

        [Display(Name = "Chuỗi định danh (url)", Prompt = "Nhập hoặc để trống tự phát sinh theo Title")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        public string Slug { get; set; }

        [Display(Name = "Nội dung")]
        public string Content { get; set; }

        [Display(Name = "Xuất bản")]
        public bool Published { get; set; }

        [Display(Name = "Chuyên mục")]
        public int[] CategoryIDs { get; set; }
    }
}