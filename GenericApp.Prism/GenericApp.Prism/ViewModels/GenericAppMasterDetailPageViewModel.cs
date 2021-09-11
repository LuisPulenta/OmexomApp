using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using GenericApp.Common.Helpers;
using GenericApp.Common.Models;
using GenericApp.Common.Responses;
using GenericApp.Prism.ItemViewModels;
using GenericApp.Prism.Views;
using Prism.Navigation;

namespace GenericApp.Prism.ViewModels
{
    public class GenericAppMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private static GenericAppMasterDetailPageViewModel _instance;
        public static GenericAppMasterDetailPageViewModel GetInstance()
        {
            return _instance;
        }


        private UsuarioAppResponse _user;
        public UsuarioAppResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }


        public GenericAppMasterDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            LoadMenus();
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


        private void LoadMenus()
        {
            List<Menu> menus = new List<Menu>
        {
            new Menu
            {
                Icon = "ic_home",
                PageName = $"{nameof(HomePage)}",
                Title = "Inicio",
                IsLoginRequired = true
            },


                new Menu
            {
                Icon = "ic_construction",
                PageName = $"{nameof(ObrasPage)}",
                Title = "Obras",
                IsLoginRequired = true
            },
                       
                new Menu
            {
                Icon = "ic_watch_later",
                PageName = $"{nameof(ObrasWOMPage)}",
                Title = "Obras WOM",
                IsLoginRequired = true
            },
           
            new Menu
            {
                Icon = "ic_exit_to_app",
                PageName = $"{nameof(LoginPage)}",
                Title = Settings.IsLogin ? "Cerrar Sesión" : "Login"
            }
        };

            Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title,
                    IsLoginRequired = m.IsLoginRequired
                }).ToList());
        }
    }
}