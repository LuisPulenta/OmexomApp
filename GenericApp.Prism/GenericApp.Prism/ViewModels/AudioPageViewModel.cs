using GenericApp.Common.Services;
using Plugin.AudioRecorder;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace GenericApp.Prism.ViewModels
{
    public class AudioPageViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;

        private DelegateCommand _recordCommand;
        public DelegateCommand RecordCommand => _recordCommand ?? (_recordCommand = new DelegateCommand(Record));

        private DelegateCommand _playCommand;
        public DelegateCommand PlayCommand => _playCommand ?? (_playCommand = new DelegateCommand(Play));

        private DelegateCommand _stopCommand;
        public DelegateCommand StopCommand => _stopCommand ?? (_stopCommand = new DelegateCommand(Stop));

        AudioRecorderService recorder;
        AudioPlayer player;

        private string _operacion;

        private bool _botonRecordEnabled;
        public bool BotonRecordEnabled
        {
            get => _botonRecordEnabled;
            set => SetProperty(ref _botonRecordEnabled, value);
        }

        private bool _botonPlayEnabled;
        public bool BotonPlayEnabled
        {
            get => _botonPlayEnabled;
            set => SetProperty(ref _botonPlayEnabled, value);
        }

        private bool _botonStopEnabled;
        public bool BotonStopEnabled
        {
            get => _botonStopEnabled;
            set => SetProperty(ref _botonStopEnabled, value);
        }

        private string _titulo;
        public string Titulo
        {
            get => _titulo;
            set => SetProperty(ref _titulo, value);
        }

        private TimeSpan _tiempoTranscurrido;
        public TimeSpan TiempoTranscurrido
        {
            get => _tiempoTranscurrido;
            set => SetProperty(ref _tiempoTranscurrido, value);
        }

        DateTime tiempoInicio = DateTime.MinValue;

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        private Color _colorRecordBotton;
        public Color ColorRecordBotton
        {
            get => _colorRecordBotton;
            set => SetProperty(ref _colorRecordBotton, value);
        }

        private Color _colorPlaydBotton;
        public Color ColorPlayBotton
        {
            get => _colorPlaydBotton;
            set => SetProperty(ref _colorPlaydBotton, value);
        }

        private Color _colorStopBotton;
        public Color ColorStopBotton
        {
            get => _colorStopBotton;
            set => SetProperty(ref _colorStopBotton, value);
        }

        public AudioPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _apiService = apiService;
            Title = "Audio";
            recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(15),
                AudioSilenceTimeout = TimeSpan.FromSeconds(2)
            };

            player = new AudioPlayer();
            BotonRecordEnabled = true;
            BotonPlayEnabled = false;
            BotonStopEnabled = false;
            ColorRecordBotton = Color.Gray;
            ColorPlayBotton = Color.LightGray;
            ColorStopBotton = Color.LightGray;
            Titulo = "Grabador de mensajes";
            player.FinishedPlaying += Player_FinishedPlaying;
        }

        private async Task<bool> CheckAudioPermisionsAsync()
        {
            PermissionStatus permissionAudio = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);
            bool isAudioEnabled = permissionAudio == PermissionStatus.Granted;
            if (isAudioEnabled)
            {
                return true;
            }

            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Microphone);
            
            permissionAudio = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);

            return permissionAudio == PermissionStatus.Granted;
        }

        private async Task<bool> CheckStoragePermisionsAsync()
        {
            PermissionStatus permissionStorage = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            bool isStorageEnabled = permissionStorage == PermissionStatus.Granted;
            if (isStorageEnabled)
            {
                return true;
            }

            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);

            permissionStorage = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            return permissionStorage == PermissionStatus.Granted;
        }


        private async void Record()
        {

            bool isAudioPermision = await CheckAudioPermisionsAsync();
            bool isStoragePermision = await CheckStoragePermisionsAsync();
            _operacion = "grabando";
            if (isAudioPermision && isStoragePermision)
            {
                BotonRecordEnabled = false;
                BotonPlayEnabled = false;
                BotonStopEnabled = true;
                ColorRecordBotton = Color.LightGray;
                ColorPlayBotton = Color.LightGray;
                ColorStopBotton = Color.Gray;
                Titulo = "Grabando...";
                tiempoInicio = DateTime.Now;
                IsRunning = true;
        
                Device.StartTimer(TimeSpan.FromSeconds(1), doitt);

                bool doitt()
                {
                    if(IsRunning)
                    {
                        TiempoTranscurrido = DateTime.Now - tiempoInicio;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                var audioRecordTask = await recorder.StartRecording();
                await audioRecordTask;
            }
        }

        private async void Stop()
        {
            BotonRecordEnabled = true;
            BotonPlayEnabled = true;
            BotonStopEnabled = false;
            ColorRecordBotton = Color.Gray;
            ColorPlayBotton = Color.Gray;
            ColorStopBotton = Color.LightGray;
            Titulo = "Grabador de mensajes";
            //TiempoTranscurrido = DateTime.Now - tiempoInicio;
            IsRunning = false;

            if(_operacion=="grabando")
            {
                await recorder.StopRecording();
            }
            if (_operacion == "reproduciendo")
            {
                player.Pause();
            }
        }


        private void Play()
        {
            _operacion = "reproduciendo";
            try
            {
                var filePath = recorder.GetAudioFilePath();

                if (filePath != null)
                {
                    BotonRecordEnabled = false;
                    BotonPlayEnabled = false;
                    BotonStopEnabled = true;
                    ColorRecordBotton = Color.LightGray;
                    ColorPlayBotton = Color.LightGray;
                    ColorStopBotton = Color.Gray;
                    Titulo = "Reproduciendo...";
                    player.Play(filePath);
                    IsRunning = true;
                    tiempoInicio = DateTime.Now;

                    Device.StartTimer(TimeSpan.FromSeconds(1), doitt);

                    bool doitt()
                    {
                        if (IsRunning)
                        {
                            TiempoTranscurrido = DateTime.Now - tiempoInicio;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception)
            {
                BotonRecordEnabled = true;
                BotonPlayEnabled = true;
                BotonStopEnabled = false;
                ColorRecordBotton = Color.Gray;
                ColorPlayBotton = Color.Gray;
                ColorStopBotton = Color.LightGray;
                Titulo = "Grabador de mensajes";
                //TiempoTranscurrido = DateTime.Now - tiempoInicio;
                IsRunning = false;
            }
        }

        

        void Player_FinishedPlaying(object sender, EventArgs e)
        {
            BotonRecordEnabled = true;
            BotonPlayEnabled = true;
            BotonStopEnabled = false;
            ColorRecordBotton = Color.Gray;
            ColorPlayBotton = Color.Gray;
            ColorStopBotton = Color.LightGray;
            IsRunning = false;
            Titulo = "Grabador de mensajes";
        }
    }
}
