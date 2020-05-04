using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using MvvmHelpers.Commands;
using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class UtilsPage : ContentPage
    {
        public AsyncCommand<string> NavigateCommand { get; }
        public UtilsPage()
        {
            InitializeComponent();

            NavigateCommand = new AsyncCommand<string>(Navigate);
            BindingContext = this;

        }

        async Task Navigate(string page)
        {
            Analytics.TrackEvent("Navigation", new Dictionary<string, string>
            {
                ["page"] = page
            });
            await Shell.Current.GoToAsync($"//utils/{page}");
        }
    }
}