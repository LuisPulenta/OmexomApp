using GenericApp.Common.Models;
using System.Collections.Generic;

namespace GenericApp.Prism.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<ElementsList> GetCountries();
    }
}