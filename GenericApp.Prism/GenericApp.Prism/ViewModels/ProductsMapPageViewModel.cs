using GenericApp.Common.Services;
using Prism.Navigation;

namespace GenericApp.Prism.ViewModels
{
    public class ProductsMapPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private static ProductsMapPageViewModel _instance;

        public static ProductsMapPageViewModel GetInstance()
        {
            return _instance;
        }

        public ProductsMapPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _instance = this;
            Title = "Mapa";
        }

        public async void CerrarMapa()
        {
            await _navigationService.GoBackAsync();
        }
    }
}