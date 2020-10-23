using System;
using System.Collections.Generic;
using BackgroundPing.ViewModels;
using BackgroundPing.Views;
using Xamarin.Forms;

namespace BackgroundPing
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
