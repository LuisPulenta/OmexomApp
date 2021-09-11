using GenericApp.Common.Helpers;
using GenericApp.Common.Requests;
using GenericApp.Common.Responses;
using GenericApp.Common.Services;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GenericApp.Prism.ViewModels
{
    public class AddProductPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IGeolocatorService _geolocatorService;
        private readonly IApiService _apiService;
        private readonly IFilesHelper _filesHelper;

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }
        
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
        
        private MediaFile _file;
        public MediaFile File
        {
            get => _file;
            set => SetProperty(ref _file, value);
        }

        private Position _position;

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private StateResponse _state;
        public StateResponse State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        private CategoryResponse _category;
        public CategoryResponse Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        private ObservableCollection<CategoryResponse> _categories;
        public ObservableCollection<CategoryResponse> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        private ObservableCollection<StateResponse> _states;
        public ObservableCollection<StateResponse> States
        {
            get => _states;
            set => SetProperty(ref _states, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        private double _latitude;
        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        private double _longitude;
        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
        }

        private DelegateCommand _takePhotoCommand;
        public DelegateCommand TakePhotoCommand => _takePhotoCommand ?? (_takePhotoCommand = new DelegateCommand(TakePhoto));
        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(Cancel));
        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Save));
        private DelegateCommand _getAddressCommand;
        public DelegateCommand GetAddressCommand => _getAddressCommand ?? (_getAddressCommand = new DelegateCommand(LoadSourceAsync));

        public AddProductPageViewModel(INavigationService navigationService, IGeolocatorService geolocatorService, IApiService apiService, IFilesHelper filesHelper) : base(navigationService)
        {
            _navigationService = navigationService;
            _geolocatorService = geolocatorService;
            _apiService = apiService;
            _filesHelper = filesHelper;

            IsEnabled = true;
            Title = "Agregar Nuevo Producto";
            instance = this;
            ImageSource = "noimage.png";

            LoadCategories();
            LoadStates();
        }

        #region Singleton

        private static AddProductPageViewModel instance;
        public static AddProductPageViewModel GetInstance()
        {
            return instance;
        }
        #endregion

        private async void LoadSourceAsync()
        {
            IsEnabled = false;
            await _geolocatorService.GetLocationAsync();

            if (_geolocatorService.Latitude == 0 && _geolocatorService.Longitude == 0)
            {
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Error de Geolocalización",
                    "Aceptar");
                //await _navigationService.GoBackAsync();
                return;
            }

            _position = new Position(_geolocatorService.Latitude, _geolocatorService.Longitude);
            Latitude = _geolocatorService.Latitude;
            Longitude = _geolocatorService.Longitude;
            Geocoder geoCoder = new Geocoder();
            IEnumerable<string> sources = await geoCoder.GetAddressesForPositionAsync(_position);
            List<string> addresses = new List<string>(sources);

            if (addresses.Count > 1)
            {
                Description = addresses[0];
            }
            IsEnabled = true;
        }


        private async void LoadCategories()
        {
            string url = App.Current.Resources["UrlAPI"].ToString();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Revise su conexión a Internet",
                    "Aceptar");
                return;
            }

            Response response = await _apiService.GetListAsync<CategoryResponse>(url, "api", "/Categories");

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }
            List<CategoryResponse> list = (List<CategoryResponse>)response.Result;
            Categories = new ObservableCollection<CategoryResponse>(list.OrderBy(t => t.Name));
        }

        private async void LoadStates()
        {
            string url = App.Current.Resources["UrlAPI"].ToString();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Revise su conexión a Internet",
                    "Aceptar");
                return;
            }

            Response response = await _apiService.GetListAsync<StateResponse>(url, "api", "/States");

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }
            List<StateResponse> list = (List<StateResponse>)response.Result;
            States = new ObservableCollection<StateResponse>(list.OrderBy(t => t.Id));
        }

        private async void Save()
        {
            if (Category == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una Categoría.", "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(Name))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el Nombre.", "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(Description))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar la Descripción.", "Aceptar");
                return;
            }

            if (Latitude == 0 || Longitude == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Se necesitan las Coordenadas de Geolocalización.", "Aceptar");
                return;
            }

            if (State == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar el Estado.", "Aceptar");
                return;
            }

            if (_file == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe cargar la foto del Producto.", "Aceptar");
                return;
            }


            IsRunning = true;
            IsEnabled = false;

            string url = App.Current.Resources["UrlAPI"].ToString();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = true;
                IsEnabled = false;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Revise su conexión a Internet",
                    "Aceptar");
                return;
            }


            byte[] ImageArray = null;
            if (File != null)
            {
                ImageArray = _filesHelper.ReadFully(File.GetStream());
                File.Dispose();
            }

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            UserResponse user = token.User;

            ProductRequest myproduct = new ProductRequest
            {
                Name = Name,
                Description =Description,
                Price=Price,
                Category=Category,
                Latitude = Latitude,
                Longitude = Longitude,
                PhotoArray = ImageArray,
                State = State,
            };

            ResponseT<object> response = await _apiService.PostAsync(
            url,
            "api",
            "/Products",
            myproduct);



            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;

                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }

            IsRunning = false;
            IsEnabled = true;

            await App.Current.MainPage.DisplayAlert(
                "Ok",
                "Guardado con éxito!!",
                "Aceptar");

            ProductsPageViewModel productsPageViewModel = ProductsPageViewModel.GetInstance();
            productsPageViewModel.LoadProductsAsync();
            productsPageViewModel.RefreshList();

            await _navigationService.GoBackAsync();
        }

        private async void Cancel()
        {
            await _navigationService.GoBackAsync();
        }

        private async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();
            _file = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg",
                    PhotoSize = PhotoSize.Small,
                }
            );
            if (_file != null)
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = _file.GetStream();
                    return stream;
                });
            }
            IsRunning = false;
        }
    }
}