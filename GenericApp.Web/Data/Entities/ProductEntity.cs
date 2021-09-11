using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GenericApp.Web.Data.Entities
{
    public class ProductEntity
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Producto")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Precio")]
        public decimal Price { get; set; }

        [DisplayName("Activo")]
        public bool IsActive { get; set; }

        [DisplayName("Categoría")]
        public CategoryEntity Category { get; set; }

        [DisplayName("Latitud")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public double Latitude { get; set; }
        
        [DisplayName("Longitud")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public double Longitude { get; set; }

        [DisplayName("Estado")]
        public StateEntity State { get; set; }

        public ICollection<ProductImageEntity> ProductImages { get; set; }

        [DisplayName("N° Imágenes")]
        public int ProductImagesNumber => ProductImages == null ? 0 : ProductImages.Count;

        [Display(Name = "Imagen")]
        public string ImageFullPath => ProductImages == null || ProductImages.Count == 0
            ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Products/noimage.png"
            : ProductImages.FirstOrDefault().ImageFullPath;
    }
}