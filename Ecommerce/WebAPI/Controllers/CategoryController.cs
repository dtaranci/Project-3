using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Events;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public event OnLogCreatedDelegate OnCreate;
        public event OnLogErrorDelegate OnError;

        private readonly EcommerceDwaContext _context;
        public CategoryController(EcommerceDwaContext context)
        {
            _context = context;
            try
            {
                OnCreate += (obj, args2) =>
                {
                    //StaticLogs.logs.Add(++StaticLogs.Id + " - " + args2.DateTime.ToString() + " - " + LogLevel.Information.ToString() + " - " + "CategoryController: " + args2.Content);
                    StaticLogs.logs.Add(new Log { Id = ++StaticLogs.Id, Timestamp = args2.DateTime, LogLevel = LogLevel.Information, Message = "CategoryController: " + args2.Content });
                };
                OnError += (obj, args2) =>
                {
                    //StaticLogs.logs.Add(++StaticLogs.Id + " - " + args2.DateTime.ToString() + " - " + LogLevel.Warning.ToString() + " - " + "CategoryController: " + args2.Content + " - " + args2.Exception.Message.ToString());
                    StaticLogs.logs.Add(new Log { Id = ++StaticLogs.Id, Timestamp = args2.DateTime, LogLevel = LogLevel.Warning, Message = "CategoryController: " + args2.Content + " - " + args2.Exception.Message.ToString() });

                };
            }
            catch (Exception)
            {
            }
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public ActionResult<CategoryCreateDto> Get() // ++ W O R K S ++
        {
            try
            {
                //var result = _context.Categories.Select(x => new CategoryCreateDto { IdCategory = x.IdCategory, Name = x.Name, Price = x.Price, Category = new CategoryCreateDto(x.Category), Description = x.Description, ImgUrl = x.ImgUrl, IsAvailable = x.IsAvailable});
                var result = _context.Categories.Select(x => new CategoryCreateDto(x));
                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = "Query for all Categories." });
                return result == null ? NotFound("No Categories available in the database.") : Ok(result);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = "Query for all Categories.", Exception = ex });
                return StatusCode(500, ex.Message);
            }

        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public ActionResult<CategoryCreateDto> Get(int id) // ++ W O R K S ++
        {
            try
            {
                var result = _context.Categories.First((x) => x.IdCategory == id);
                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Query for Category id: ({id})." });
                return Ok(new CategoryCreateDto(result));
            }
            catch (InvalidOperationException ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for Category id: ({id}) failed: item was not found.", Exception = ex });

                //return NotFound($"Id: ({id}) was not found. Error: " + ex.Message);
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for Category id: ({id}) failed", Exception = ex });

                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<CategoryController>
        [HttpPost]
        public ActionResult<CategoryCreateDto> Post([FromBody] CategoryCreateDto value) // ++ W O R K S ++
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newCategory = new Category
                {
                    IdCategory = value.IdCategory,
                    Name = value.Name
                };

                _context.Categories.Add(newCategory);

                _context.SaveChanges();

                value.IdCategory = newCategory.IdCategory;

                value.Name = _context.Categories.FirstOrDefault(x => x.IdCategory == value.IdCategory).Name;

                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Insertion for Category id: ({newCategory.IdCategory})." });

                //return Ok(value);
                return StatusCode(201, value); //201 created
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Insertion for new Category name: ({value.Name}) failed.", Exception = ex });

                return StatusCode(400, ex.Message);
            }
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{id}")]
        public ActionResult<CategoryCreateDto> Put(int id, [FromBody] CategoryCreateDto value) // ++ W O R K S ++
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = _context.Categories.FirstOrDefault(x => x.IdCategory == id);
                if (result == null)
                {
                    throw new Exception($"Category with id: ({id}) does not exist");
                }
                result.Name = value.Name == null ? result.Name : value.Name;

                _context.SaveChanges();

                value.IdCategory = result.IdCategory;

                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Update for Category id: ({id})." });


                return Ok(value);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Update for Category id: ({id}) failed.", Exception = ex });

                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id) // ++ W O R K S ++
        {
            try
            {
                if (Get(id).Result is OkObjectResult okResult && okResult.StatusCode == 200) // reuse instead of duplicating code
                {
                    try
                    {
                        var result = _context.Categories.First((x) => x.IdCategory == id);
                        _context.Categories.Remove(result);
                        _context.SaveChanges();

                        OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Deletion for Category id: ({id})." });

                        return StatusCode(204);
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Deletion for Category id: ({id}) failed.", Exception = ex });

                        return StatusCode(500, ex.Message);
                    }
                }
                else
                {
                    OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Deletion for Category id: ({id}) failed.", Exception = new MissingFieldException($"Category id: ({id}) does not exist") });

                    return StatusCode(404, $"Category id: ({id}) does not exist.");
                }
            }
            catch (Exception ev)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Deletion for Category id: ({id}) failed.", Exception = ev });
                return StatusCode(500, ev.Message);
            }
        }

        // GET api/<CategoryController>/search
        [HttpGet("search")]
        public ActionResult<IEnumerable<CategoryCreateDto>> GetByName(string name, int count = 1, int page = 1) // ++ W O R K S ++
        {
            try
            {
                var result = _context.Categories.Where((x) => x.Name.ToLower().Contains(name.ToLower())); // Include category fix
                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Query for Categories with name: ({name})." });
                var list = new List<CategoryCreateDto>();
                result?.ToList().ForEach(x => list.Add(new CategoryCreateDto(x)));

                var pagelist = new List<CategoryCreateDto>();
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

                return Ok(pagelist);
            }
            catch (InvalidOperationException ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for Categories with name: ({name}) failed: items were not found.", Exception = ex });

                return NotFound($"Name: ({name}) was not found. Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for Categories with name: ({name}) failed", Exception = ex });

                return StatusCode(500, ex.Message);
            }
        }
    }
}
