using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using WebAPI.DTOs;
using WebAPI.Events;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public event OnLogCreatedDelegate OnCreate;
        public event OnLogErrorDelegate OnError;

        private readonly EcommerceDwaContext _context;
        public ProductController(EcommerceDwaContext context)
        {
            _context = context;
            try
            {
                OnCreate += (obj, args2) =>
                {
                    //StaticLogs.logs.Add(++StaticLogs.Id + " - " + args2.DateTime.ToString() + " - " + LogLevel.Information.ToString() + " - " + "ProductController: " + args2.Content);
                    StaticLogs.logs.Add(new Log { Id = ++StaticLogs.Id, Timestamp = args2.DateTime, LogLevel = LogLevel.Information, Message = "ProductController: " + args2.Content });
                };
                OnError += (obj, args2) =>
                {
                    //StaticLogs.logs.Add(++StaticLogs.Id + " - " + args2.DateTime.ToString() + " - " + LogLevel.Warning.ToString() + " - " + "ProductController: " + args2.Content + " - " + args2.Exception.Message.ToString());
                    StaticLogs.logs.Add(new Log { Id = ++StaticLogs.Id, Timestamp = args2.DateTime, LogLevel = LogLevel.Warning, Message = "ProductController: " + args2.Content + " - " + args2.Exception.Message.ToString() });
                };
            }
            catch (Exception)
            {
            }
        }

        // GET: api/<ProductController>
        [HttpGet]
        public ActionResult<ProductDto> Get() // ++ W O R K S ++
        {
            try
            {
                //var result = _context.Products.Select(x => new ProductDto { IdProduct = x.IdProduct, Name = x.Name, Price = x.Price, Category = new CategoryDto(x.Category), Description = x.Description, ImgUrl = x.ImgUrl, IsAvailable = x.IsAvailable});
                var result = _context.Products.Include(x => x.Category).Select(x => new ProductDto(x, _context)); // M-N

                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = "Query for all products." });
                return result == null ? NotFound("No products available in the database.") : Ok(result);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = "Query for all products.", Exception = ex });
                return StatusCode(500, ex.Message);
            }

        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public ActionResult<ProductDto> Get(int id) // ++ W O R K S ++
        {
            try
            {
                //return Ok(new ProductDto(_context.Products.First((x) => x.IdProduct == id))); //first so that an exception is triggered if missing
                var result = _context.Products.Include(x => x.Category).Include(x => x.CountryProducts).First((x) => x.IdProduct == id); // Include category fix
                //return Ok(new ProductDto{ IdProduct = result.IdProduct, Name = result.Name, Price = result.Price, Category = new CategoryDto(result.Category), Description = result.Description, ImgUrl = result.ImgUrl, IsAvailable = result.IsAvailable }); //first so that an exception is triggered if missing
                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Query for product id: ({id})." });
                return Ok(new ProductDto(result, _context)); //first so that an exception is triggered if missing
            }
            catch (InvalidOperationException ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for product id: ({id}) failed: item was not found.", Exception = ex });

                //return NotFound($"Id: ({id}) was not found. Error: " + ex.Message);
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for product id: ({id}) failed", Exception = ex });

                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<ProductController>
        [HttpPost]
        public ActionResult<ProductDto> Post([FromBody] ProductCreateDto value) // ++ W O R K S ++
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //M-N
                List<CountryProduct> couProList = new List<CountryProduct>();
                

                var newProduct = new Product
                {
                    Name = value.Name,
                    CategoryId = value.Category.IdCategory,
                    Description = value.Description,
                    ImgUrl = value.ImgUrl,
                    IsAvailable = value.IsAvailable,
                    Price = value.Price
                };

                _context.Products.Add(newProduct);

                _context.SaveChanges();



                //value.IdProduct = newProduct.IdProduct;

                //value.Category.Name = _context.Categories.FirstOrDefault(x => x.IdCategory == value.Category.IdCategory).Name;


                //M-N after autogen id retrieved, I need the autogen id which is granted after creation in order to use it to fill CountryProduct

                if (value.Countries.Any(x => !_context.Countries.Any(y => y.IdCountry == x.IdCountry)))
                {
                    throw new Exception("Country ID does not exist.");
                }

                value.Countries.ToList().ForEach(x => couProList.Add(new CountryProduct { CountryId = x.IdCountry, ProductId = newProduct.IdProduct }));

                newProduct.CountryProducts = couProList;

                _context.SaveChanges();

                //value.Countries.ToList().ForEach(x => x.Name = _context.Countries.First(y => y.IdCountry == x.IdCountry).Name); //add country names to return

                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Insertion for product id: ({newProduct.IdProduct})." });

                //create output with Id
                ProductDto OutputProduct = new ProductDto(value, _context, newProduct.IdProduct);

                OutputProduct.IdProduct = newProduct.IdProduct;

                //return Ok(value);
                return StatusCode(201,OutputProduct); //201 created
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Insertion for new product name: ({value.Name}) failed.", Exception = ex });

                return StatusCode(400, ex.Message);
            }
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public ActionResult<ProductDto> Put(int id, [FromBody] ProductDto value) // ++ W O R K S ++
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = _context.Products.Include(x => x.Category).Include(x => x.CountryProducts).FirstOrDefault(x => x.IdProduct == id);
                if (result == null)
                {
                    throw new Exception($"Product with id: ({id}) does not exist");
                }

                List<CountryProduct> couProList = new List<CountryProduct>();
                List<CountryProduct> couProListRemove = new List<CountryProduct>();
                value.Countries.ToList().ForEach(x => couProList.Add(new CountryProduct { CountryId = x.IdCountry, ProductId = id }));

                if (_context.CountryProducts.Any(x => x.ProductId == id)) // don't attempt to add duplicates, remove them from the add list before adding
                {
                    // logic because .Remove(x) != couProList's list item
                    _context.CountryProducts.Where(x => x.ProductId == id).ToList().ForEach(x => couProList.ForEach(y =>
                    {
                        if (y.CountryId == x.CountryId)
                        {
                            couProListRemove.Add(y);
                        }
                    }));
                }
                //remove existing M-N relations not in value anymore
                List<CountryProduct> toRemove = new List<CountryProduct>();
                _context.CountryProducts.ToList().Where(a => a.ProductId == id).Where(x => !couProList.Any(y => y.CountryId == x.CountryId)).ToList().ForEach(z => toRemove.Add(z));
                foreach (CountryProduct countryProduct in toRemove)
                {
                    _context.CountryProducts.Remove(countryProduct);
                }

                foreach (var country in couProListRemove) //anti duplicate
                {
                    couProList.Remove(country);
                }


                result.Name = value.Name == null ? result.Name : value.Name;
                result.Price = value.Price == 0 ? result.Price : value.Price;
                result.IsAvailable = value.IsAvailable == null ? result.IsAvailable : value.IsAvailable;
                result.CategoryId = value.Category.IdCategory == null ? result.CategoryId : value.Category.IdCategory;
                result.Description = value.Description == null ? result.Description : value.Description;
                result.ImgUrl = value.ImgUrl == null ? result.ImgUrl : value.ImgUrl;
                couProList.ToList().ForEach(x => result.CountryProducts.Add(x));



                _context.SaveChanges();

                value.IdProduct = result.IdProduct;

                //value.Countries.ToList().ForEach(x => x.Name = _context.Countries.First(y => y.IdCountry == x.IdCountry).Name);

                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Update for product id: ({id})." });


                return Ok(value);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Update for product id: ({id}) failed.", Exception = ex });

                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id) // ++ W O R K S ++
        {
            try
            {
                if (Get(id).Result is OkObjectResult okResult && okResult.StatusCode == 200) // reuse instead of duplicating code
                {
                    try
                    {
                        var result = _context.Products.Include(x => x.CountryProducts).First((x) => x.IdProduct == id);

                        _context.CountryProducts.ToList().Where(x => x.ProductId == id).ToList().ForEach(x => _context.CountryProducts.Remove(x));

                        _context.Products.Remove(result);
                        _context.SaveChanges();

                        OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Deletion for product id: ({id})." });

                        return StatusCode(204);
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Deletion for product id: ({id}) failed.", Exception = ex });

                        return StatusCode(500, ex.Message);
                    }
                }
                else
                {
                    OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Deletion for product id: ({id}) failed.", Exception = new MissingFieldException($"Product id: ({id}) does not exist") });

                    return StatusCode(404, $"Product id: ({id}) does not exist.");
                }
            }
            catch (Exception ev)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Deletion for product id: ({id}) failed.", Exception = ev });
                return StatusCode(500, ev.Message);
            }
        }

        // GET api/<ProductController>/search
        [HttpGet("search")]
        public ActionResult<IEnumerable<ProductDto>> GetByName(string name, int count = 1, int page = 1 ) // ++ W O R K S ++
        {                                                     //      gtx780          2              1
            try
            {
                //return Ok(new ProductDto(_context.Products.First((x) => x.IdProduct == id))); //first so that an exception is triggered if missing
                var result = _context.Products.Include(x => x.Category).Where((x) => x.Name.ToLower().Contains(name.ToLower())); // Include category fix
                //return Ok(new ProductDto{ IdProduct = result.IdProduct, Name = result.Name, Price = result.Price, Category = new CategoryDto(result.Category), Description = result.Description, ImgUrl = result.ImgUrl, IsAvailable = result.IsAvailable }); //first so that an exception is triggered if missing
                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Query for products with name: ({name})." });
                var list = new List<ProductDto>();
                result?.ToList().ForEach(x => list.Add(new ProductDto(x, _context)));

                var pagelist = new List<ProductDto>();
                int startcount = count;
                int countpageresume = count * page;
                int listed = 0;
                for (int i = countpageresume - startcount; i < list.Count; i++)
                {
                    if (listed >= count)
                    {
                        break;
                    }
                    pagelist.Add(list[i]);
                    ++listed;
                }

                return Ok(pagelist); //first so that an exception is triggered if missing
            }
            catch (InvalidOperationException ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for products with name: ({name}) failed: items were not found.", Exception = ex });

                return NotFound($"Name: ({name}) was not found. Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for products with name: ({name}) failed", Exception = ex });

                return StatusCode(500, ex.Message);
            }
        }
    }
}
