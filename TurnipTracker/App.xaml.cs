using System;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using TurnipTracker.Services;
using TurnipTracker.View;
using Xamarin.Essentials;
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

#if DEBUG
        static string ip = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        public static string BaseUrl = $"http://{ip}:7071";
#else
        public const string BaseUrl = "AC_BASEURL";
#endif

        public const string GetFriendsKey = "AC_GetFriendsKey";
        public const string GetFriendRequestsKey = "AC_GetFriendRequestsKey";
        public const string GetFriendRequestCountKey = "AC_GetFriendRequestCountKey";
        public const string PostRemoveFriendRequestKey = "AC_PostRemoveFriendRequestKey";
        public const string PostApproveFriendRequestKey = "AC_PostApproveFriendRequestKey";
        public const string DeleteRemoveFriendKey = "AC_DeleteRemoveFriendKey";
        public const string PostSubmitFriendRequestKey = "AC_PostSubmitFriendRequestKey";
        public const string PutUpdateProfileKey = "AC_PutUpdateProfileKey";
        public const string PutUpdateTurnipPricesKey = "AC_PutUpdateTurnipPricesKey";
        public const string PostCreateProfileKey = "AC_PostCreateProfileKey";

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

            Analytics.TrackEvent("RegisterFriend", new Dictionary<string, string>
            {
                ["type"] = "applink"
            });

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
