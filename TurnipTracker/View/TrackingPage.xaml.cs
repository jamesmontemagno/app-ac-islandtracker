using System;
using System.Collections.Generic;
using TurnipTracker.Services;
using TurnipTracker.ViewModel;
using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class TrackingPage : ContentPage
    {
        TrackingViewModel vm;
        public TrackingPage()
        {
            InitializeComponent();
            BindingContext = vm = new TrackingViewModel();
            Chart.SuspendSeriesNotification();
        }

        protected override bool OnBackButtonPressed()
        {
            if (vm.IsBusy)
                return false;

            return base.OnBackButtonPressed();
        }

        private void SfExpander_Expanded(object sender, Syncfusion.XForms.Expander.ExpandedAndCollapsedEventArgs e)
        {
            if (ChartExpander.IsExpanded)
                Chart.ResumeSeriesNotification();
            else
                Chart.SuspendSeriesNotification();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            vm.OnAppearing();
        }
    }
}
