using GenericApp.Common.Responses;
using GenericApp.Web.Data.Entities;
using GenericApp.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenericApp.Web.Helpers
{
    public interface IConverterHelper
    {
        CategoryEntity ToCategoryEntity(CategoryViewModel model, string path, bool isNew);

        CategoryViewModel ToCategoryViewModel(CategoryEntity categoryEntity);

        Task<ProductEntity> ToProductAsync(ProductViewModel model, bool isNew);

        ProductViewModel ToProductViewModel(ProductEntity product);

        CountryEntity ToCountryEntity(CountryViewModel model, string path, bool isNew);

        CountryViewModel ToCountryViewModel(CountryEntity countryEntity);

        TeamEntity ToTeamEntity(TeamViewModel model, string path,bool isNew);

        TeamViewModel ToTeamViewModel(TeamEntity product);

        ProductResponse ToProductResponse(ProductEntity productEntity);

        List<ProductResponse> ToProductResponse(List<ProductEntity> productEntities);
    }
}