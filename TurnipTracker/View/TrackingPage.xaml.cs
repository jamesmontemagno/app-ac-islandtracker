using System;
using System.Collections.Generic;
using TurnipTracker.Services;
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if(DateTime.Now.DayOfYear > 129 && SettingsService.AskForSurvey)
            {
                SettingsService.AskForSurvey = false;
                if(await DisplayAlert("Survey Time!", "Thanks for testing Island Tracker. Please take 2 minutes to provide feedback on the app! <3", "OK", "Not now"))
                {
                    await Xamarin.Essentials.Browser.OpenAsync("https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAAMAAINl_EhURU9ZTVRZWVE0WExFMEJXTDhTSlkxQVZRSi4u");
                }
            }
        }
    }
}
