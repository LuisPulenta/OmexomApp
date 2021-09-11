using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Models
{
    public class ChangePasswordViewModel
    {
        [Display(Name = "Password actual")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
        public string OldPassword { get; set; }

        [Display(Name = "Nuevo password")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirmar Password")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
        [Compare("NewPassword")]
        public string Confirm { get; set; }
    }
}