using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using GenericApp.Common.Services;
using GenericApp.Prism.Views;
using GenericApp.Prism.ViewModels;
using GenericApp.Prism.Helpers;
using GenericApp.Common.Helpers;
using GenericApp.Common.Data;
using GenericApp.Prism.Services;

namespace GenericApp.Prism
{
    public partial class App
    {
        private static UsersDatabase database;

        public static UsersDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new UsersDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("usersdb.db3"));
                }
                return database;
            }
        }

        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY2MzIyQDMxMzcyZTMzMmUzMFVnNW5KSnM2dTZmRDljWm1RYTduQXFwRmNKSzVPWk1lT1JGSFRySXZCUTA9");
            InitializeComponent();

            if (Settings.IsLogin)
            {
                await NavigationService.NavigateAsync($"{nameof(GenericAppMasterDetailPage)}/NavigationPage/{nameof(HomePage)}");
            }
            else
            {
                //await NavigationService.NavigateAsync($"{nameof(GenericAppMasterDetailPage)}/NavigationPage/{nameof(LoginPage)}");
                await NavigationService.NavigateAsync($"{nameof(GenericAppMasterDetailPage)}/NavigationPage/{nameof(AudioPage)}");
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.Register<IFilesHelper, FilesHelper>();
            containerRegistry.Register<IRegexHelper, RegexHelper>();
            containerRegistry.Register<ICombosHelper, CombosHelper>();
            containerRegistry.Register<IGeolocatorService, GeolocatorService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<RecoverPasswordPage, RecoverPasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<GenericAppMasterDetailPage, GenericAppMasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<ModifyUserPage, ModifyUserPageViewModel>();

            containerRegistry.RegisterForNavigation<ChangePasswordPage, ChangePasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<ProductsPage, ProductsPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<ProductsMapPage, ProductsMapPageViewModel>();
            containerRegistry.RegisterForNavigation<AddProductPage, AddProductPageViewModel>();
            containerRegistry.RegisterForNavigation<EditProductPage, EditProductPageViewModel>();
            containerRegistry.RegisterForNavigation<ViewProductPage, ViewProductPageViewModel>();
            containerRegistry.RegisterForNavigation<ObrasPage, ObrasPageViewModel>();
            containerRegistry.RegisterForNavigation<ObraPage, ObraPageViewModel>();
            containerRegistry.RegisterForNavigation<ObrasWOMPage, ObrasWOMPageViewModel>();
            containerRegistry.RegisterForNavigation<AudioPage, AudioPageViewModel>();
        }
    }
}
