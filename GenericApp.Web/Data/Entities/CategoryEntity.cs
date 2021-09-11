using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Data.Entities
{
    public class CategoryEntity
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Categoría")]
        public string Name { get; set; }

        [Display(Name = "Imagen")]
        public string ImagePath { get; set; }

        [Display(Name = "Imagen")]
        public string ImageFullPath => string.IsNullOrEmpty(ImagePath)
           ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Categories/noimage.png"
           : $"http://keypress.serveftp.net:88/GenericAppApi{ImagePath.Substring(1)}";
    }
}