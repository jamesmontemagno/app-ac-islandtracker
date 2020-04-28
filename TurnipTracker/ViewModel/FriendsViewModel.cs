using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers;
using MvvmHelpers.Commands;
using TurnipTracker.Services;
using TurnipTracker.Shared;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TurnipTracker.ViewModel
{
    public class FriendsViewModel : ViewModelBase
    {
        public ObservableRangeCollection<FriendStatus> Friends { get; }
        public FriendsViewModel()
        {
            Friends = new ObservableRangeCollection<FriendStatus>();
            RegisterFriendClipboardCommand = new AsyncCommand(RegisterFriendClipboard);
            RegisterFriendCommand = new AsyncCommand<string>(RegisterFriend);
            RefreshCommand = new AsyncCommand(RefreshAsync);
            SendFriendRequestCommand = new AsyncCommand(SendFriendRequest);
            RemoveFriendCommand = new AsyncCommand<FriendStatus>(RemoveFriend);
            GoToFriendRequestCommand = new AsyncCommand(GoToFriendRequest);
        }

        public AsyncCommand SendFriendRequestCommand { get; set; }

        async Task SendFriendRequest()
        {
            if(!SettingsService.HasRegistered)
            {
                await App.Current.MainPage.DisplayAlert("Register First", "Please register your account on the profile tab.", "OK");
                return;
            }

            var name = DataService.GetProfile().Name;
            if(string.IsNullOrWhiteSpace(name))
            {
                await App.Current.MainPage.DisplayAlert("Update Profile", "Please update profile before sending a friend request.", "OK");
                return;
            }

            if (!(await CheckConnectivity("Check connectivity", "Unable to update profile, please check internet and try again")))
                return;

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

        public AsyncCommand GoToFriendRequestCommand { get; }

        async Task GoToFriendRequest()
        {
            if (!SettingsService.HasRegistered)
            {
                await App.Current.MainPage.DisplayAlert("Register First", "Please register your account on the profile tab.", "OK");
                return;
            }

            await Shell.Current.GoToAsync("//friends/friendrequests");
        }

        public AsyncCommand<string> RegisterFriendCommand { get; }
        public AsyncCommand RegisterFriendClipboardCommand { get; }

        async Task RegisterFriendClipboard()
        {
            if (!Clipboard.HasText)
                return;

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
            if (IsBusy)
                return;

            if (!SettingsService.HasRegistered)
            {
                await App.Current.MainPage.DisplayAlert("Register First", "Please register your account on the profile tab.", "OK");
                return;
            }

            IsBusy = true;
            var fake = Friends.Count == 0;
            try
            {
                if (fake)
                    Friends.Add(new FriendStatus());

                var statuses = await DataService.GetFriendsAsync();
                Friends.ReplaceRange(statuses.OrderByDescending(s=>s.TurnipUpdateDayOfYear));
            }
            catch (Exception ex)
            {
                if (fake)
                    Friends.Clear();

                await DisplayAlert("Uh oh, turbulence", "Looks like something went wrong. Check internet and try again.");

                Crashes.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public AsyncCommand<FriendStatus> RemoveFriendCommand { get; set; }
        async Task RemoveFriend(FriendStatus friendStatus)
        {
            if (IsBusy)
                return;

            if (!(await CheckConnectivity("Check connectivity", "Unable to update profile, please check internet and try again")))
                return;

            if (!await DisplayAlert("Remove friend?", $"Are you sure you want to remove {friendStatus.Name}?", "Yes, remove", "Cancel"))
                return;
            
            IsBusy = true;
            try
            {

                await DataService.RemoveFriendAsync(friendStatus.PublicKey);
                Friends.Remove(friendStatus);
            }
            catch (Exception ex)
            {


                await DisplayAlert("Uh oh, turbulence", "Looks like something went wrong. Check internet and try again.");
                Crashes.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
