using System;
using System.Threading.Tasks;
using MvvmHelpers;
using MvvmHelpers.Commands;
using TurnipTracker.Services;
using Xamarin.Essentials;

namespace TurnipTracker.ViewModel
{
    public class FriendsViewModel : BaseViewModel
    {
        public FriendsViewModel()
        {
            RefreshCommand = new AsyncCommand(RefreshAsync);
            SendFriendRequestCommand = new AsyncCommand(SendFriendRequest);
        }

        public AsyncCommand SendFriendRequestCommand { get; set; }

        async Task SendFriendRequest()
        {
            var name = DataService.GetProfile().Name;
            if(string.IsNullOrWhiteSpace(name))
            {
                await App.Current.MainPage.DisplayAlert("Update Profile", "Please update profile before sending a friend request", "OK");
                return;
            }

            var key = await SettingsService.GetPublicKey();
            var message = $"acislandtracker://invite?id={key}&name={Uri.EscapeDataString(name)}";
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = "Island Tracker Invite",
                Text = message
            });
        }

        public AsyncCommand RefreshCommand { get; set; }
        async Task RefreshAsync()
        {
            IsBusy = true;
            await Task.Delay(2000);
            IsBusy = false;
        }
    }
}
