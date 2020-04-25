
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace TurnipTracker.Droid
{
    [Activity(Label = "Island Tracker", 
        Icon = "@mipmap/icon", 
        RoundIcon = "@mipmap/icon",
        Theme = "@style/SplashTheme",
        MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.AddFlags(ActivityFlags.SingleTop);
            StartActivity(intent);
            Finish();
        }
    }
}