using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using CustomRenderer.Droid;
using GenericApp.Prism;
using GenericApp.Prism.Droid;
using GenericApp.Prism.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CustomRenderer.Droid
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        private int opc;
        List<CustomPin> customPins;

        public CustomMapRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Address);
            marker.SetSnippet(pin.Label);

            if (pin.ClassId == "Item1")
            {
                opc = 1;
            }
            if (pin.ClassId == "Item2")
            {
                opc = 2;
            }
            if (pin.ClassId == "Item3")
            {
                opc = 3;
            }
            if (pin.ClassId == "Item4")
            {
                opc = 4;
            }
            if (pin.ClassId == "Item5")
            {
                opc = 5;
            }
            if (pin.ClassId == "Item6")
            {
                opc = 6;
            }

            if (pin.StyleId == "PinesAmarillos")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinamarillo));
            }
            if (pin.StyleId == "PinesAzules")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinazul));
            }
            if (pin.StyleId == "PinesCelestes")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinceleste));
            }
            if (pin.StyleId == "PinesNaranjas")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinnaranja));
            }
            if (pin.StyleId == "PinesRojos")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinrojo));
            }
            if (pin.StyleId == "PinesRosas")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinrosa));
            }
            if (pin.StyleId == "PinesVerdes")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinverde));
            }
            if (pin.StyleId == "PinesVerdesClaro")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinverdeclaro));
            }
            if (pin.StyleId == "PinesVioletas")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinvioleta));
            }
            return marker;
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var txtBuscar = e.Marker.Title;

            if (opc == 1)
            {
                ProductsPageViewModel.GetInstance().Search = txtBuscar;
                ProductsPageViewModel.GetInstance().Search = txtBuscar;
                ProductsPageViewModel.GetInstance().CerrarMapa();
            }

            if (opc == 2)
            {
                ProductsPageViewModel.GetInstance().Search = txtBuscar;
                ProductsPageViewModel.GetInstance().CerrarMapa();
            }


            if (opc == 3)
            {
                ProductsPageViewModel.GetInstance().Search = txtBuscar;
                ProductsPageViewModel.GetInstance().CerrarMapa();
            }

            if (opc == 4)
            {
                ProductsPageViewModel.GetInstance().Search = txtBuscar;
                ProductsPageViewModel.GetInstance().CerrarMapa();
            }
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;
                return null;// view;
            }
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }
    }
}