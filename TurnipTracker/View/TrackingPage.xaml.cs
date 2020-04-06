using System;
using System.Collections.Generic;
using TurnipTracker.ViewModel;
using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class TrackingPage : ContentPage
    {
        public TrackingPage()
        {
            InitializeComponent();
            BindingContext = new TrackingViewModel();
        }
    }
}
