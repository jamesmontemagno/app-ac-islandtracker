using TurnipTracker.ViewModel;
using Xamarin.Forms;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ComboBoxAutoRefreshHours.SelectedItem = ComboBoxAutoRefreshHours.ComboBoxSource[vm.RefreshAfterHours];
        }
    }
}