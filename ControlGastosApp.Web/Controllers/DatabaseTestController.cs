using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ControlGastosApp.Web.Utils;
using System.Threading.Tasks;

namespace ControlGastosApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DatabaseTestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var result = await DatabaseConnectionTester.TestConnectionAsync(connectionString);
            
            if (result.success)
            {
                return Ok(new { success = true, message = result.message });
            }
            
            return BadRequest(new { success = false, message = result.message });
        }
    }
} 