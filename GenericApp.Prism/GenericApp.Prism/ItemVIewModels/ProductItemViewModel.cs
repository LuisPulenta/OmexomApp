using Newtonsoft.Json;
using GenericApp.Common.Helpers;
using GenericApp.Common.Responses;
using GenericApp.Prism.Views;
using Prism.Commands;
using Prism.Navigation;
using System;
using GenericApp.Common.Services;
using GenericApp.Prism.ViewModels;

namespace GenericApp.Prism.ItemViewModels
{
    public class ProductItemViewModel : ProductResponse
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;

        private DelegateCommand _editProductCommand;
        public DelegateCommand EditProductCommand => _editProductCommand ?? (_editProductCommand = new DelegateCommand(EditProductAsync));

        private DelegateCommand _deleteProductCommand;
        public DelegateCommand DeleteProductCommand => _deleteProductCommand ?? (_deleteProductCommand = new DelegateCommand(DeleteProductAsync));

        private DelegateCommand _viewProductCommand;
        public DelegateCommand ViewProductCommand => _viewProductCommand ?? (_viewProductCommand = new DelegateCommand(ViewProductAsync));

        public ProductItemViewModel(INavigationService navigationService, IApiService apiService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
        }

        private async void EditProductAsync()
        {
            NavigationParameters parameters = new NavigationParameters
                {
                    { "product", this }
                };
            Settings.Product = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync(nameof(EditProductPage), parameters);
        }

        private async void DeleteProductAsync()
        {
            var answer = await App.Current.MainPage.DisplayAlert(
              "Confirmar",
              "¿Está seguro de borrar este Producto?",
              "Si",
              "No");

            if (!answer)
            {
                return;
            }

            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.DeleteAsync(
               url,
               "api",
               "/Products",
               this.Id);

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "No se pudo borrar", //response.Message,
                    "Aceptar");
                return;
            }
            ProductsPageViewModel.GetInstance().LoadProductsAsync();
        }

        private async void ViewProductAsync()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "product", this }
            };
            Settings.Product = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync("ViewProductPage", parameters);
        }
    }
}