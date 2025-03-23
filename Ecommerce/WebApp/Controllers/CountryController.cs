using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Viewmodels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountryController : Controller
    {
        private readonly EcommerceDwaContext _context;

        public CountryController(EcommerceDwaContext context)
        {
            _context = context;
        }

        // GET: CountryController
        public ActionResult Index()
        {
            List<CountryVM> countries = new List<CountryVM>();

            _context.Countries.ToList().ForEach(x => countries.Add(new CountryVM { Id = x.IdCountry, Name = x.Name}));

            return View(countries);
        }

        // GET: CountryController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var country = _context.Countries.FirstOrDefault(x => x.IdCountry == id);

                if (country == null)
                {
                    return NotFound();
                }

                var countryVM = new CountryVM
                {
                    Id = country.IdCountry,
                    Name = country.Name,
                };

                return View(countryVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: CountryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CountryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CountryVM country)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(country);
                }

                var newCountry = new Country
                {
                    Name = country.Name,
                };

                _context.Countries.Add(newCountry);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CountryController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var country = _context.Countries.FirstOrDefault(x => x.IdCountry == id);

                if (country == null)
                {
                    return NotFound();
                }

                var countryVM = new CountryVM
                {
                    Id = country.IdCountry,
                    Name = country.Name
                };

                return View(countryVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: CountryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CountryVM country)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(country);
                }

                var countrytoedit = _context.Countries.FirstOrDefault(x => x.IdCountry == id);

                if (countrytoedit == null)
                {
                    return NotFound();
                }

                countrytoedit.Name = country.Name;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CountryController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var country = _context.Countries.FirstOrDefault(x => x.IdCountry == id);

                if (country == null)
                {
                    return NotFound();
                }

                var countryVM = new CountryVM
                {
                    Id = country.IdCountry,
                    Name = country.Name
                };

                return View(countryVM);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: CountryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, CountryVM country)
        {
            try
            {
                var countrytodelete = _context.Countries.FirstOrDefault(x => x.IdCountry == id);

                if (countrytodelete == null)
                {
                    return NotFound();
                }

                _context.Countries.Remove(countrytodelete);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var temp = _context.Countries.FirstOrDefault(x => x.IdCountry == id);
                
                ModelState.AddModelError("", $"Failed to delete country"); ;

                return View(new CountryVM { Id = temp.IdCountry, Name = temp.Name});
            }
        }
    }
}
