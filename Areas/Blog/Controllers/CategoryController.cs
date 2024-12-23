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
        public async Task<IActionResult> Create([Bind("Keyword,Title,Description,Content,Slug,SeoTitle,SeoDescription,ParentCategoryId,Status,IndexFollow,FileUpload")] Category category)
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
                    // Tách phần tên và phần mở rộng
                    string originalFileName = Path.GetFileNameWithoutExtension(category.FileUpload.FileName); // Lấy tên tệp không bao gồm phần mở rộng
                    string fileExtension = Path.GetExtension(category.FileUpload.FileName); // Lấy phần mở rộng của tệp (bao gồm dấu '.')

                    // Tạo tên tệp mới theo định dạng: tên-gốc-{id}.phần-mở-rộng
                    string fileName = $"{originalFileName}-{category.Id}{fileExtension}";

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Keyword,Title,Description,Content,Slug,SeoTitle,SeoDescription,ParentCategoryId,Status,IndexFollow,FileUpload")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            bool canUpdate = true;

            // Kiểm tra trường hợp danh mục cha không hợp lệ
            if (category.ParentCategoryId == category.Id)
            {
                ModelState.AddModelError(string.Empty, "Phải chọn danh mục cha khác");
                canUpdate = false;
            }

            // Kiểm tra vòng lặp cha-con
            if (canUpdate && category.ParentCategoryId != null)
            {
                var childCategories = _context.Categories
                    .Include(c => c.CategoryChildren)
                    .Where(c => c.ParentCategoryId == category.Id)
                    .ToList();

                Func<List<Category>, bool> hasInvalidParent = null;
                hasInvalidParent = (categories) =>
                {
                    foreach (var cat in categories)
                    {
                        if (cat.Id == category.ParentCategoryId)
                        {
                            canUpdate = false;
                            ModelState.AddModelError(string.Empty, "Phải chọn danh mục cha khác");
                            return true;
                        }
                        if (cat.CategoryChildren != null)
                            return hasInvalidParent(cat.CategoryChildren.ToList());
                    }
                    return false;
                };
                hasInvalidParent(childCategories);
            }

            if (ModelState.IsValid && canUpdate)
            {
                try
                {
                    if (category.ParentCategoryId == -1)
                        category.ParentCategoryId = null;

                    // Lấy dữ liệu cũ từ database
                    var existingCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }

                    // Xử lý file upload (nếu có)
                    if (category.FileUpload != null)
                    {
                        // Tách phần tên và phần mở rộng
                        string originalFileName = Path.GetFileNameWithoutExtension(category.FileUpload.FileName); // Lấy tên tệp không bao gồm phần mở rộng
                        string fileExtension = Path.GetExtension(category.FileUpload.FileName); // Lấy phần mở rộng của tệp (bao gồm dấu '.')

                        // Tạo tên tệp mới theo định dạng: tên-gốc-{id}.phần-mở-rộng
                        string fileName = $"{originalFileName}-{category.Id}{fileExtension}";

                        // Tạo đường dẫn
                        var uploadsFolder = Path.Combine("Uploads", "Blogs");
                        Directory.CreateDirectory(uploadsFolder); // Đảm bảo thư mục tồn tại
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Lưu file mới
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await category.FileUpload.CopyToAsync(fileStream);
                        }

                        // Xóa file cũ (nếu tồn tại)
                        if (!string.IsNullOrEmpty(existingCategory.Avatar))
                        {
                            var oldFilePath = Path.Combine(uploadsFolder, existingCategory.Avatar);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Cập nhật Avatar
                        category.Avatar = fileName;
                    }
                    else
                    {
                        // Giữ nguyên Avatar cũ nếu không có file mới
                        category.Avatar = existingCategory.Avatar;
                    }

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