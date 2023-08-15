using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaoChi.Models;
using BaoChi.Helpers;
using PagedList.Core;
using System.IO;

namespace BaoChi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly aspnetBlogContext _context;

        public CategoriesController(aspnetBlogContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0?1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            var lsCategories = _context.Categories
                .OrderByDescending(x => x.CatId);
            PagedList<Category> models = new PagedList<Category>(lsCategories, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            //Lấy dữ liệu cha
            ViewData["DanhMucGoc"] = new SelectList(_context.Categories.Where(x => x.Levels == 1), "CatId", "CatName");
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CatName,Title,Alias,MetaDesc,MetaKey,Thumb,Published,Ordering,Parents,Levels,Icon,Cover,Description")] Category category, Microsoft.AspNetCore.Http.IFormFile thumb, Microsoft.AspNetCore.Http.IFormFile cover, Microsoft.AspNetCore.Http.IFormFile icon)
        {
            if (ModelState.IsValid)
            {
                //Xử lý Alias 
                category.Alias = Utilities.SEOUrl(category.CatName);

                //Xử lý danh mục
                if (category.Parents == null)
                {
                    category.Levels = 1; //Không đặt thì mặc định là 1
                }
                else
                {
                    category.Levels = category.Parents == 0 ? 1 : 2; //Nhập parents = 0 thì tự lên 1, khác 0 thì cho bằng 2
                }

                //Xử lý tên ảnh
                if(thumb!= null)
                {
                    string extension = Path.GetExtension(thumb.FileName);
                    string Newname = Utilities.SEOUrl(category.CatName)+"preview_"+extension;
                    category.Thumb = await Utilities.UploadFile(thumb, @"categories\", Newname.ToLower());
                }

                if (cover != null)
                {
                    string extension = Path.GetExtension(cover.FileName);
                    string Newname = "cover_"+ Utilities.SEOUrl(category.CatName) + extension;
                    category.Cover = await Utilities.UploadFile(cover, @"covers\", Newname.ToLower());
                }

                if (icon != null)
                {
                    string extension = Path.GetExtension(icon.FileName);
                    string Newname = "icon_" + Utilities.SEOUrl(category.CatName) + extension;
                    category.Icon = await Utilities.UploadFile(icon, @"icons\", Newname.ToLower());
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
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
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatId,CatName,Title,Alias,MetaDesc,MetaKey,Thumb,Published,Ordering,Parents,Levels,Icon,Cover,Description")] Category category, Microsoft.AspNetCore.Http.IFormFile thumb, Microsoft.AspNetCore.Http.IFormFile cover, Microsoft.AspNetCore.Http.IFormFile icon)
        {
            if (id != category.CatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Xử lý Alias 
                    category.Alias = Utilities.SEOUrl(category.CatName);

                    //Xử lý danh mục
                    if (category.Parents == null)
                    {
                        category.Levels = 1; //Không đặt thì mặc định là 1
                    }
                    else
                    {
                        category.Levels = category.Parents == 0 ? 1 : 2; //Nhập parents = 0 thì tự lên 1, khác 0 thì cho bằng 2
                    }

                    //Xử lý tên ảnh
                    if (thumb != null)
                    {
                        string extension = Path.GetExtension(thumb.FileName);
                        string Newname = Utilities.SEOUrl(category.CatName) + "preview_" + extension;
                        category.Thumb = await Utilities.UploadFile(thumb, @"categories\", Newname.ToLower());
                    }

                    if (cover != null)
                    {
                        string extension = Path.GetExtension(cover.FileName);
                        string Newname = "cover_" + Utilities.SEOUrl(category.CatName) + extension;
                        category.Cover = await Utilities.UploadFile(cover, @"covers\", Newname.ToLower());
                    }

                    if (icon != null)
                    {
                        string extension = Path.GetExtension(icon.FileName);
                        string Newname = "icon_" + Utilities.SEOUrl(category.CatName) + extension;
                        category.Icon = await Utilities.UploadFile(icon, @"icons\", Newname.ToLower());
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CatId))
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
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CatId == id);
        }
    }
}
