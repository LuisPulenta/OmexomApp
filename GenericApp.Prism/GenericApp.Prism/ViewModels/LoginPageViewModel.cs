using Newtonsoft.Json;
using GenericApp.Common.Requests;
using GenericApp.Common.Responses;
using GenericApp.Common.Services;
using GenericApp.Prism.Views;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Essentials;
using GenericApp.Common.Helpers;
using System;

namespace GenericApp.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private bool _isRunning;
        private bool _isEnabled;
        private string _password;
        private DelegateCommand _loginCommand;
        private DelegateCommand _registerCommand;
        private DelegateCommand _forgotPasswordCommand;
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;

        private string _pageReturn;

        public LoginPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Login";
            IsEnabled = true;
            //Email = "AVASILE";
            //Password = "AVA123";
        }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(LoginAsync));

        public DelegateCommand RegisterCommand => _registerCommand ?? (_registerCommand = new DelegateCommand(RegisterAsync));

        public DelegateCommand ForgotPasswordCommand => _forgotPasswordCommand ?? (_forgotPasswordCommand = new DelegateCommand(ForgotPasswordAsync));

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public string Email { get; set; }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("pageReturn"))
            {
                _pageReturn = parameters.GetValue<string>("pageReturn");
            }
        }



        private async void LoginAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un Usuario",
                    "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar una Clave",
                    "Aceptar");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Error de conexión",
                    "Aceptar");
                return;
            }

            string url = App.Current.Resources["UrlAPI"].ToString();

            //*******************************************************************************


            TokenResponse token = new TokenResponse();
            token.Token = "123";
            token.Expiration = DateTime.Now;
            token.User = new UserResponse();
            token.User.Address = "";
            token.User.City = new CityResponse();
            token.User.Document = "";
            token.User.Email = "";
            token.User.FavoriteTeam = new TeamResponse();
            token.User.FirstName = "Luis";
            token.User.Id = "1";
            token.User.LastName = "Nuñez";
            token.User.PhoneNumber = "123";
            token.User.PicturePath = null;
            token.User.UserType = Common.Enums.UserType.Admin;





            Settings.Token = JsonConvert.SerializeObject(token);
            Settings.IsLogin = true;



            //*******************************************************************************




            var response = await _apiService.GetUserByEmailAsync(url, "api", "/Account/GetUserByEmail", Email, Password);

            if (!response.IsSuccess)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert("Error", "Usuario o password incorrecto.", "Aceptar");
                //Password = string.Empty;
                return;
            }

            //Verificar Password
            if (!(response.Result.Contrasena.ToLower() == Password.ToLower()))
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Usuario o clave incorrecta.", "Aceptar");
                return;
            }
            //Verificar Usuario Habilitado
            if (response.Result.AutorWOM != 1)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Usuario no habilitado.", "Aceptar");
                return;
            }

            if (response.Result.Estado != 1)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Usuario no habilitado.", "Aceptar");
                return;
            }

            Settings.UsuarioLogueado = JsonConvert.SerializeObject(response.Result);

            await _navigationService.NavigateAsync("/GenericAppMasterDetailPage/NavigationPage/HomePage");


            //await _navigationService.NavigateAsync($"/{nameof(OnSaleMasterDetailPage)}/NavigationPage/{nameof(ProductsPage)}");

            //if (string.IsNullOrEmpty(_pageReturn))
            //{
            //    await _navigationService.NavigateAsync($"/{nameof(GenericAppMasterDetailPage)}/NavigationPage/{nameof(HomePage)}");
            //}
            //else
            //{
            //    await _navigationService.NavigateAsync($"/{nameof(GenericAppMasterDetailPage)}/NavigationPage/{_pageReturn}");
            //}
            //Password = string.Empty;
        }

        private async void ForgotPasswordAsync()
        {
            var parameters = new NavigationParameters();
            parameters.Add("email", Email);
            await _navigationService.NavigateAsync(nameof(RecoverPasswordPage), parameters);
        }

        private async void RegisterAsync()
        {
            await _navigationService.NavigateAsync(nameof(RegisterPage));
        }
    }
}