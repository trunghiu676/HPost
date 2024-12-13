using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Models.Blog;
using Microsoft.AspNetCore.Authorization;
using App.Data;
using System.IO;

namespace AppMvc.Net.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("admin/blog/category/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator)]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CategoryController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Blog/Category
        public async Task<IActionResult> Index()
        {
            //qr lấy ra các category cha, trong cha có tập hợp các danh mục con
            var qr = (from c in _context.Categories select c)
                     .Include(c => c.ParentCategory)
                     .Include(c => c.CategoryChildren);
            //Lấy ra danh mục có ParentCategory(danh mục cha) là null, trong đó đã bao gồm các danh mục con năm trong 
            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategory == null)
                             .ToList();

            return View(categories);
        }

        // GET: Blog/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //tạo ra select list chọn danh mục cha và không có danh mục cha
        private void CreateSelectItems(List<Category> source, List<Category> des, int level)
        {
            //Nối danh mục với ký tự, ...
            string prefix = string.Concat(Enumerable.Repeat("|--", level));
            foreach (var category in source)
            {
                // category.Title = prefix + " " + category.Title;
                des.Add(new Category()
                {
                    Id = category.Id,
                    Title = prefix + " " + category.Title
                });
                if (category.CategoryChildren?.Count > 0)
                {
                    CreateSelectItems(category.CategoryChildren.ToList(), des, level + 1);
                }
            }
        }
        // GET: Blog/Category/Create
        public async Task<IActionResult> CreateAsync()
        {
            var qr = (from c in _context.Categories select c)
                     .Include(c => c.ParentCategory)
                     .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategory == null)
                             .ToList();
            categories.Insert(0, new Category()
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });
            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);
            var selectList = new SelectList(items, "Id", "Title");


            ViewData["ParentCategoryId"] = selectList;
            return View();
        }

        // POST: Blog/Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,ParentCategoryId,FileUpload")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentCategoryId == -1)
                {
                    category.ParentCategoryId = null;
                }

                // Lưu category trước để lấy Id
                _context.Add(category);
                await _context.SaveChangesAsync();

                if (category.FileUpload != null)
                {
                    // Sử dụng category.Id sau khi lưu để tạo tên file
                    var fileName = category.FileUpload.FileName + "-" + category.Id;

                    // Tạo đường dẫn
                    var uploadsFolder = Path.Combine("Uploads", "Blogs");
                    Directory.CreateDirectory(uploadsFolder); // Đảm bảo thư mục tồn tại
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Tạo file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await category.FileUpload.CopyToAsync(fileStream);
                    }

                    // Cập nhật tên file vào thuộc tính Avatar
                    category.Avatar = fileName;

                    // Cập nhật lại thông tin category
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Lấy lại dữ liệu select list
            await PrepareSelectListAsync();
            return View(category);
        }





        // GET: Blog/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await PrepareSelectListAsync();

            return View(category);
        }


        // POST: Blog/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Slug,ParentCategoryId")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            bool canUpdate = true;

            if (category.ParentCategoryId == category.Id)
            {
                ModelState.AddModelError(string.Empty, "Phải chọn danh mục cha khác");
                canUpdate = false;
            }

            // Kiem tra thiet lap muc cha phu hop
            if (canUpdate && category.ParentCategoryId != null)
            {
                var childCates =
                            (from c in _context.Categories select c)
                            .Include(c => c.CategoryChildren)
                            .ToList()
                            .Where(c => c.ParentCategoryId == category.Id);


                // Func check Id 
                Func<List<Category>, bool> checkCateIds = null;
                checkCateIds = (cates) =>
                    {
                        foreach (var cate in cates)
                        {
                            Console.WriteLine(cate.Title);
                            if (cate.Id == category.ParentCategoryId)
                            {
                                canUpdate = false;
                                ModelState.AddModelError(string.Empty, "Phải chọn danh mục cha khácXX");
                                return true;
                            }
                            if (cate.CategoryChildren != null)
                                return checkCateIds(cate.CategoryChildren.ToList());

                        }
                        return false;
                    };
                // End Func 
                checkCateIds(childCates.ToList());
            }


            if (ModelState.IsValid && canUpdate)
            {
                try
                {
                    if (category.ParentCategoryId == -1)
                        category.ParentCategoryId = null;

                    var dtc = _context.Categories.FirstOrDefault(c => c.Id == id);
                    _context.Entry(dtc).State = EntityState.Detached;

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            await PrepareSelectListAsync();

            return View(category);
        }

        // GET: Blog/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Blog/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Lấy ra danh mục, lấy thêm các danh mục con trong nó
            var category = await _context.Categories
                           .Include(c => c.CategoryChildren)
                           .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            //Kiểm tra danh mục có danh mục con hay không, thiết lập danh mục cha của danh mục con giống với danh mục cha của danh mục xóa
            foreach (var cCategory in category.CategoryChildren)
            {
                cCategory.ParentCategoryId = category.ParentCategoryId;
            }


            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private async Task PrepareSelectListAsync()
        {
            var qr = (from c in _context.Categories select c)
                     .Include(c => c.ParentCategory)
                     .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategory == null)
                             .ToList();
            categories.Insert(0, new Category()
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });

            var items = new List<Category>();
            CreateSelectItems(categories, items, 0);
            var selectList = new SelectList(items, "Id", "Title");
            ViewData["ParentCategoryId"] = selectList;
        }

    }
}