using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace TurnipTracker.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendRequestPage : ContentPage
    {
        public FriendRequestPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }


        protected override void OnSizeAllocated(double width, double height)
        {
            var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
            safeInsets.Bottom = 0;
            Padding = safeInsets;
            base.OnSizeAllocated(width, height);
        }

        protected override bool OnBackButtonPressed()
        {
            if (IsBusy)
                return false;

            return base.OnBackButtonPressed();
        }

        async void ButtonClose_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        async void SfEffectsView_TouchUp(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}