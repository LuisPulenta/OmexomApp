using GenericApp.Common.Helpers;
using GenericApp.Common.Requests;
using GenericApp.Common.Responses;
using GenericApp.Common.Services;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GenericApp.Prism.ViewModels
{
    public class ChangePasswordPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _changePasswordCommand;

        public ChangePasswordPageViewModel(INavigationService navigationService, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            IsEnabled = true;
            Title = "Cambiar Password";
        }
        public DelegateCommand ChangePasswordCommand => _changePasswordCommand ?? (_changePasswordCommand = new DelegateCommand(ChangePasswordAsync));

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string PasswordConfirm { get; set; }

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

        private async void ChangePasswordAsync()
        {
            var isValid = await ValidateDataAsync();
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
                await App.Current.MainPage.DisplayAlert("Error", "Error de conexión", "Aceptar");
                return;
            }

            ChangePasswordRequest request = new ChangePasswordRequest
            {
                NewPassword = NewPassword,
                OldPassword = CurrentPassword,
            };

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.ChangePasswordAsync(url, "api", "/Account/ChangePassword", request, token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                if (response.Message == "Error001")
                {
                    await App.Current.MainPage.DisplayAlert("Error", "El usuario no existe", "Aceptar");
                }
                else if (response.Message == "Error005")
                {
                    await App.Current.MainPage.DisplayAlert("Error", "El password actual es incorrecto", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                }

                return;
            }

            await App.Current.MainPage.DisplayAlert("Ok", "El Password fue cambiado con éxito!!", "Aceptar");
            await _navigationService.GoBackAsync();
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(CurrentPassword))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese Password actual", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(NewPassword) || NewPassword?.Length < 6)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese Nuevo Password", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(PasswordConfirm))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese Confirmación de Password", "Aceptar");
                return false;
            }

            if (NewPassword != PasswordConfirm)
            {
                await App.Current.MainPage.DisplayAlert("Error", "El Nuevo Password y su Confirmación no son iguales", "Aceptar");
                return false;
            }
            return true;
        }
    }
}