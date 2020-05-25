using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnipTracker.ViewModel;
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

        protected override void OnAppearing()
        {
            ((FriendRequestViewModel)BindingContext).RefreshCommand.ExecuteAsync().ContinueWith((r) => { });
            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            if (IsBusy)
                return false;

            return base.OnBackButtonPressed();
        }
    }
}