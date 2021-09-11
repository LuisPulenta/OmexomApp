using GenericApp.Common.Helpers;
using GenericApp.Common.Requests;
using GenericApp.Common.Responses;
using GenericApp.Common.Services;
using GenericApp.Prism.Helpers;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GenericApp.Prism.ViewModels
{
    public class RegisterPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IRegexHelper _regexHelper;
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

        private MediaFile _file;

        private ImageSource _image;
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        private UserRequest _user;
        public UserRequest User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        private CountryResponse _country;
        public CountryResponse Country
        {
            get => _country;
            set
            {
                Departments = value != null ? new ObservableCollection<DepartmentResponse>(value.Departments) : null;
                Cities = new ObservableCollection<CityResponse>();
                Department = null;
                City = null;
                SetProperty(ref _country, value);
            }
        }

        private ObservableCollection<CountryResponse> _countries;
        public ObservableCollection<CountryResponse> Countries
        {
            get => _countries;
            set => SetProperty(ref _countries, value);
        }

        private DepartmentResponse _department;
        public DepartmentResponse Department
        {
            get => _department;
            set
            {
                Cities = value != null ? new ObservableCollection<CityResponse>(value.Cities) : null;
                City = null;
                SetProperty(ref _department, value);
            }
        }

        private ObservableCollection<DepartmentResponse> _departments;
        public ObservableCollection<DepartmentResponse> Departments
        {
            get => _departments;
            set => SetProperty(ref _departments, value);
        }


        private CityResponse _city;
        public CityResponse City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        private ObservableCollection<CityResponse> _cities;
        public ObservableCollection<CityResponse> Cities
        {
            get => _cities;
            set => SetProperty(ref _cities, value);
        }

        private CountryResponse _country2;
        public CountryResponse Country2
        {
            get => _country2;
            set
            {
                Teams2 = value != null ? new ObservableCollection<TeamResponse>(value.Teams) : null;
                Team2 = null;
                SetProperty(ref _country2, value);
            }
        }

        private ObservableCollection<CountryResponse> _countries2;
        public ObservableCollection<CountryResponse> Countries2
        {
            get => _countries2;
            set => SetProperty(ref _countries2, value);
        }


        private TeamResponse _team2;
        public TeamResponse Team2
        {
            get => _team2;
            set
            {
                SetProperty(ref _team2, value);
            }
        }
        private ObservableCollection<TeamResponse> _teams2;
        public ObservableCollection<TeamResponse> Teams2
        {
            get => _teams2;
            set => SetProperty(ref _teams2, value);
        }

        private DelegateCommand _registerCommand;
        public DelegateCommand RegisterCommand => _registerCommand ?? (_registerCommand = new DelegateCommand(RegisterAsync));

        private DelegateCommand _changeImageCommand;
        public DelegateCommand ChangeImageCommand => _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageAsync));


        public RegisterPageViewModel(
            INavigationService navigationService,
            IRegexHelper regexHelper,
            IApiService apiService,
            IFilesHelper filesHelper)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _regexHelper = regexHelper;
            _apiService = apiService;
            _filesHelper = filesHelper;
            Title = "Registrar Nuevo Usuario";
            Image = App.Current.Resources["UrlNoUser"].ToString();
            IsEnabled = true;
            User = new UserRequest();
            LoadCountriesAsync();
        }

        private async void LoadCountriesAsync()
        {
            IsRunning = true;
            IsEnabled = false;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Error de Conexión", "Aceptar");
                return;
            }

            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.GetListAsync<CountryResponse>(url, "api", "/Countries");
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                return;
            }

            List<CountryResponse> list = (List<CountryResponse>)response.Result;
            Countries = new ObservableCollection<CountryResponse>(list.OrderBy(c => c.Name));
            Countries2 = new ObservableCollection<CountryResponse>(list.OrderBy(c => c.Name));
        }

        private async void RegisterAsync()
        {
            bool isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "ConnectionError", "Aceptar");
                return;
            }

            byte[] imageArray = null;
            if (_file != null)
            {
                imageArray = _filesHelper.ReadFully(_file.GetStream());
            }

            User.PictureArray = imageArray;

            string url = App.Current.Resources["UrlAPI"].ToString();

            User.CityId = City.Id;
            User.FavoriteTeamId = Team2.Id;

            Response response = await _apiService.RegisterUserAsync(url, "api", "/Account/Register", User);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                if (response.Message == "Error003")
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Este usuario ya existe", "Aceptar");
                }
                else if (response.Message == "Error004")
                {
                    await App.Current.MainPage.DisplayAlert("Error", "La ciudad no es válida", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                }

                return;
            }
            await App.Current.MainPage.DisplayAlert("Ok", "El Registro fue correcto. Se le ha enviado un mail para confirmar el mismo.", "Aceptar");
            await _navigationService.GoBackAsync();
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(User.Email) || !_regexHelper.IsValidEmail(User.Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un EMail", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.Document))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese Documento", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.FirstName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un Nombre", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.LastName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un Apellido", "Aceptar");
                return false;
            }

            if (Country == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un País", "Aceptar");
                return false;
            }

            if (Department == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione una Provincia", "Aceptar");
                return false;
            }

            if (City == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione una Ciudad", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.Address))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese una Dirección", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.Phone))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un Teléfono", "Aceptar");
                return false;
            }

            if (Country2 == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un País", "Aceptar");
                return false;
            }

            if (Team2 == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un Equipo", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.Password) || User.Password?.Length < 6)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un Password", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.PasswordConfirm))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese una Conf. de Password", "Aceptar");
                return false;
            }

            if (User.Password != User.PasswordConfirm)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Password y su Confirmación deben ser iguales", "Aceptar");
                return false;
            }
            return true;
        }

        private async void ChangeImageAsync()
        {
            await CrossMedia.Current.Initialize();

            string source = await Application.Current.MainPage.DisplayActionSheet(
                "De donde quiere tomar la foto?",
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
                if (!CrossMedia.Current.IsCameraAvailable)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "La cámara no está disponible", "Aceptar");
                    return;
                }

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
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "La Galería no está disponible", "Aceptar");
                    return;
                }

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
    }
}