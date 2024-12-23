using System.ComponentModel.DataAnnotations.Schema;

//tạo bảng này cho mqh nhiều nhiều giữa post và category
namespace App.Models.Blog {
    [Table("PostCategory")]
    public class PostCategory
    {
        public int PostID {set; get;}

        public int CategoryID {set; get;}
        

        [ForeignKey("PostID")]
        public Post Post {set; get;}

        [ForeignKey("CategoryID")]
        public Category Category {set; get;}
    }
}