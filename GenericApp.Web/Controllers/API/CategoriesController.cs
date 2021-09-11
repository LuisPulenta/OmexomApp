using Microsoft.AspNetCore.Mvc;
using GenericApp.Web.Data;

namespace GenericApp.Web.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoriesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            return Ok(_context.Categories);
        }
    }
}