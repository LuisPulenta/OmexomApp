using GenericApp.Common.Responses;
using GenericApp.Common.Services;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;
using GenericApp.Prism.ItemViewModels;

namespace GenericApp.Prism.ViewModels
{
    public class ProductsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        
        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        private List<ProductResponse> _myProducts;
        
        private ObservableCollection<ProductItemViewModel> _products;
        public ObservableCollection<ProductItemViewModel> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        private string _search;
        public string Search
        {
            get => _search;
            set
            {
                SetProperty(ref _search, value);
                RefreshList();
            }
        }
        
        private DelegateCommand _addProductCommand;
        public DelegateCommand AddProductCommand => _addProductCommand ?? (_addProductCommand = new DelegateCommand(AddProduct));
        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand => _searchCommand ?? (_searchCommand = new DelegateCommand(RefreshList));
        private DelegateCommand _productsMapCommand;
        public DelegateCommand ProductsMapCommand => _productsMapCommand ?? (_productsMapCommand = new DelegateCommand(ProductsMap));

        private static ProductsPageViewModel _instance;
        public static ProductsPageViewModel GetInstance()
        {
            return _instance;
        }

        public ProductsPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _instance = this;
            Title = "Productos";
            LoadProductsAsync();
        }

        

        public async void LoadProductsAsync()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Error de conexión.", "Aceptar");
                return;
            }
            IsRunning = true;
            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.GetListAsync<ProductResponse>(
                url,
                "api",
                "/Products");
            IsRunning = false;
            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }
            _myProducts = (List<ProductResponse>)response.Result;
            RefreshList();
        }

        public void RefreshList()
        {
            IsRefreshing = true;
            if (string.IsNullOrEmpty(Search))
            {
                Products = new ObservableCollection<ProductItemViewModel>(_myProducts.Select(p => new ProductItemViewModel(_navigationService,_apiService)
                {
                    Category = p.Category,
                    Description = p.Description,
                    Id = p.Id,
                    IsActive = p.IsActive,
                    Latitude=p.Latitude,
                    Longitude=p.Longitude,
                    Name = p.Name,
                    Price = p.Price,
                    ProductImages = p.ProductImages,
                    State=p.State
                })
    .ToList());

            }
            else
            {
                Products = new ObservableCollection<ProductItemViewModel>(_myProducts.Select(p => new ProductItemViewModel(_navigationService,_apiService)
                {
                    Category = p.Category,
                    Description = p.Description,
                    Id = p.Id,
                    IsActive = p.IsActive,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Name = p.Name,
                    Price = p.Price,
                    ProductImages = p.ProductImages,
                    State = p.State
                })
    .Where(p => 
                    p.Name.ToLower().Contains(Search.ToLower())
                    ||
                    p.State.Name.ToLower().Contains(Search.ToLower())
          )
    .ToList());
            }
        }

        private async void ProductsMap()
        {
            await _navigationService.NavigateAsync("ProductsMapPage");
        }

        public async void CerrarMapa()
        {
            await _navigationService.GoBackAsync();
        }

        private async void AddProduct()
        {
            await _navigationService.NavigateAsync("AddProductPage");
        }
    }
}