using System;
using System.Threading.Tasks;
using System.Web;
using MvvmHelpers;
using MvvmHelpers.Commands;
using TurnipTracker.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TurnipTracker.ViewModel
{
    public class FriendsViewModel : BaseViewModel
    {
        public FriendsViewModel()
        {
            RegisterFriendClipboardCommand = new AsyncCommand(RegisterFriendClipboard);
            RegisterFriendCommand = new AsyncCommand<string>(RegisterFriend);
            RefreshCommand = new AsyncCommand(RefreshAsync);
            SendFriendRequestCommand = new AsyncCommand(SendFriendRequest);
        }

        public AsyncCommand SendFriendRequestCommand { get; set; }

        async Task SendFriendRequest()
        {
            var name = App.DataService.GetProfile().Name;
            if(string.IsNullOrWhiteSpace(name))
            {
                await App.Current.MainPage.DisplayAlert("Update Profile", "Please update profile before sending a friend request", "OK");
                return;
            }

            var key = await SettingsService.GetPublicKey();
            var message = $"acislandtracker://friends/invite?id={key}&name={Uri.EscapeDataString(name)}";

#if DEBUG
            await Launcher.OpenAsync(message);
#else
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = "Island Tracker Invite",
                Text = message
            });
#endif
        }

        public AsyncCommand<string> RegisterFriendCommand { get; }
        public AsyncCommand RegisterFriendClipboardCommand { get; }

        async Task RegisterFriendClipboard()
        {
            var clip = await Clipboard.GetTextAsync();
            if(await RegisterFriend(clip))
            {
                await Clipboard.SetTextAsync(string.Empty);
            }
        }

        async Task<bool> RegisterFriend(string uriString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(uriString))
                    return false;

                var uri = new Uri(uriString);
                if (uri != null && uri.Host == "friends" && uri.Scheme == "acislandtracker")
                {

                    var key = await SettingsService.GetPublicKey();
                    if (uriString.Contains(key))
                        return false;

                    await Shell.Current.GoToAsync($"//{uri.Host}/{uri.PathAndQuery}");
                    return true;
                }
            }
            catch (Exception ex)
            {

            }

            return false;
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
