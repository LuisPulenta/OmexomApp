using GenericApp.Common.Helpers;
using GenericApp.Common.Responses;
using GenericApp.Common.Services;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GenericApp.Prism.ViewModels
{
    public class ObrasPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private UsuarioAppResponse _user;
        public UsuarioAppResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }
        
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isRefreshing;
        private ObservableCollection<ObraItemViewModel> _obras;
        private static ObrasPageViewModel _instance;
        private int _cantObras;
        private string _filter;
        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
        }
        private DelegateCommand _searchCommand;
        private DelegateCommand _refreshCommand;

        public DelegateCommand SearchCommand => _searchCommand ?? (_searchCommand = new DelegateCommand(Search));
        public DelegateCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(Refresh));



        public int CantObras
        {
            get => _cantObras;
            set => SetProperty(ref _cantObras, value);
        }
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
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
        public ObservableCollection<ObraItemViewModel> Obras
        {
            get => _obras;
            set => SetProperty(ref _obras, value);
        }

        public List<ObraResponse> MyObras { get; set; }

        public static ObrasPageViewModel GetInstance()
        {
            return _instance;
        }


        public ObrasPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;
            LoadUser();
            Obras = new ObservableCollection<ObraItemViewModel>();
            Title = "Obras";
            _instance = this;
        }


        public async void LoadUser()
        {
            User = JsonConvert.DeserializeObject<UsuarioAppResponse>(Settings.UsuarioLogueado);
            var controller = string.Format("/Account/GetObras");
            var url = App.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.GetObras(
                url,
                "api",
                controller);
            IsRefreshing = false;
            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Problema para recuperar datos.", "Aceptar");
                return;
            }
            MyObras = (List<ObraResponse>)response.Result;
            RefreshList();
            IsRefreshing = false;

            //Remotes = new ObservableCollection<RemoteItemViewModel>(MyRemotes.Select(a => new RemoteItemViewModel(_navigationService)
            //{
            //}).ToList());
        }

        public void RefreshList()
        {

            if (string.IsNullOrEmpty(this.Filter))
            {

                var myObraItemViewModel = MyObras.Select(a => new ObraItemViewModel(_navigationService)
                {
                    ELEMPEP=a.ELEMPEP,
                    NombreObra=a.NombreObra,
                    NroObra=a.NroObra,
                    CantObras=a.CantObras,
                    ObrasDocumentos=a.ObrasDocumentos
                    
                });
                Obras = new ObservableCollection<ObraItemViewModel>(myObraItemViewModel
                    .OrderBy(o => o.NombreObra));

                CantObras = Obras.Count();
            }
            else
            {
                var myObraItemViewModel = MyObras.Select(a => new ObraItemViewModel(_navigationService)
                {
                    ELEMPEP = a.ELEMPEP,
                    NombreObra = a.NombreObra,
                    NroObra = a.NroObra,
                    CantObras = a.CantObras,
                    ObrasDocumentos = a.ObrasDocumentos
                });
                Obras = new ObservableCollection<ObraItemViewModel>(myObraItemViewModel
                    .OrderBy(o => o.NombreObra)
                    .Where(
                            o => (o.NombreObra.ToLower().Contains(this.Filter.ToLower()))
                            ||
                            (o.ELEMPEP.ToLower().Contains(this.Filter.ToLower()))
                            ||
                            (o.NroObra.ToString().Contains(this.Filter.ToLower())))
                          );
                CantObras = Obras.Count();
            }
        }
        private async void Search()
            {
                RefreshList();
            }

        private async void Refresh()
            {
                LoadUser();
            }
    }
}