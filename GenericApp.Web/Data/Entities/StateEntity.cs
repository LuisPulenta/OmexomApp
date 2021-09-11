using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Data.Entities
{
    public class StateEntity
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Estado")]
        public string Name { get; set; }
    }
}