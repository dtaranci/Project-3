using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Viewmodels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly EcommerceDwaContext _context;

        public CategoryController(EcommerceDwaContext context)
        {
            _context = context;
        }

        // GET: CategoryController
        public ActionResult Index()
        {
            List<CategoryVM> categories = new List<CategoryVM>();
            
            _context.Categories.ToList().ForEach(x => categories.Add(new CategoryVM { Id = x.IdCategory, Name = x.Name}));
            
            return View(categories);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var category = _context.Categories.FirstOrDefault(x => x.IdCategory == id);

                if (category == null)
                {
                    return NotFound();
                }

                var categoryVM = new CategoryVM
                {
                    Id = category.IdCategory,
                    Name = category.Name,
                };

                return View(categoryVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryVM category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(category);
                }

                var newCategory = new Category
                {
                    Name = category.Name,
                };

                _context.Categories.Add(newCategory);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var category = _context.Categories.FirstOrDefault(x => x.IdCategory == id);

                if (category == null)
                {
                    return NotFound();
                }

                var categoryVM = new CategoryVM
                {
                    Id = category.IdCategory,
                    Name = category.Name
                };

                return View(categoryVM);
            }
            catch (Exception)
            {
                return NotFound();
            }

        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CategoryVM category)
        {
            try
            {
                //var editCategory = _context.Categories.FirstOrDefault(x => x.IdCategory == id);

                //if (category == null)
                //{
                //    return NotFound();
                //}

                //editCategory.Name = category.Name;

                //_context.SaveChanges();

                //return RedirectToAction(nameof(Index));
                if (!ModelState.IsValid)
                {
                    return View(category);
                }

                var editCategory = _context.Categories.FirstOrDefault(x => x.IdCategory == id);

                if (category == null)
                {
                    return NotFound();
                }

                editCategory.Name = category.Name;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var category = _context.Categories.FirstOrDefault(x => x.IdCategory == id);

                if (category == null)
                {
                    return NotFound();
                }

                var categoryVM = new CategoryVM
                {
                    Id = category.IdCategory,
                    Name = category.Name,
                };

                return View(categoryVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var categorytodelete = _context.Categories.FirstOrDefault(x => x.IdCategory == id);

                if (categorytodelete == null)
                {
                    return NotFound();
                }

                _context.Categories.Remove(categorytodelete);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", $"Failed to delete category");
                var temp = _context.Categories.FirstOrDefault(x => x.IdCategory == id);
                return View(new CategoryVM { Id = temp.IdCategory, Name = temp.Name });
            }
        }
    }
}
