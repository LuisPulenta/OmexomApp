using GenericApp.Common.Requests;
using GenericApp.Common.Responses;
using GenericApp.Web.Data;
using GenericApp.Web.Helpers;
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
        private readonly IConverterHelper _converterHelper;

        public AccountController(DataContext dataContext, IConverterHelper converterHelper)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
        }

        [HttpPost]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUser(UsuarioRequest userRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _dataContext.SubContratistasUsrWebs.FirstOrDefaultAsync(o => o.USRLOGIN.ToLower() == userRequest.Email.ToLower());

            if (user == null)
            {
                return BadRequest("El Usuario no existe.");
            }

            var response = new UsuarioAppResponse
            {
                APELLIDONOMBRE = user.APELLIDONOMBRE,
                USRCONTRASENA = user.USRCONTRASENA,
                CODIGO = user.CODIGO,
                PERFIL = user.PERFIL,
                USRLOGIN = user.USRLOGIN,
            };

            return Ok(response);
        }


        [HttpGet]
        [Route("GetUsuarios")]
        public async Task<IActionResult> GetUsuarios()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var usuarios = await _dataContext.SubContratistasUsrWebs
           .OrderBy(o => o.APELLIDONOMBRE)
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


            if (usuarios == null)
            {
                return BadRequest("No hay Usuarios.");
            }

            return Ok(_converterHelper.ToUsuarioAppResponse(usuarios));
        }
    }
}