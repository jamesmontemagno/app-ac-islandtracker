using System;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using TurnipTracker.Services;
using TurnipTracker.View;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnipTracker
{
    public partial class App : Application
    {

        public App()
        {
#if !DEBUG
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SyncFusionKey);
#endif
            InitializeComponent();
            Xamarin.Forms.Device.SetFlags(new List<string>() {
                    "StateTriggers_Experimental",
                    "IndicatorView_Experimental",
                    "CarouselView_Experimental",
                    "MediaElement_Experimental",
                    "SwipeView_Experimental"
                });

            MainPage = new AppShell();
        }

        const string AppCenteriOS = "AC_IOS";
        const string AppCenterAndroid = "AC_ANDROID";
        const string AppCenterUWP = "AC_UWP";
        const string SyncFusionKey = "AC_SYNC";

        protected override async void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            var key = await SettingsService.GetPublicKey();
            if (uri.PathAndQuery.Contains(key))
                return;

            await Shell.Current.GoToAsync($"//{uri.Host}/{uri.PathAndQuery}");
           
        }

        protected override void OnStart()
        {

#if !DEBUG
            Distribute.UpdateTrack = UpdateTrack.Private;
            AppCenter.Start($"ios={AppCenteriOS};" +
                $"android={AppCenterAndroid};" +
                $"uwp={AppCenterUWP}", 
                typeof(Analytics), 
                typeof(Crashes),
                typeof(Distribute));
#endif
            OnResume();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
