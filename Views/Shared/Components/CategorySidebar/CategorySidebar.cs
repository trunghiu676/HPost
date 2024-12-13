
using System.Collections.Generic;
using App.Models.Blog;
using Microsoft.AspNetCore.Mvc;

namespace App.Components
{
    [ViewComponent]
    public class CategorySidebar : ViewComponent {

        public class CategorySidebarData 
        {
            //Chứa các danh mục cần render
            public List<Category> Categories { get; set; }
            //Phân cấp category - cha con cháu
            public int level { get; set; }
            //in đậm category nào khi đang truy cập
            public string categoryslug { get; set;}

        }

        public IViewComponentResult Invoke(CategorySidebarData data)
        {
            return View(data); //Default.cshtml
        }

    }
}