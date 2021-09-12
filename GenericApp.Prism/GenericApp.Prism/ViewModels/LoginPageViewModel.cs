using GenericApp.Common.Data;
using GenericApp.Common.Helpers;
using GenericApp.Common.Responses;
using GenericApp.Common.Services;
using GenericApp.Prism.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Essentials;

namespace GenericApp.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private bool _isRunning;
        private bool _isEnabled;
        private string _password;
        private DelegateCommand _loginCommand;
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;

        private string _pageReturn;

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(LoginAsync));

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

        

        
        

        public LoginPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Login";
            IsEnabled = true;
            LoadUsers();
            UsuarioAppResponseRepository repository = new UsuarioAppResponseRepository();
            var Users = repository.GetAll();
            //Email = "AVASILE";
            //Password = "AVA123";
        }

        public List<UsuarioAppResponse> Users { get; set; }
        public List<UsuarioAppResponse> MyUsers { get; set; }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("pageReturn"))
            {
                _pageReturn = parameters.GetValue<string>("pageReturn");
            }
        }

        public async void LoadUsers()
        {
            //Chequea si la Tabla Users Local tiene datos y si hay Internet
            Users = await App.Database.GetItemsAsync();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet && Users.Count==0)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "No hay Internet y no hay Tabla Usuarios Local para chequear el Login",
                    "Aceptar");
                return;
            }

            if (Connectivity.NetworkAccess != NetworkAccess.Internet && Users.Count > 0)
            {
                return;
            }

            //Si hay Internet borra la Tabla Users Local y la llena de nuevo desde el Servidor
            //Borra la Tabla Users Local
            if (Users.Count > 0)
            {
                foreach (UsuarioAppResponse user in Users)
                {
                    await App.Database.DeleteItemAsync(user);
                }
            }

            //Trae la Tabla Usuarios desde el Servidor
            var controller = string.Format("/Account/GetUsuarios");
            var url = App.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.GetUsuarios(
                url,
                "api",
                controller);
            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Problema para recuperar datos.", "Aceptar");
                return;
            }
            MyUsers = (List<UsuarioAppResponse>)response.Result;
            //Llena la Tabla Users Local
            foreach (UsuarioAppResponse user in MyUsers)
            {
                await App.Database.SaveItemAsync(user);
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

            Users = await App.Database.GetItemsAsync();
            int MyID = 0;
            foreach (UsuarioAppResponse user in Users)
            {
                if (user.USRLOGIN.ToLower()==Email.ToLower())
                {
                    MyID = user.ID;
                    break;
                }
            }

            var response = await App.Database.GetItemAsync(MyID);

            if (response == null)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert("Error", "Usuario o password incorrecto.", "Aceptar");
                //Password = string.Empty;
                return;
            }

            //Verificar Password
            if (!(response.USRCONTRASENA.ToLower() == Password.ToLower()))
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Usuario o clave incorrecta.", "Aceptar");
                return;
            }

            Settings.UsuarioLogueado = JsonConvert.SerializeObject(response);

            await _navigationService.NavigateAsync("/GenericAppMasterDetailPage/NavigationPage/HomePage");

        }
    }
}