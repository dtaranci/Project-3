using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {

        [HttpGet("get/{N}")]
        public ActionResult<IEnumerable<Models.Log>> Get(int N = 10) // ++ W O R K S ++
        {

            var temp = new List<Models.Log>();
            int counter = 0;

            for (int i = StaticLogs.logs.Count - 1; i >= 0; i--)
            {
                if (counter++ >= N)
                {
                    break;
                }
                temp.Add(StaticLogs.logs[i]);
            }

            return temp;
        }

        [HttpGet("count")]
        public ActionResult<int> GetCount() // ++ W O R K S ++
        {
            try
            {
                return StaticLogs.logs.Count;
            }
            catch (Exception)
            {
            }

            return StatusCode(500);
        }
    }
}
