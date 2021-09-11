using GenericApp.Common.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Data.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(20, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Documento")]
        public string Document { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string LastName { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres")]
        public string Address { get; set; }

        [Display(Name = "Foto")]
        public string PicturePath { get; set; }

        [Display(Name = "Foto")]
        public string PictureFullPath => string.IsNullOrEmpty(PicturePath)
          ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Users/nouser.png"
          : $"http://keypress.serveftp.net:88/GenericAppApi{PicturePath.Substring(1)}";

        [Display(Name = "Tipo de Usuario")]
        public UserType UserType { get; set; }

        [Display(Name = "Ciudad")]
        public CityEntity City { get; set; }

        [Display(Name = "Hincha de")]
        public TeamEntity FavoriteTeam { get; set; }

        [Display(Name = "Usuario")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Usuario")]
        public string FullNameWithDocument => $"{FirstName} {LastName} - {Document}";
    }

}