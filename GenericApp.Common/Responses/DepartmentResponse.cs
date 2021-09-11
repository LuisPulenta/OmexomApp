using System.Collections.Generic;

namespace GenericApp.Common.Responses
{
    public class DepartmentResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CountryResponse Country { get; set; }

        public ICollection<CityResponse> Cities { get; set; }
    }
}