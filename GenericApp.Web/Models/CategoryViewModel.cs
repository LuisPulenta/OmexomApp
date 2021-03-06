using GenericApp.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Models
{
    public class CategoryViewModel:CategoryEntity
    {
        [Display(Name = "Imagen")]
        public IFormFile ImageFile { get; set; }
    }
}