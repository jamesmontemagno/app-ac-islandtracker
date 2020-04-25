using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Xamarin.Forms;

namespace TurnipTracker.Droid
{
    [Activity(Label = "Island Tracker", Icon = "@mipmap/icon", RoundIcon ="@mipmap/icon",
        Theme = "@style/MainTheme", MainLauncher = false,
        LaunchMode = LaunchMode.SingleTask,
        ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode)]
    [IntentFilter(new[] { Android.Content.Intent.ActionSend },
        Categories = new[] { Android.Content.Intent.CategoryDefault},
        DataMimeType = "text/plain")]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
        Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
        DataScheme = "acislandtracker")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if(intent.Action == Intent.ActionSend &&
                intent.Type == "text/plain")
            {
                var uriString = intent.GetStringExtra(Intent.ExtraText);
                if (!string.IsNullOrWhiteSpace(uriString) &&
                    uriString.StartsWith("acislandtracker:"))
                {
                    var url = new Uri(uriString);
                    Xamarin.Forms.Application.Current.SendOnAppLinkRequestReceived(url);
                }
            }

        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
        }
    }
}