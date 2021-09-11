using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [EmailAddress]
        public string Username { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MinLength(6, ErrorMessage = "El campo {0} debe tener al menos {1} caracteres.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}