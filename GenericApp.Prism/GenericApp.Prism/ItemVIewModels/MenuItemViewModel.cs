using GenericApp.Common.Helpers;
using GenericApp.Common.Models;
using GenericApp.Prism.Views;
using Prism.Commands;
using Prism.Navigation;

namespace GenericApp.Prism.ItemViewModels
{
    public class MenuItemViewModel : Menu
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectMenuCommand;

        public MenuItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectMenuCommand => _selectMenuCommand ?? (_selectMenuCommand = new DelegateCommand(SelectMenuAsync));

        private async void SelectMenuAsync()
        {
            if (PageName == nameof(LoginPage) && Settings.IsLogin)
            {
                Settings.IsLogin = false;
                Settings.Token = null;
            }

            if (IsLoginRequired && !Settings.IsLogin)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe estar logueado", "Aceptar");
                NavigationParameters parameters = new NavigationParameters
                    {
                        { "pageReturn", PageName }
                    };

                await _navigationService.NavigateAsync($"/{nameof(GenericAppMasterDetailPage)}/NavigationPage/{nameof(LoginPage)}", parameters);
            }
            else
            {
                await _navigationService.NavigateAsync($"/{nameof(GenericAppMasterDetailPage)}/NavigationPage/{PageName}");
            }
        }
    }
}