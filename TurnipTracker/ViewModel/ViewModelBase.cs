using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using MvvmHelpers;
using MvvmHelpers.Commands;
using TurnipTracker.Helpers;
using TurnipTracker.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TurnipTracker.ViewModel
{
    public class ViewModelBase : BaseViewModel
    {
        public AsyncCommand<string> GoToCommand { get; }
        public AsyncCommand CloseCommand { get; }
        public ViewModelBase()
        {
            ShareWithFriendsCommand = new AsyncCommand<Xamarin.Forms.View>(ShareWithFriends);
            CloseCommand = new AsyncCommand(Close);
            GoToCommand = new AsyncCommand<string>(GoTo);
        }

        public bool IsProAndNotBusy => IsPro && IsNotBusy;

        public bool IsPro => SettingsService.IsPro;

        public bool IsNotPro => !IsPro;

        Task GoTo(string page) => GoToAsync(page);

        Task Close() =>
            GoToAsync("..");

        DataService dataService;
        public DataService DataService => dataService ??= DependencyService.Get<DataService>();

        public AsyncCommand<Xamarin.Forms.View> ShareWithFriendsCommand { get; }


        public async Task<bool> CheckConnectivity(string title, string message)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                return true;

            await DisplayAlert(title, message);
            return false;
        }

        async Task ShareWithFriends(Xamarin.Forms.View element)
        {
            try
            {
                Analytics.TrackEvent("ShareWithFriends");
                var bounds = element.GetAbsoluteBounds();

                await Share.RequestAsync(new ShareTextRequest
                {
                    PresentationSourceBounds = bounds.ToSystemRectangle(),
                    Title = "Island Tracker for ACNH",
                    Text = "Checkout Island Tracker for ACNH and track turnips with me: https://islandtracker.app"
                });
            }
            catch (Exception)
            {
            }
        }

        bool navigating;
        public async Task GoToAsync(string page, string tracker = null)
        {
            if (navigating)
                return;

            Analytics.TrackEvent("Navigation", new Dictionary<string, string>
            {
                ["page"] = tracker ?? page
            });

            await Shell.Current.GoToAsync(page);

            navigating = false;

        }

        public Task DisplayAlert(string title, string message) =>
            Application.Current.MainPage.DisplayAlert(title, message, "OK");
        public Task<bool> DisplayAlert(string title, string message, string accept, string cancel) =>
            Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
    }
}
