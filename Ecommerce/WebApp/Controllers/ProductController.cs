//using Humanizer.Localisation; .NET 8.0
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Viewmodels;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;

namespace WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly EcommerceDwaContext _context;
        private readonly IConfiguration _configuration;

        public ProductController(EcommerceDwaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: ProductController
        public ActionResult Index()
        {
            //ViewBag.Products = _context.Products;
            var products = new List<ProductVM>();
            _context.Products.ToList().ForEach(x => products.Add(new ProductVM { Name = x.Name, CategoryId = x.CategoryId, Description = x.Description, Id = x.IdProduct, ImageURL = x.ImgUrl, IsAvailable = x.IsAvailable, Price = x.Price }));
            return View(products);  
        }

        //public ActionResult Search(string term, string orderby = "", int page = 1, int size = 10)
        //{
        //    try
        //    {
        //        IQueryable<Product> products = _context.Products.Include(x => x.Category);

        //        if (!string.IsNullOrEmpty(term))
        //        {
        //            products = products.Where(x => x.Name.Contains(term));
        //        }

        //        //...sorting, filtering...
        //        var productVms = products.Select(x => new ProductVM { Name = x.Name, CategoryId = x.CategoryId, Description = x.Description, Id = x.IdProduct, ImageURL = x.ImgUrl, IsAvailable = x.IsAvailable, Price = x.Price });

        //        return View(productVms);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public ActionResult Search(SearchVM searchVm)
        {
            try
            {
                IQueryable<Product> products = _context.Products.Include(x => x.Category).Include(x => x.CountryProducts).ThenInclude(x => x.Country);

                if (!string.IsNullOrEmpty(searchVm.Term))
                {
                    products = products.Where(x => x.Name.Contains(searchVm.Term));
                }

                var filteredCount = products.Count();

                if (!string.IsNullOrEmpty(searchVm.OrderBy))
                {
                    /*switch (searchVm.OrderBy.ToLower())
                    {
                        case "id":
                            products = products.OrderBy(x => x.IdProduct);
                            break;
                        case "name":
                            products = products.OrderBy(x => x.Name);
                            break;
                        case "category":
                            products = products.OrderBy(x => x.Category);
                            break;
                        case "country":
                            products = products.Include(x => x.CountryProducts).OrderBy(x => x.CountryProducts);
                            break;
                        case "price":
                            products = products.OrderBy(x => x.Price);
                            break;
                        case "available":
                            products = products.OrderBy(x => x.IsAvailable);
                            break;
                        case "description":
                            products = products.OrderBy(x => x.Description);
                            break;
                    }*/

                    products = products.Include(x => x.Category).Where(x => (x.Category.Name == searchVm.OrderBy));
                    filteredCount = products.Count();
                }
                products = products.Skip((searchVm.Page - 1) * searchVm.Size).Take(searchVm.Size);
                //searchVm.Products was IEnumerable and used = select new ProductVM
                searchVm.Products = new List<ProductVM>();
                products.Include(x => x.Category).Include(x => x.CountryProducts).ThenInclude(x => x.Country).ToList().ForEach(x =>
                {
                    List<CountryVM> countryvar = new List<CountryVM>();
                    x.CountryProducts.ToList().ForEach(y => countryvar.Add(new CountryVM { Name = y.Country.Name, Id = y.Country.IdCountry }));
                    searchVm.Products.Add( new ProductVM { Name = x.Name, CategoryId = x.CategoryId, CategoryName = x.Category.Name, Description = x.Description, Id = x.IdProduct, ImageURL = x.ImgUrl, IsAvailable = x.IsAvailable, Price = x.Price, AvailableCountries = countryvar });
                });

                // BEGIN PAGER
                var expandPages = _configuration.GetValue<int>("Paging:ExpandPages");
                searchVm.LastPage = (int)Math.Ceiling(1.0 * filteredCount / searchVm.Size);
                searchVm.FromPager = searchVm.Page > expandPages ?
                  searchVm.Page - expandPages :
                  1;
                searchVm.ToPager = (searchVm.Page + expandPages) < searchVm.LastPage ?
                  searchVm.Page + expandPages :
                  searchVm.LastPage;
                // END PAGER

                ViewBag.OnetoMany = _context.Categories.ToList(); //viewbag for categories

                return View(searchVm);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var product = _context.Products.Include(x => x.CountryProducts).ThenInclude(x => x.Country).FirstOrDefault(x => x.IdProduct == id);

                if (product == null)
                {
                    return NotFound();
                }

                List<CountryVM> countryVMs = new List<CountryVM>();

                product.CountryProducts.ToList().ForEach(x => countryVMs.Add(new CountryVM
                {
                    Id = x.CountryId,
                    Name = x.Country.Name,
                    IsSelected = true //if it did not belong to this product it wouldn't be in the list.
                }));

                var productVM = new ProductVM
                {
                    Id = product.IdProduct,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageURL = product.ImgUrl,
                    CategoryId = product.CategoryId,
                    AvailableCountries = countryVMs.ToArray(),
                    IsAvailable = product.IsAvailable
                };
                ViewBag.CategoryName = _context.Categories
                .First(x => x.IdCategory == product.CategoryId).Name;

                return View(productVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: ProductController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ProductVM newProduct = new ProductVM();
           // List<CountryVM> temp = new List<CountryVM>();
            _context.Countries.ToList().ForEach(x => newProduct.AvailableCountries.Add(new CountryVM { Id = x.IdCountry, Name = x.Name }));
            //ViewBag.Countries = temp.ToList();

            ViewBag.Categories = _context.Categories.ToList();

            return View(newProduct);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(ProductVM product)
        {
            try
            {

                product.ValidateSelectedCountry(ModelState);

                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = _context.Categories.ToList();
                    return View(product);
                }

                var newProduct = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    ImgUrl = product.ImageURL,
                    IsAvailable = product.IsAvailable
                };

                foreach (var country in product.AvailableCountries)
                {
                    if (country.IsSelected && _context.Countries.Any(x => x.IdCountry == country.Id))
                    {
                        newProduct.CountryProducts.Add(new CountryProduct
                        {
                            Country = _context.Countries.First(x => x.IdCountry == country.Id),
                            Product = newProduct
                        });
                    }
                    //newProduct.CountryProducts.Add(new CountryProduct
                    //{
                    //    CountryId = country.Id,
                    //    Product = newProduct
                    //});
                }

                _context.Products.Add(newProduct);

                _context.SaveChanges();

                return RedirectToAction(nameof(Search));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            try
            {
                var product = _context.Products.Include(x => x.CountryProducts).FirstOrDefault(x => x.IdProduct == id);

                if (product == null)
                {
                    return NotFound();
                }

                IList<CountryVM> allcountries = new List<CountryVM>();

                _context.Countries.ToList().ForEach(x => allcountries.Add(new CountryVM { Id = x.IdCountry, Name = x.Name, IsSelected = false}));

                IList<CountryVM> selectedcountries = new List<CountryVM>();
                product.CountryProducts.ToList().ForEach(x => selectedcountries.Add(new CountryVM { Id = x.Country.IdCountry, Name = x.Country.Name, IsSelected = true }));


                var filteredlist = allcountries.Where(x => !selectedcountries.Any(y => y.Id == x.Id));

                var finallist = selectedcountries.Union(filteredlist).ToList();

                var productVM = new ProductVM
                {
                    Id = product.IdProduct,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageURL = product.ImgUrl,
                    CategoryId = product.CategoryId,
                    AvailableCountries = finallist,
                    IsAvailable = product.IsAvailable
                };

                ViewBag.Categories = _context.Categories.ToList();

                return View(productVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        //// POST: ProductController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        //public ActionResult Edit(int id, ProductVM product)
        //{
        //    try
        //    {
        //        var editProduct = _context.Products.FirstOrDefault(x => x.IdProduct == id);
        //        editProduct.Name = product.Name;
        //        editProduct.Description = product.Description;
        //        editProduct.Price = product.Price;
        //        editProduct.CategoryId = product.CategoryId;
        //        editProduct.ImgUrl = product.ImageURL;

        //        foreach (var country in product.AvailableCountries)
        //        {
        //            var newCountryProduct = new CountryProduct
        //            {
        //                Country = _context.Countries.First(x => x.IdCountry == country.Id),
        //                Product = editProduct
        //            };
        //            if (country.IsSelected && _context.Countries.Any(x => x.IdCountry == country.Id))
        //            {
        //                if (!editProduct.CountryProducts.Any(x => x.CountryId == newCountryProduct.CountryId && x.ProductId == newCountryProduct.ProductId))
        //                {
        //                    editProduct.CountryProducts.Add(newCountryProduct);
        //                }
        //            }
        //            else
        //            {
        //                if (!country.IsSelected && _context.Countries.Any(x => x.IdCountry == country.Id) && editProduct.CountryProducts.Any(x => x.CountryId == newCountryProduct.CountryId && x.ProductId == newCountryProduct.ProductId))
        //                {
        //                    editProduct.CountryProducts.Remove(newCountryProduct);
        //                }
        //            }
        //        }

        //        _context.SaveChanges();

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, ProductVM product)
        {
            try
            {
                //fail condition
                //var validationContext = new ValidationContext(product);
                //product.ValidateSelectedCountry(validationContext);

                product.ValidateSelectedCountry(ModelState);

                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = _context.Categories.ToList();
                    return View(product);
                }

                var editProduct = _context.Products
                    .Include(x => x.CountryProducts)
                    .FirstOrDefault(x => x.IdProduct == id);

                if (editProduct == null)
                {
                    return NotFound();
                }

                editProduct.Name = product.Name;
                editProduct.Description = product.Description;
                editProduct.Price = product.Price;
                editProduct.CategoryId = product.CategoryId;
                editProduct.ImgUrl = product.ImageURL;
                editProduct.IsAvailable = product.IsAvailable;

                var existingCountryProducts = editProduct.CountryProducts.ToList();

                foreach (var country in product.AvailableCountries)
                {
                    var CountryContext = _context.Countries.FirstOrDefault(x => x.IdCountry == country.Id);
                    if (CountryContext == null)
                    {
                        continue;
                    }

                    if (country.IsSelected)
                    {
                        if (!existingCountryProducts.Any(x => x.CountryId == CountryContext.IdCountry && x.ProductId == editProduct.IdProduct))
                        {
                            var newCountryProduct = new CountryProduct
                            {
                                Country = CountryContext,
                                Product = editProduct
                            };
                            editProduct.CountryProducts.Add(newCountryProduct);
                        }
                    }
                    else
                    {
                        var existingCountryProduct = existingCountryProducts
                            .FirstOrDefault(x => x.CountryId == CountryContext.IdCountry && x.ProductId == editProduct.IdProduct);
                        if (existingCountryProduct != null)
                        {
                            _context.CountryProducts.Remove(existingCountryProduct);
                        }
                    }
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Search));
            }
            catch (Exception ex)
            {
                return View(product);
            }
        }

        // GET: ProductController/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(x => x.IdProduct == id);

                if (product == null)
                {
                    return NotFound();
                }

                var productVM = new ProductVM
                {
                    Id = product.IdProduct,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageURL = product.ImgUrl,
                    CategoryId = product.CategoryId
                };

                return View(productVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, ProductVM product)
        {
            try
            {
                //delete M-N relations before attempting deletion of primary

                List<CountryProduct> toRemove = new List<CountryProduct>();

                if (_context.CountryProducts.Any(x => x.ProductId == id))
                {
                    _context.CountryProducts.Where(x => x.ProductId == id).ToList().ForEach(x => toRemove.Add(x));
                }

                foreach (CountryProduct countryProduct in toRemove)
                {
                    _context.CountryProducts.Remove(countryProduct);
                }

                _context.SaveChanges();

                var producttodelete = _context.Products.FirstOrDefault(x => x.IdProduct == id);

                _context.Products.Remove(producttodelete);

                _context.SaveChanges();

                return RedirectToAction(nameof(Search));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Purchase(int id, ProductVM product)
        {
            try
            {
                var productt = _context.Products.First(x => x.IdProduct == id);
                ViewBag.Product = new ProductVM
                {
                    Id = productt.IdProduct,
                    Name = productt.Name,
                    Price = productt.Price,
                    ImageURL = productt.ImgUrl
                };
                return View(product);
            }
            catch (Exception)
            {

                return NotFound();
            }
        }
    }
}
