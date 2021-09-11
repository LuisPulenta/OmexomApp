using GenericApp.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Models
{
    public class TeamViewModel : TeamEntity
    {
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [Display(Name = "Imagen")]
        public IFormFile ImageFile { get; set; }
    }
}