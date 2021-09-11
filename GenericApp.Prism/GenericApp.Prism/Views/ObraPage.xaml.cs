using GenericApp.Prism.ViewModels;
using Syncfusion.SfRotator.XForms;
using System;
using Xamarin.Forms;

namespace GenericApp.Prism.Views
{
    public partial class ObraPage : ContentPage
    {
        public ObraPage()
        {
            InitializeComponent();
        }
        private void Rotator_ItemTapped(object sender, EventArgs e)
        {
            //DisplayAlert("Notification", "Rotator Item is Tapped", "Ok");
        }

        private void Rotator_SelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e)
        {
            ObraPageViewModel obraPageViewModel = ObraPageViewModel.GetInstance();
            int foto = rotator.SelectedIndex;
            obraPageViewModel.IdPhotoNro = foto;
            obraPageViewModel.IdPhoto = obraPageViewModel.Images[foto].NROREGISTRO;
            var bb = 1;
            //DisplayAlert("Notification", "Selected Index is Changed", "Ok");

        }
    }
}
