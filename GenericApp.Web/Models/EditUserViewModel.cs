using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Documento")]
        [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Document { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string LastName { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string Address { get; set; }

        [Display(Name = "Teléfono")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Foto")]
        public string PicturePath { get; set; }

        [Display(Name = "Foto")]
        public string PictureFullPath => string.IsNullOrEmpty(PicturePath)
            ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Users/nouser.png"
           : $"http://keypress.serveftp.net:88/GenericAppApi{PicturePath.Substring(1)}";

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "País")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un país")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Provincia")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una provincia")]
        public int DepartmentId { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "País")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un país")]
        public int CountryTeamId { get; set; }

        public IEnumerable<SelectListItem> Teams { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Ciudad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una ciudad")]
        public int CityId { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Equipo")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un equipo")]
        public int TeamId { get; set; }

        [Display(Name = "Tipo Usuario")]
        public string UserType { get; set; }

        public IEnumerable<SelectListItem> UserTypes { get; set; }
    }
}