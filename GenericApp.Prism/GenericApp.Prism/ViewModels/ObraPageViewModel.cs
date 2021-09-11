using GenericApp.Common.Helpers;
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
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GenericApp.Prism.ViewModels
{
    public class ObraPageViewModel : ViewModelBase
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

        private int _idPhoto;
        public int IdPhoto { get => _idPhoto; set => SetProperty(ref _idPhoto, value); }

        private int _idPhotoNro;
        public int IdPhotoNro { get => _idPhotoNro; set => SetProperty(ref _idPhotoNro, value); }

        private ObservableCollection<ObrasDocumentoResponse> _images;
        public ObservableCollection<ObrasDocumentoResponse> Images
        {
            get => _images;
            set => SetProperty(ref _images, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private MediaFile _file;
        public MediaFile File
        {
            get => _file;
            set => SetProperty(ref _file, value);
        }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private ObraResponse _obra;
        public ObraResponse Obra
        {
            get => _obra;
            set => SetProperty(ref _obra, value);
        }

        private string _observaciones;
        public string Observaciones
        {
            get => _observaciones;
            set => SetProperty(ref _observaciones, value);
        }

        private string _nroObra;
        public string NroObra
        {
            get => _nroObra;
            set => SetProperty(ref _nroObra, value);
        }

        private string _nombreObra;
        public string NombreObra
        {
            get => _nombreObra;
            set => SetProperty(ref _nombreObra, value);
        }

        private string _eLEMPEP;
        public string ELEMPEP
        {
            get => _eLEMPEP;
            set => SetProperty(ref _eLEMPEP, value);
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(Cancel));
        
        private DelegateCommand _deletePhotoCommand;
        public DelegateCommand DeletePhotoCommand => _deletePhotoCommand ?? (_deletePhotoCommand = new DelegateCommand(DeletePhotoAsync));
        
        private DelegateCommand _newPhotoCommand;
        public DelegateCommand NewPhotoCommand => _newPhotoCommand ?? (_newPhotoCommand = new DelegateCommand(TakePhoto));
        
        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Save));
        
        
        public ObraPageViewModel(INavigationService navigationService, IApiService apiService, IFilesHelper filesHelper) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _filesHelper = filesHelper;
            //Obra = JsonConvert.DeserializeObject<ObraResponse>(Settings.Obra);
            //Images = new ObservableCollection<ObrasDocumentoResponse>(Obra.ObrasDocumentos);
            IsEnabled = true;
            //Title = "Obra: "+Obra.NombreObra;
            instance = this;
            ImageSource = "noimage.png";
            

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("obra"))
            {
                Obra = parameters.GetValue<ObraResponse>("obra");
                Title = Obra.NombreObra;
                Images = new ObservableCollection<ObrasDocumentoResponse>(Obra.ObrasDocumentos);
                IdPhoto = 0;
                if (Images.Count > 0)
                {
                    IdPhoto = Images[0].NROREGISTRO;
                };
                NroObra = Obra.NroObra.ToString();
                NombreObra = Obra.NombreObra;
                ELEMPEP = Obra.ELEMPEP;
            }
        }

        #region Singleton

        private static ObraPageViewModel instance;
        public static ObraPageViewModel GetInstance()
        {
            return instance;
        }
        #endregion

        private async void Save()
        {
            if (_file == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe cargar la foto de la Obra.", "Aceptar");
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


            UsuarioAppResponse User = JsonConvert.DeserializeObject<UsuarioAppResponse>(Settings.UsuarioLogueado);
            

            ObrasDocumentoRequest obrasDocumento = new ObrasDocumentoRequest
            {
                FECHA = DateTime.Now,
                NROOBRA=Obra.NroObra,
                OBSERVACION=Observaciones,
                MODULO="App",
                Estante="App",
                GeneradoPor=User.APELLIDONOMBRE,
                NroLote="App",
                Sector="App",
                ImageArray = ImageArray,
            };

            ResponseT<object> response = await _apiService.PostAsync(
            url,
            "api",
            "/ObrasDocuments",
            obrasDocumento);



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

            ObrasPageViewModel obrasPageViewModel = ObrasPageViewModel.GetInstance();
            obrasPageViewModel.RefreshList();

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


            ObraResponse obra = JsonConvert.DeserializeObject<ObraResponse>(Settings.Obra);
            UsuarioAppResponse User = JsonConvert.DeserializeObject<UsuarioAppResponse>(Settings.UsuarioLogueado);

            ObrasDocumentoRequest myobraDocument = new ObrasDocumentoRequest
            {
                ImageArray = ImageArray,
                FECHA = DateTime.Now,
                NROOBRA = obra.NroObra,
                OBSERVACION = Observaciones,
                Estante = "App",
                GeneradoPor = User.APELLIDONOMBRE,
                MODULO = "App",
                NroLote = "App",
                Sector = "App",
                Obra = obra,
            };

            var response = await _apiService.PostAsync(
            url,
            "api",
            "/ObrasDocuments",
            myobraDocument);

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

            ObrasPageViewModel obrasPageViewModel = ObrasPageViewModel.GetInstance();
            obrasPageViewModel.LoadUser();
            obrasPageViewModel.RefreshList();

            ObrasDocumentoRequest obraDocumentoRequest = (ObrasDocumentoRequest)response.Result;


            ObrasDocumentoResponse obrasDocumentoResponse2 = new ObrasDocumentoResponse
            {
                NROREGISTRO = obraDocumentoRequest.NROREGISTRO,
                LINK = obraDocumentoRequest.LINK,
            };


            ObraPageViewModel obraPageViewModel = ObraPageViewModel.GetInstance();
            obraPageViewModel.Images.Add(new ObrasDocumentoResponse
            {
                NROREGISTRO = obrasDocumentoResponse2.NROREGISTRO,
                LINK = obrasDocumentoResponse2.LINK,
                NROOBRA = obra.NroObra,
            }
                ); ;


            IdPhoto = Images[Images.Count - 1].NROREGISTRO;

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
                IdPhoto = Images[0].NROREGISTRO;
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

            var response = await _apiService.DeleteAsync(
            url,
            "api",
            "/ObrasDocuments",
            IdPhoto);

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

            ObrasPageViewModel obrasPageViewModel = ObrasPageViewModel.GetInstance();
            obrasPageViewModel.LoadUser();
            obrasPageViewModel.RefreshList();

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
                IdPhoto = Images[0].NROREGISTRO;
            }
            else
            {
                IdPhoto = Images[IdPhotoNro].NROREGISTRO;
            };
        }
    }
}