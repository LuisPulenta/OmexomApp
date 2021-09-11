using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Data.Entities
{
    public class CountryEntity
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} debe contener menos de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "País")]
        public string Name { get; set; }

        [Display(Name = "Bandera")]
        public string FlagImagePath { get; set; }

        public ICollection<DepartmentEntity> Departments { get; set; }

        public ICollection<TeamEntity> Teams { get; set; }

        [DisplayName("N° Provincias")]
        public int DepartmentsNumber => Departments == null ? 0 : Departments.Count;

        [DisplayName("N° Equipos")]
        public int TeamsNumber => Teams == null ? 0 : Teams.Count;

        public string FlagImageFullPath => string.IsNullOrEmpty(FlagImagePath)
          ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Flags/noimage.png"
          : $"http://keypress.serveftp.net:88/GenericAppApi{FlagImagePath.Substring(1)}";
    }
}