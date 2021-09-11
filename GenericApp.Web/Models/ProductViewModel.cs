using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using GenericApp.Web.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Models
{
    public class ProductViewModel : ProductEntity
    {
        [Display(Name = "Categoría")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría.")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int CategoryId { get; set; }

        [Display(Name = "Estado")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado.")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int StateId { get; set; }

        public IFormFile ImageFile { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        [Display(Name = "Precio")]
        [MaxLength(12)]
        [RegularExpression(@"^\d+([\.\,]?\d+)?$", ErrorMessage = "Use sólo números o . o , para poner decimales")]
        [Required]
        public string PriceString { get; set; }
    }
}