using System.ComponentModel;
using Xamarin.Forms;
using BackgroundPing.ViewModels;

namespace BackgroundPing.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}