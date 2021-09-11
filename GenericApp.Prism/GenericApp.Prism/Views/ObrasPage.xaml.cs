using GenericApp.Prism.ViewModels;
using Xamarin.Forms;

namespace GenericApp.Prism.Views
{
    public partial class ObrasPage : ContentPage
    {
        public ObrasPage()
        {
            InitializeComponent();
        }
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var obrasViewModel = ObrasPageViewModel.GetInstance();
            obrasViewModel.RefreshList();
        }
    }
}
