using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class TransactionsView : ContentView
    {
        public TransactionsView()
        {
            InitializeComponent();
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await App.Current.MainPage.DisplayActionSheet("Add Transaction", "Cancel", null, "Purchase", "Sale");
        }
    }
}
