using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Models;
using App.Models.Blog;
using App.Models.UserManagement;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Quản lý database
namespace App.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbManageController(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }
        [TempData]
        public string StatusMessage { get; set; }
        //Xóa database
        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var success = await _context.Database.EnsureDeletedAsync();

            StatusMessage = success ? "Xoa thanh cong" : "Xoa that bai";
            return RedirectToAction(nameof(Index));
        }
        //Tạo, cập nhật database lên sql
        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _context.Database.MigrateAsync();

            StatusMessage = "Da tao database thanh cong";
            return RedirectToAction(nameof(Index));
        }
        //Tạo các role, tạo user admin có role Administrator, tạo danh mục lập trình nếu chưa có bất kì danh mục nào
        public async Task<IActionResult> SeedDataAsync()
        {
            // Lấy ra các trường từ model App.Data.RoleName
            var rolenames = typeof(RoleName).GetFields().ToList();
            foreach (var r in rolenames)
            {
                var rolename = (string)r.GetRawConstantValue(); // Lấy ra tên trường
                var rfound = await _roleManager.FindByNameAsync(rolename);
                if (rfound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(rolename));
                }
            }

            // Admin, pass = admin123, admin@example.com
            var useradmin = await _userManager.FindByEmailAsync("admin@example.com");
            if (useradmin == null)
            {
                useradmin = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(useradmin, "admin123"); // Tạo user admin với pass là admin123
                await _userManager.AddToRoleAsync(useradmin, RoleName.Administrator);
            }

            // Kiểm tra nếu không có bất kỳ danh mục nào trong cơ sở dữ liệu
            // var anyCategory = await _context.Categories.AnyAsync();
            // if (!anyCategory)
            // {
            //     var newCategory = new Category()
            //     {
            //         Title = "Lập trình",
            //         Slug = "lap-trinh", // Bạn có thể tự động sinh Slug theo Title nếu cần
            //         Description = "Chuyên mục về lập trình",
            //         ParentCategoryId = null // Không có danh mục cha
            //     };

            //     _context.Categories.Add(newCategory);
            //     await _context.SaveChangesAsync();
            // }
            // SeedPostCategory();

            // Seed thêm dữ liệu từ file SQL
            try
            {
                string sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DataAddress.sql");
                if (System.IO.File.Exists(sqlFilePath))
                {
                    string sqlContent = await System.IO.File.ReadAllTextAsync(sqlFilePath);
                    await _context.Database.ExecuteSqlRawAsync(sqlContent);
                    StatusMessage = "Đã seed database thành công từ file SQL.";
                }
                else
                {
                    StatusMessage = "Không tìm thấy file DataAddress.sql.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Có lỗi xảy ra khi chạy file SQL: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        //tạo data mẫu sử dụng pagake Bogus cho bảng category va post
        private void SeedPostCategory()
        {
            _context.Categories.RemoveRange(_context.Categories.Where(c => c.Description.Contains("[fakeData]"))); //xóa các category chứa fakeData sau đó mới phát sinh dữ liệu mẫu
            _context.Posts.RemoveRange(_context.Posts.Where(p => p.Content.Contains("[fakeData]")));

            _context.SaveChanges();

            var fakerCategory = new Faker<Category>();
            int cm = 1;
            fakerCategory.RuleFor(c => c.Title, fk => $"CM{cm++} " + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakerCategory.RuleFor(c => c.Description, fk => fk.Lorem.Sentences(5) + "[fakeData]");
            fakerCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());



            var cate1 = fakerCategory.Generate();
            var cate11 = fakerCategory.Generate();
            var cate12 = fakerCategory.Generate();
            var cate2 = fakerCategory.Generate();
            var cate21 = fakerCategory.Generate();
            var cate211 = fakerCategory.Generate();

            //thiết lập mối quan hệ cha con
            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;
            cate21.ParentCategory = cate2;
            cate211.ParentCategory = cate21;

            var categories = new Category[] { cate1, cate2, cate12, cate11, cate21, cate211 };
            _context.Categories.AddRange(categories);



            // phát sinh POST
            var rCateIndex = new Random();
            int bv = 1;

            var user = _userManager.GetUserAsync(this.User).Result;
            var fakerPost = new Faker<Post>();
            fakerPost.RuleFor(p => p.AuthorId, f => user.Id);
            fakerPost.RuleFor(p => p.Content, f => f.Lorem.Paragraphs(7) + "[fakeData]");
            fakerPost.RuleFor(p => p.DateCreated, f => f.Date.Between(new DateTime(2021, 1, 1), new DateTime(2021, 7, 1)));
            fakerPost.RuleFor(p => p.Description, f => f.Lorem.Sentences(3));
            fakerPost.RuleFor(p => p.Published, f => true);
            fakerPost.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            fakerPost.RuleFor(p => p.Title, f => $"Bài {bv++} " + f.Lorem.Sentence(3, 4).Trim('.'));

            List<Post> posts = new List<Post>();
            List<PostCategory> post_categories = new List<PostCategory>();


            for (int i = 0; i < 40; i++)
            {
                var post = fakerPost.Generate();
                post.DateUpdated = post.DateCreated;
                posts.Add(post);
                post_categories.Add(new PostCategory()
                {
                    Post = post,
                    Category = categories[rCateIndex.Next(5)]
                });
            }

            _context.AddRange(posts);
            _context.AddRange(post_categories);
            // END POST



            _context.SaveChanges();
        }

    }

}