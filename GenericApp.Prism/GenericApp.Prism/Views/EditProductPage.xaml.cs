using GenericApp.Prism.ViewModels;
using Syncfusion.SfRotator.XForms;
using System;
using Xamarin.Forms;

namespace GenericApp.Prism.Views
{
    public partial class EditProductPage : ContentPage
    {
        public EditProductPage()
        {
            InitializeComponent();
        }

        private void Rotator_ItemTapped(object sender, EventArgs e)
        {
            //DisplayAlert("Notification", "Rotator Item is Tapped", "Ok");
        }

        private void Rotator_SelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e)
        {
            EditProductPageViewModel editProductPageViewModel = EditProductPageViewModel.GetInstance();
            int foto = rotator.SelectedIndex;
            editProductPageViewModel.IdPhotoNro = foto;
            editProductPageViewModel.IdPhoto = editProductPageViewModel.Images[foto].Id;
            //DisplayAlert("Notification", "Selected Index is Changed", "Ok");
        }
    }
}