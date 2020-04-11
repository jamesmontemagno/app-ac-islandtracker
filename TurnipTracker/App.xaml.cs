using System;
using System.Collections.Generic;
using TurnipTracker.View;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnipTracker
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            Xamarin.Forms.Device.SetFlags(new List<string>() {
                    "StateTriggers_Experimental",
                    "IndicatorView_Experimental",
                    "CarouselView_Experimental",
                    "MediaElement_Experimental",
                    "SwipeView_Experimental"
                });

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
