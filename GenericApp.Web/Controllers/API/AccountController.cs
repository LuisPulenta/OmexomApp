using GenericApp.Common.Requests;
using GenericApp.Common.Responses;
using GenericApp.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApp.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public AccountController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUser(UsuarioRequest userRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _dataContext.Usuarios.FirstOrDefaultAsync(o => o.Login.ToLower() == userRequest.Email.ToLower());

            if (user == null)
            {
                return BadRequest("El Usuario no existe.");
            }

            var response = new UsuarioAppResponse
            {
                IDUsuario = user.IDUsuario,
                Login = user.Login,
                Contrasena = user.Contrasena,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                AutorWOM = user.AutorWOM,
                Estado = user.Estado,
            };

            return Ok(response);
        }


        [HttpGet]
        [Route("GetObras")]
        public async Task<IActionResult> GetObras()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var obras = await _dataContext.Obras
            .Include(p => p.ObrasDocumentos)
           .Where(o => (o.Finalizada == 0) 
           && (o.Modulo == "Rowing") 
           && (o.GrupoAlmacen!="") 
           && (o.GrupoCausante != "") 
           && (o.SUPERVISORE != "Sin Asignar") 
           && (o.CodigoEstado != "TE"))
           .OrderBy(o => o.NroObra)
           //.GroupBy(r => new
           //{
           //    r.NroObra,
           //    r.NombreObra,
           //    r.ELEMPEP,
           //    r.ObrasDocumentos
           //})
           //.Select(g => new
           //{
           //    NroObra = g.Key.NroObra,
           //    NombreObra = g.Key.NombreObra,
           //    ELEMPEP = g.Key.ELEMPEP,
           //    CantObras = g.Count(),
           //    ObrasDocumentos=g.Obras
           //})
           .ToListAsync();


            if (obras == null)
            {
                return BadRequest("No hay Obras.");
            }

            return Ok(obras);
        }
    }
}