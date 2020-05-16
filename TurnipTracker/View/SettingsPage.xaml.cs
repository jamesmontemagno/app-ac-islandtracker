using TurnipTracker.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace TurnipTracker.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel vm;
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = vm = new SettingsViewModel();
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
            base.OnAppearing();
            ComboBoxAutoRefreshHours.SelectedItem = ComboBoxAutoRefreshHours.ComboBoxSource[vm.RefreshAfterHours];
        }
    }
}