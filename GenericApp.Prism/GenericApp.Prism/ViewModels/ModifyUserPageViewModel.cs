using Newtonsoft.Json;
using GenericApp.Common.Helpers;
using GenericApp.Common.Requests;
using GenericApp.Common.Responses;
using GenericApp.Common.Services;
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
using ImageSource = Xamarin.Forms.ImageSource;
using GenericApp.Prism.Views;

namespace GenericApp.Prism.ViewModels
{
    public class ModifyUserPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
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

        private UserResponse _user;
        public UserResponse User
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
             
        private DelegateCommand _changeImageCommand;
        public DelegateCommand ChangeImageCommand => _changeImageCommand ??
            (_changeImageCommand = new DelegateCommand(ChangeImageAsync));
        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand => _saveCommand ??
            (_saveCommand = new DelegateCommand(SaveAsync));
        private DelegateCommand _changePasswordCommand;
        public DelegateCommand ChangePasswordCommand => _changePasswordCommand ??
            (_changePasswordCommand = new DelegateCommand(ChangePasswordAsync));

        public ModifyUserPageViewModel(
            INavigationService navigationService,
            IApiService apiService,
            IFilesHelper filesHelper)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _filesHelper = filesHelper;
            Title = "Modificar Usuario";
            IsEnabled = true;
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            User = token.User;
            Image = User.PictureFullPath;
            LoadCountriesAsync();
        }
       
        public async void LoadCountriesAsync()
        {
            IsRunning = true;
            IsEnabled = false;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Connection Error",
                    "Accept");
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
            LoadCurrentCountryDepartmentCity();
            LoadCurrentCountryTeam();
        }

        private void LoadCurrentCountryDepartmentCity()
        {
            Country = Countries.FirstOrDefault(c => c.Departments.FirstOrDefault(d => d.Cities.FirstOrDefault(ci => ci.Id == User.City.Id) != null) != null);
            Department = Country.Departments.FirstOrDefault(d => d.Cities.FirstOrDefault(c => c.Id == User.City.Id) != null);
            City = Department.Cities.FirstOrDefault(c => c.Id == User.City.Id);
        }

        private void LoadCurrentCountryTeam()
        {
            Country2 = Countries.FirstOrDefault(c => c.Teams.FirstOrDefault(ci => ci.Id == User.FavoriteTeam.Id) != null);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            User = token.User;
            Team2 = Country2.Teams.FirstOrDefault(c => c.Id == User.FavoriteTeam.Id);
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
                Image = Xamarin.Forms.ImageSource.FromStream(() =>
                {
                    System.IO.Stream stream = _file.GetStream();
                    return stream;
                });
            }
        }

        private async void SaveAsync()
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
                await App.Current.MainPage.DisplayAlert("Error", "Connection Error", "Accept");
                return;
            }

            byte[] imageArray = null;
            if (_file != null)
            {
                imageArray = _filesHelper.ReadFully(_file.GetStream());
            }

            UserRequest request = new UserRequest
            {
                Address = User.Address,
                CityId = City.Id,
                FavoriteTeamId = Team2.Id,
                Document = User.Document,
                Email = User.Email,
                FirstName = User.FirstName,
                PictureArray = imageArray,
                LastName = User.LastName,
                Password = "123456", // Doen't matter, it's only to pass the data annotation
                Phone = User.PhoneNumber,
                //FavoriteTeamId
            };

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.ModifyUserAsync(url, "api", "/Account", request, token.Token);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                if (response.Message == "Error001")
                {
                    await App.Current.MainPage.DisplayAlert("Error", "El Usuario no existe", "Aceptar");
                }
                else if (response.Message == "Error004")
                {
                    await App.Current.MainPage.DisplayAlert("Error", "La ciudad no existe", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                }

                return;
            }

            UserResponse updatedUser = (UserResponse)response.Result;
            token.User = updatedUser;
            Settings.Token = JsonConvert.SerializeObject(token);
            GenericAppMasterDetailPageViewModel.GetInstance().LoadUser();
            await App.Current.MainPage.DisplayAlert("Ok", "El Usuario fue actualizado con éxito.", "Aceptar");
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(User.Document))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un Documento", "Aceptar");
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
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un país", "Accept");
                return false;
            }

            if (Department == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un departamento", "Aceptar");
                return false;
            }

            if (City == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione una ciudad", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.Address))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese una Dirección", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.PhoneNumber))
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
            return true;
        }

        private async void ChangePasswordAsync()
        {
            await _navigationService.NavigateAsync(nameof(ChangePasswordPage));
        }
    }
}