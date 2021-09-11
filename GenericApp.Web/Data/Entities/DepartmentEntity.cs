using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenericApp.Web.Data.Entities
{
    public class DepartmentEntity
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Provincia")]
        public string Name { get; set; }

        public ICollection<CityEntity> Cities { get; set; }

        [DisplayName("N° Ciudades")]
        public int CitiesNumber => Cities == null ? 0 : Cities.Count;

        [JsonIgnore]
        [NotMapped]
        public int IdCountry { get; set; }

        [JsonIgnore]
        public CountryEntity Country { get; set; }
    }
}