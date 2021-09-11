using GenericApp.Common.Models;
using System.Collections.Generic;

namespace GenericApp.Prism.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        public IEnumerable<ElementsList> GetCountries()
        {
            List<ElementsList> paymentMethods = new List<ElementsList>
        {
            new ElementsList { Id = 1, Name = "Opción 1" },
            new ElementsList { Id = 2, Name = "Opción 2" },
            new ElementsList { Id = 3, Name = "Opción 3" }
        };
            return paymentMethods;
        }
    }
}