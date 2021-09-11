using GenericApp.Common.Services;
using GenericApp.Prism.ItemViewModels;
using GenericApp.Prism.ViewModels;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GenericApp.Prism.Views
{
    public partial class ProductsMapPage : ContentPage
    {
        private readonly IGeolocatorService _geolocatorService;

        public ProductsMapPage(IGeolocatorService geolocatorService)
        {
            InitializeComponent();
            _geolocatorService = geolocatorService;
            MyMap.MapType = MapType.Street;
            MyMap.IsVisible = false;
            MoveMapToCurrentPositionAsync();
            MyMap.IsVisible = true;
            ShowPinsAsync();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MyMap.IsVisible = false;
            MoveMapToCurrentPositionAsync();
            MyMap.IsVisible = true;
        }

        private async Task<List<CustomPin>> ShowPinsAsync()
        {

            CustomPin pin = new CustomPin
            {
                Type = PinType.Place,
                Position = new Position(-0, 0),
                Label = " ",
                Address = " ",
                Name = " ",
                StyleId = "",
                Url = ""
            };

            var pins = new List<CustomPin> { pin };
            ProductsPageViewModel productsViewModel = ProductsPageViewModel.GetInstance();
            foreach (ProductItemViewModel product in productsViewModel.Products.ToList())
            {
                if (!string.IsNullOrEmpty(Convert.ToString(product.Latitude)) && !string.IsNullOrEmpty(Convert.ToString(product.Longitude)))
                {
                    if (Convert.ToString(product.Latitude).Length > 5 && Convert.ToString(product.Longitude).Length > 5)
                    {
                        Position position = new Position(Convert.ToDouble(product.Latitude), Convert.ToDouble(product.Longitude));

                        if (product.State.Name == "Sin Iniciar")
                        {
                            MyMap.Pins.Add(new CustomPin
                            {
                                Label = Convert.ToString(product.Id) + '-' + product.State.Name,
                                Address = product.Name,
                                Position = position,
                                Type = PinType.Place,
                                StyleId = "PinesCelestes",
                                ClassId = "Item1",
                            });
                        }

                        else if (product.State.Name == "Iniciado")
                        {
                            MyMap.Pins.Add(new CustomPin
                            {
                                Label = Convert.ToString(product.Id) + '-' + product.State.Name,
                                Address = product.Name,
                                Position = position,
                                Type = PinType.Place,
                                StyleId = "PinesAmarillos",
                                ClassId = "Item1",
                            });
                        }

                        else if (product.State.Name == "Pendiente")
                        {
                            MyMap.Pins.Add(new CustomPin
                            {
                                Label = Convert.ToString(product.Id) + '-' + product.State.Name,
                                Address = product.Name,
                                Position = position,
                                Type = PinType.Place,
                                StyleId = "PinesRojos",
                                ClassId = "Item1",
                            });
                        }

                        else if (product.State.Name == "Terminado")
                        {
                            MyMap.Pins.Add(new CustomPin
                            {
                                Label = Convert.ToString(product.Id) + '-' + product.State.Name,
                                Address = product.Name,
                                Position = position,
                                Type = PinType.Place,
                                StyleId = "PinesVerdes",
                                ClassId = "Item1",
                            });
                        }

                    }
                }
            }
            return pins;
        }

        private async void MoveMapToCurrentPositionAsync()
        {
            bool isLocationPermision = await CheckLocationPermisionsAsync();

            if (isLocationPermision)
            {
                MyMap.IsShowingUser = true;

                await _geolocatorService.GetLocationAsync();
                if (_geolocatorService.Latitude != 0 && _geolocatorService.Longitude != 0)
                {
                    Position position = new Position(
                        _geolocatorService.Latitude,
                        _geolocatorService.Longitude);
                    MyMap.IsVisible = false;


                    MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        position,
                        Distance.FromKilometers(.5)));
                    MyMap.IsVisible = true;

                }
            }
        }

        private async Task<bool> CheckLocationPermisionsAsync()
        {
            PermissionStatus permissionLocation = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            PermissionStatus permissionLocationAlways = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationAlways);
            PermissionStatus permissionLocationWhenInUse = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            bool isLocationEnabled = permissionLocation == PermissionStatus.Granted ||
                                     permissionLocationAlways == PermissionStatus.Granted ||
                                     permissionLocationWhenInUse == PermissionStatus.Granted;
            if (isLocationEnabled)
            {
                return true;
            }

            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

            permissionLocation = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            permissionLocationAlways = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationAlways);
            permissionLocationWhenInUse = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            return permissionLocation == PermissionStatus.Granted ||
                   permissionLocationAlways == PermissionStatus.Granted ||
                   permissionLocationWhenInUse == PermissionStatus.Granted;
        }

        private void MapStreetCommand(object sender, EventArgs eventArgs)
        {
            MyMap.MapType = MapType.Street;
        }
        private void MapSateliteCommand(object sender, EventArgs eventArgs)
        {
            MyMap.MapType = MapType.Satellite;
        }
        private void MapHybridCommand(object sender, EventArgs eventArgs)
        {
            MyMap.MapType = MapType.Hybrid;
        }
    }
}