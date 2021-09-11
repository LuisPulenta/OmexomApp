using System.Collections.ObjectModel;
using Newtonsoft.Json;
using GenericApp.Common.Helpers;
using GenericApp.Common.Responses;
using GenericApp.Prism.ItemViewModels;
using Prism.Navigation;

namespace GenericApp.Prism.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private UsuarioAppResponse _user;
        public UsuarioAppResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public HomePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Omexom App";
            LoadUser();
        }
        public ObservableCollection<MenuItemViewModel> Menus { get; set; }


        public void LoadUser()
        {
            if (Settings.IsLogin)
            {
                User = JsonConvert.DeserializeObject<UsuarioAppResponse>(Settings.UsuarioLogueado);
            }
        }
    }
}