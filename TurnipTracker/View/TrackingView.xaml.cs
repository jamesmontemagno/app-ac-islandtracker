using System;
using System.Collections.Generic;
using TurnipTracker.ViewModel;
using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class TrackingView : ContentView
    {
        public TrackingView()
        {
            InitializeComponent();
            BindingContext = new TrackingViewModel();
        }
    }
}
