using GenericApp.Common.Helpers;
using GenericApp.Common.Responses;
using GenericApp.Common.Services;
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
    public class ViewProductPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IGeolocatorService _geolocatorService;
        private readonly IApiService _apiService;
        private readonly IFilesHelper _filesHelper;

        private bool _isRunning;
        public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }

        private string _busyIndicatorTitle;
        public string BusyIndicatorTitle { get => _busyIndicatorTitle; set => SetProperty(ref _busyIndicatorTitle, value); }

        private bool _isEnabled;
        public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

        private int _idPhoto;
        public int IdPhoto { get => _idPhoto; set => SetProperty(ref _idPhoto, value); }

        private ProductResponse _product;
        public ProductResponse Product { get => _product; set => SetProperty(ref _product, value); }

        private ImageSource _image;
        public ImageSource Image { get => _image; set => SetProperty(ref _image, value); }

        private ImageSource _imageOriginal;
        public ImageSource ImageOriginal { get => _imageOriginal; set => SetProperty(ref _imageOriginal, value); }

        private int _idPhotoNro;
        public int IdPhotoNro { get => _idPhotoNro; set => SetProperty(ref _idPhotoNro, value); }

        private StateResponse _state;
        public StateResponse State
        {
            get => _state;
            set => SetProperty(ref _state, value);
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

        private ObservableCollection<StateResponse> _states;
        public ObservableCollection<StateResponse> States
        {
            get => _states;
            set => SetProperty(ref _states, value);
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

        private ObservableCollection<ProductImageResponse> _images;
        public ObservableCollection<ProductImageResponse> Images
        {
            get => _images;
            set => SetProperty(ref _images, value);
        }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        
        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(Cancel));

        private MediaFile _file;
        public MediaFile File
        {
            get => _file;
            set => SetProperty(ref _file, value);
        }

        private DateTime _date;
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }

        public DateTime Hoy { get; set; }




        private static ViewProductPageViewModel instance;
        public static ViewProductPageViewModel GetInstance()
        {
            return instance;
        }

        public ViewProductPageViewModel(INavigationService navigationService, IGeolocatorService geolocatorService, IApiService apiService, IFilesHelper filesHelper) : base(navigationService)
        {
            _navigationService = navigationService;
            _geolocatorService = geolocatorService;
            _apiService = apiService;
            _filesHelper = filesHelper;

            instance = this;
            IsEnabled = true;

            Title = "Ver Producto";
        }

        private void LoadCurrentState()
        {
            State = States.FirstOrDefault(c => c.Id == Product.State.Id);
        }

        private void LoadCurrentCategory()
        {
            Category = Categories.FirstOrDefault(c => c.Id == Product.Category.Id);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("product"))
            {
                Product = parameters.GetValue<ProductResponse>("product");
                Title = Product.Name;
                Images = new ObservableCollection<ProductImageResponse>(Product.ProductImages);
                IdPhoto = 0;
                if (Images.Count > 0)
                {
                    IdPhoto = Images[0].Id;
                };
                Name = Product.Name;
                Description = Product.Description;
                Price = Product.Price;
                Latitude = Product.Latitude;
                Longitude = Product.Longitude;
                LoadStates();
                LoadCategories();

            }
        }

        public async void LoadStates()
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
            LoadCurrentState();
        }

        public async void LoadCategories()
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
            LoadCurrentCategory();
        }

        private async void ChangeImageAsync()
        {
            await CrossMedia.Current.Initialize();

            string source = await Application.Current.MainPage.DisplayActionSheet(
                 "¿De dónde quiere tomar la foto?:",
                "Cancelar",
                null,
                "Galería",
                "Cámara");

            if (source == "Cancelar")
            {
                _file = null;
                return;
            }

            if (source == "Cámara")
            {
                _file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                _file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (_file != null)
            {
                Image = ImageSource.FromStream(() =>
                {
                    System.IO.Stream stream = _file.GetStream();
                    return stream;
                });
            }
        }

        private async void Cancel()
        {
            await _navigationService.GoBackAsync();
        }
    }
}