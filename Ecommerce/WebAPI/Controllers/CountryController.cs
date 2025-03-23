using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Events;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        public event OnLogCreatedDelegate OnCreate;
        public event OnLogErrorDelegate OnError;

        private readonly EcommerceDwaContext _context;
        public CountryController(EcommerceDwaContext context)
        {
            _context = context;
            try
            {
                OnCreate += (obj, args2) =>
                {
                    //StaticLogs.logs.Add(++StaticLogs.Id + " - " + args2.DateTime.ToString() + " - " + LogLevel.Information.ToString() + " - " + "CountryController: " + args2.Content);
                    StaticLogs.logs.Add(new Log { Id = ++StaticLogs.Id, Timestamp = args2.DateTime, LogLevel = LogLevel.Information, Message = "CountryController: " + args2.Content });

                };
                OnError += (obj, args2) =>
                {
                    //StaticLogs.logs.Add(++StaticLogs.Id + " - " + args2.DateTime.ToString() + " - " + LogLevel.Warning.ToString() + " - " + "CountryController: " + args2.Content + " - " + args2.Exception.Message.ToString());
                    StaticLogs.logs.Add(new Log { Id = ++StaticLogs.Id, Timestamp = args2.DateTime, LogLevel = LogLevel.Warning, Message = "CountryController: " + args2.Content + " - " + args2.Exception.Message.ToString() });

                };
            }
            catch (Exception)
            {
            }
        }

        // GET: api/<CountryController>
        [HttpGet]
        public ActionResult<CountryCreateDto> Get() // ++ W O R K S ++
        {
            try
            {
                //var result = _context.Countries.Select(x => new CountryCreateDto { IdCountry = x.IdCountry, Name = x.Name, Price = x.Price, Country = new CountryCreateDto(x.Country), Description = x.Description, ImgUrl = x.ImgUrl, IsAvailable = x.IsAvailable});
                var result = _context.Countries.Select(x => new CountryCreateDto(x));
                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = "Query for all Countries." });
                return result == null ? NotFound("No Countries available in the database.") : Ok(result);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = "Query for all Countries.", Exception = ex });
                return StatusCode(500, ex.Message);
            }

        }

        // GET api/<CountryController>/5
        [HttpGet("{id}")]
        public ActionResult<CountryCreateDto> Get(int id) // ++ W O R K S ++
        {
            try
            {
                var result = _context.Countries.First((x) => x.IdCountry == id);
                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Query for Country id: ({id})." });
                return Ok(new CountryCreateDto(result));
            }
            catch (InvalidOperationException ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for Country id: ({id}) failed: item was not found.", Exception = ex });

                //return NotFound($"Id: ({id}) was not found. Error: " + ex.Message);
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for Country id: ({id}) failed", Exception = ex });

                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<CountryController>
        [HttpPost]
        public ActionResult<CountryCreateDto> Post([FromBody] CountryCreateDto value) // ++ W O R K S ++
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newCountry = new Country
                {
                    IdCountry = value.IdCountry,
                    Name = value.Name
                };

                _context.Countries.Add(newCountry);

                _context.SaveChanges();

                value.IdCountry = newCountry.IdCountry;

                value.Name = _context.Countries.FirstOrDefault(x => x.IdCountry == value.IdCountry).Name;

                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Insertion for Country id: ({newCountry.IdCountry})." });

                //return Ok(value);
                return StatusCode(201, value); //201 created
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Insertion for new Country name: ({value.Name}) failed.", Exception = ex });

                return StatusCode(400, ex.Message);
            }
        }

        // PUT api/<CountryController>/5
        [HttpPut("{id}")]
        public ActionResult<CountryCreateDto> Put(int id, [FromBody] CountryCreateDto value) // ++ W O R K S ++
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = _context.Countries.FirstOrDefault(x => x.IdCountry == id);
                if (result == null)
                {
                    throw new Exception($"Country with id: ({id}) does not exist");
                }
                result.Name = value.Name == null ? result.Name : value.Name;

                _context.SaveChanges();

                value.IdCountry = result.IdCountry;

                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Update for Country id: ({id})." });


                return Ok(value);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Update for Country id: ({id}) failed.", Exception = ex });

                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<CountryController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id) // ++ W O R K S ++
        {
            try
            {
                if (Get(id).Result is OkObjectResult okResult && okResult.StatusCode == 200) // reuse instead of duplicating code
                {
                    try
                    {
                        var result = _context.Countries.First((x) => x.IdCountry == id);
                        _context.Countries.Remove(result);
                        _context.SaveChanges();

                        OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Deletion for Country id: ({id})." });

                        return StatusCode(204);
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Deletion for Country id: ({id}) failed.", Exception = ex });

                        return StatusCode(500, ex.Message);
                    }
                }
                else
                {
                    OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Deletion for Country id: ({id}) failed.", Exception = new MissingFieldException($"Country id: ({id}) does not exist") });

                    return StatusCode(404, $"Country id: ({id}) does not exist.");
                }
            }
            catch (Exception ev)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Deletion for Country id: ({id}) failed.", Exception = ev });
                return StatusCode(500, ev.Message);
            }
        }

        // GET api/<CountryController>/search
        [HttpGet("search")]
        public ActionResult<IEnumerable<CountryCreateDto>> GetByName(string name, int count = 1, int page = 1) // ++ W O R K S ++
        {
            try
            {
                var result = _context.Countries.Where((x) => x.Name.ToLower().Contains(name.ToLower())); // Include Country fix
                OnCreate?.Invoke(this, new OnLogCreatedArgs { DateTime = DateTime.Now, Content = $"Query for Countries with name: ({name})." });
                var list = new List<CountryCreateDto>();
                result?.ToList().ForEach(x => list.Add(new CountryCreateDto(x)));

                var pagelist = new List<CountryCreateDto>();
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
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for Countries with name: ({name}) failed: items were not found.", Exception = ex });

                return NotFound($"Name: ({name}) was not found. Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnLogErrorArgs { DateTime = DateTime.Now, Content = $"Query for Countries with name: ({name}) failed", Exception = ex });

                return StatusCode(500, ex.Message);
            }
        }
    }
}
