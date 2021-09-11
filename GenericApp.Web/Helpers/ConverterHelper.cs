using GenericApp.Common.Responses;
using GenericApp.Web.Data;
using GenericApp.Web.Data.Entities;
using GenericApp.Web.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApp.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(DataContext context, ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }

        public CategoryEntity ToCategoryEntity(CategoryViewModel model, string path, bool isNew)
        {
            return new CategoryEntity
            {
                Id = isNew ? 0 : model.Id,
                ImagePath = path,
                Name = model.Name
            };
        }

        public CategoryViewModel ToCategoryViewModel(CategoryEntity categoryEntity)
        {
            return new CategoryViewModel
            {
                Id = categoryEntity.Id,
                ImagePath = categoryEntity.ImagePath,
                Name = categoryEntity.Name
            };
        }

        public async Task<ProductEntity> ToProductAsync(ProductViewModel model, bool isNew)
        {
            return new ProductEntity
            {
                Category = await _context.Categories.FindAsync(model.CategoryId),
                Description = model.Description,
                Id = isNew ? 0 : model.Id,
                IsActive = model.IsActive,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Name = model.Name,
                Price = ToPrice(model.PriceString),
                ProductImages = model.ProductImages,
                State = await _context.States.FindAsync(model.StateId),
            };
        }

        private decimal ToPrice(string priceString)
        {
            string nds = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (nds == ".")
            {
                priceString = priceString.Replace(',', '.');

            }
            else
            {
                priceString = priceString.Replace('.', ',');
            }

            return decimal.Parse(priceString);
        }

        public ProductViewModel ToProductViewModel(ProductEntity product)
        {
            return new ProductViewModel
            {
                Categories = _combosHelper.GetComboCategories(),
                Category = product.Category,
                CategoryId = product.Category.Id,
                Description = product.Description,
                Id = product.Id,
                IsActive = product.IsActive,
                Latitude = product.Latitude,
                Longitude = product.Longitude,
                Name = product.Name,
                PriceString = $"{product.Price}",
                ProductImages = product.ProductImages,
                States = _combosHelper.GetComboStates(),
                State = product.State,
                StateId = product.State.Id
            };
        }

        public CountryEntity ToCountryEntity(CountryViewModel model, string path, bool isNew)
        {
            return new CountryEntity
            {
                Id = isNew ? 0 : model.Id,
                FlagImagePath = path,
                Name = model.Name
            };
        }

        public CountryViewModel ToCountryViewModel(CountryEntity countryEntity)
        {
            return new CountryViewModel
            {
                Id = countryEntity.Id,
                FlagImagePath = countryEntity.FlagImagePath,
                Name = countryEntity.Name
            };
        }



        public TeamEntity ToTeamEntity(TeamViewModel model, string path, bool isNew)
        {
            return new TeamEntity
            {
                Id = isNew ? 0 : model.Id,
                LogoImagePath = path,
                Name = model.Name,
                IdCountry = model.IdCountry,
                Country = model.Country,
            };
        }


        public TeamViewModel ToTeamViewModel(TeamEntity team)
        {
            return new TeamViewModel
            {
                Countries = _combosHelper.GetComboCountries(),
                Country = team.Country,
                CountryId = team.Country.Id,
                Id = team.Id,
                Name = team.Name,
                LogoImagePath = team.LogoImagePath,
            };
        }



        public  ProductResponse ToProductResponse(ProductEntity productEntity)
        {
            return new ProductResponse
            {
                Id = productEntity.Id,
                IsActive = productEntity.IsActive,
                Description = productEntity.Description,
                Name = productEntity.Name,
                Price= productEntity.Price,
                Latitude = productEntity.Latitude,
                Longitude = productEntity.Longitude,
                Category = new CategoryResponse
                {
                    Id = productEntity.Category.Id,
                    Name = productEntity.Category.Name,
                    ImagePath = productEntity.Category.ImagePath,
                },
                State = new StateResponse {
                    Id = productEntity.State.Id,
                    Name = productEntity.State.Name,
                },
                ProductImages = productEntity.ProductImages?.Select(g => new ProductImageResponse
                {
                    Id = g.Id,
                    ImagePath=g.ImagePath,
                    ProductId=g.Product.Id
                }).ToList()
            };
        }

        public List<ProductResponse> ToProductResponse(List<ProductEntity> productEntities)
        {
            List<ProductResponse> list = new List<ProductResponse>();
            foreach (ProductEntity productEntity in productEntities)
            {
                list.Add(ToProductResponse(productEntity));
            }
            return list;
        }
    }
}