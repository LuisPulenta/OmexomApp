using GenericApp.Common.Helpers;
using GenericApp.Common.Models;
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
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GenericApp.Prism.ViewModels
{
    public class EditProductPageViewModel : ViewModelBase
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

        private Position _position;

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

        private DelegateCommand _newPhotoCommand;
        public DelegateCommand NewPhotoCommand => _newPhotoCommand ?? (_newPhotoCommand = new DelegateCommand(TakePhoto));
        private DelegateCommand _deletePhotoCommand;
        public DelegateCommand DeletePhotoCommand => _deletePhotoCommand ?? (_deletePhotoCommand = new DelegateCommand(DeletePhotoAsync));
        private DelegateCommand _changeImageCommand;
        public DelegateCommand ChangeImageCommand => _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageAsync));
        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(Cancel));
        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Save));
        private DelegateCommand _getAddressCommand;
        public DelegateCommand GetAddressCommand => _getAddressCommand ?? (_getAddressCommand = new DelegateCommand(LoadSourceAsync));

        private MediaFile _file;
        public MediaFile File
        {
            get => _file;
            set => SetProperty(ref _file, value);
        }

        private DateTime _date;
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }

        public DateTime Hoy { get; set; }




        private static EditProductPageViewModel instance;
        public static EditProductPageViewModel GetInstance()
        {
            return instance;
        }


        public EditProductPageViewModel(INavigationService navigationService, IGeolocatorService geolocatorService, IApiService apiService, IFilesHelper filesHelper) : base(navigationService)
        {
            _navigationService = navigationService;
            _geolocatorService = geolocatorService;
            _apiService = apiService;
            _filesHelper = filesHelper;

            //Product = JsonConvert.DeserializeObject<ProductResponse>(Settings.Product);



            instance = this;
            IsEnabled = true;

            Title = "Editar Producto";
           

            
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

        private async void Save()
        {

            if (Category == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una Categoría.", "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(Product.Name))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el Nombre.", "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(Product.Description))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar la Descripción.", "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(Product.Price.ToString()))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el Precio.", "Aceptar");
                return;
            }

            if (Product.Latitude == 0 || Product.Longitude == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Se necesitan las Coordenadas de Geolocalización.", "Aceptar");
                return;
            }

            if (Product.State == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar el Estado.", "Aceptar");
                return;
            }

            IsRunning = true;
            BusyIndicatorTitle = "Guardando...";
            IsEnabled = false;

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

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            
            var myproduct = new ProductRequest
            {
                Id = Product.Id,
                Name = Name,
                Description = Description,
                Latitude = Latitude,
                Longitude = Longitude,
                Category = Category,
                Price=Price,
                State = State,
            };

            var response = await _apiService.PutAsync(
            url,
            "api",
            "/Products",
            Product.Id,
            myproduct,
            "bearer",
            token.Token);

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
                File = _file;
            }
            else
            {
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
            ProductResponse product = JsonConvert.DeserializeObject<ProductResponse>(Settings.Product);

            ProductImageRequest myproductImage = new ProductImageRequest
            {
                ImageArray = ImageArray,
                Product = product
            };

            var response = await _apiService.PostAsync(
            url,
            "api",
            "/ProductImages",
            myproductImage);

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

           ProductImageRequest productImageRequest = (ProductImageRequest)response.Result;


            ProductImageResponse2 productImageResponse2 = new ProductImageResponse2
            {
                Id = productImageRequest.Id,
                ImageUrl = productImageRequest.ImageUrl,
            };


            EditProductPageViewModel editProductPageViewModel = EditProductPageViewModel.GetInstance();
            editProductPageViewModel.Images.Add(new ProductImageResponse
            {
                Id = productImageResponse2.Id,
                ImagePath = productImageResponse2.ImageUrl,
                ProductId = product.Id,
            }
                ); ;


            IdPhoto = Images[Images.Count-1].Id;

            //await _navigationService.GoBackAsync();

        }

        private async void DeletePhotoAsync()
        {
            

            if (IdPhoto == 0)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Desplace las fotos hasta la foto que desea borrar",
                    "Aceptar");
                return;
            }

            if (Images.Count == 1)
            {
                IdPhoto = Images[0].Id;
            };


            var answer = await App.Current.MainPage.DisplayAlert(
                "Confirmar",
                "Está seguro de borrar esta foto?",
                "Si",
                "No");

            if (!answer)
            {
                return;
            }





            IsRunning = true;
            BusyIndicatorTitle = "Borrando...";
            IsEnabled = false;


            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
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

            var response = await _apiService.DeleteAsync(
            url,
            "api",
            "/ProductImages",
            IdPhoto,
            "bearer",
            token.Token);



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
            IdPhoto = 0;
            await App.Current.MainPage.DisplayAlert(
                "Ok",
                "Foto eliminada con éxito!!",
                "Aceptar");

            Images.Remove(Images[IdPhotoNro]);


            ProductsPageViewModel productsPageViewModel = ProductsPageViewModel.GetInstance();
            productsPageViewModel.LoadProductsAsync();
            productsPageViewModel.RefreshList();


            if (IdPhotoNro == Images.Count)
            {
                IdPhotoNro = 0;
            }



            if (Images.Count == 0)
            {
                IdPhoto = 0;
                return;
            }

            if (Images.Count == 1)
            {
                IdPhotoNro = 0;
                IdPhoto = Images[0].Id;
            }
            else
            {
                IdPhoto = Images[IdPhotoNro].Id;
            };
        }

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
            Product.Latitude = _geolocatorService.Latitude;
            Product.Longitude = _geolocatorService.Longitude;
            Geocoder geoCoder = new Geocoder();
            IEnumerable<string> sources = await geoCoder.GetAddressesForPositionAsync(_position);
            List<string> addresses = new List<string>(sources);

            if (addresses.Count > 1)
            {
                Description = addresses[0];
            }
            IsEnabled = true;
        }
    }
}