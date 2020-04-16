using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace TurnipTracker.Droid
{
    /*
     * <intent-filter>
        <action android:name="android.intent.action.SEND" />
        <category android:name="android.intent.category.DEFAULT" />
        <data android:mimeType="text/plain" />
    </intent-filter>
     */
    [Activity(Label = "Island Tracker", Icon = "@mipmap/icon", RoundIcon ="@mipmap/icon",
        Theme = "@style/MainTheme", MainLauncher = true,
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

        protected override async void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            Android.Net.Uri uri = null;
            if(intent.Action == Intent.ActionSend &&
                intent.Type == "text/plain")
            {
                var uriString = intent.GetStringExtra(Intent.ExtraText);
                if (!string.IsNullOrWhiteSpace(uriString))
                    uri = Android.Net.Uri.Parse(uriString);
            }
            else if(intent.Action == Intent.ActionView && intent.Data != null)
            {
                uri = intent.Data;
            }

            if(uri != null && uri.Scheme == "acislandtracker" && uri.Host == "invite")
            {
                var id = uri.GetQueryParameter("id");
                var name = uri.GetQueryParameter("name");
                if (!string.IsNullOrWhiteSpace(id))
                    await App.Current.MainPage.DisplayAlert($"Add Friend", $"Would you like to add {name} as a friend?", "Yes", "No");
            }
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
        }
    }
}