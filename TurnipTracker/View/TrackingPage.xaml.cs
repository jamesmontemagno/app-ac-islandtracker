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
            Chart.SuspendSeriesNotification();
        }

        private void SfExpander_Expanded(object sender, Syncfusion.XForms.Expander.ExpandedAndCollapsedEventArgs e)
        {
            if (ChartExpander.IsExpanded)
                Chart.ResumeSeriesNotification();
            else
                Chart.SuspendSeriesNotification();
        }
    }
}
