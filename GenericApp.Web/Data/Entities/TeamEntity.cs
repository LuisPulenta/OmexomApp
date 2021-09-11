using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenericApp.Web.Data.Entities
{
    public class TeamEntity
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Equipo")]
        public string Name { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int IdCountry { get; set; }

        [JsonIgnore]
        public CountryEntity Country { get; set; }


        [Display(Name = "Logo")]
        public string LogoImagePath { get; set; }

        public string LogoImageFullPath => string.IsNullOrEmpty(LogoImagePath)
          ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Logos/noimage.png"
          : $"http://keypress.serveftp.net:88/GenericAppApi{LogoImagePath.Substring(1)}";
    }
}