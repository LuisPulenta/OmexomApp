using System.ComponentModel.DataAnnotations;

namespace GenericApp.Common.Requests
{
    public class EmailRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}