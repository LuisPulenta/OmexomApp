using Microsoft.AspNetCore.Mvc;
using GenericApp.Web.Data;

namespace GenericApp.Web.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatesController : ControllerBase
    {
        private readonly DataContext _context;

        public StatesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetStates()
        {
            return Ok(_context.States);
        }
    }
}