using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class TransactionPage : ContentPage
    {
        public TransactionPage()
        {
            InitializeComponent();
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await DisplayActionSheet("Add Transaction", "Cancel", null, "Purchase", "Sale");
        }
    }
}
