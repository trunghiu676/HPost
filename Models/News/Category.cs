using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Blog
{
  //bảng danh mục 
  [Table("Category")]
  public class Category
  {

    [Key]
    public int Id { get; set; }

    // Từ khóa category
    [Required(ErrorMessage = "Phải có từ khóa trang")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
    [Display(Name = "Từ khóa trang")]
    public string Keyword { get; set; }

    // Tiều đề Category
    [Required(ErrorMessage = "Phải có tên thể loại")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
    [Display(Name = "Tên thể loại")]
    public string Title { get; set; }

    // Mô tả ngắn Category
    [Required(ErrorMessage = "Phải có mô tả ngắn")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
    [Display(Name = "Mô tả ngắn")]
    public string Description { get; set; }

    // Nội dung, thông tin chi tiết về Category
    [DataType(DataType.Text)]
    [Display(Name = "Nội dung danh mục")]
    public string Content { get; set; }

    //chuỗi Url
    [Required(ErrorMessage = "Phải tạo url")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
    [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
    [Display(Name = "Url hiện thị")]
    public string Slug { set; get; }

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


    //Các trường seo
    // Tiều đề Category
    [Required(ErrorMessage = "Phải có tiêu đề seo")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
    [Display(Name = "Tiêu đề SEO (<title>)")]
    public string SeoTitle { get; set; }

    // Mô tả ngắn Category
    [Required(ErrorMessage = "Phải có mô tả")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
    [Display(Name = "Mô Tả Trang ( <meta Description> )")]
    public string SeoDescription { get; set; }

    [Display(Name = "Trạng thái (Ẩn / Hiện)")]
    public bool Status { set; get; }

    [Display(Name = "Index, Follow")]
    public bool IndexFollow { set; get; }


    // Các Category con
    public ICollection<Category>? CategoryChildren { get; set; }

    // Category cha (FKey)
    [Display(Name = "Danh mục cha")]
    public int? ParentCategoryId { get; set; }

    [ForeignKey("ParentCategoryId")]
    [Display(Name = "Danh mục cha")]
    public Category? ParentCategory { set; get; }

    //Những category con thuộc category
    public void ChildCategoryIDs(ICollection<Category> childcates, List<int> lists)
    {
      if (childcates == null)
        childcates = this.CategoryChildren;

      foreach (Category category in childcates)
      {
        lists.Add(category.Id);
        ChildCategoryIDs(category.CategoryChildren, lists);

      }
    }

    //code cho chức năng breadcrumb
    public List<Category> ListParents()
    {
      List<Category> li = new List<Category>();
      var parent = this.ParentCategory;
      while (parent != null)
      {
        li.Add(parent);
        parent = parent.ParentCategory;

      }
      li.Reverse();//đảo ngược danh sách list
      return li;
    }
  }
}