using System;
using System.Collections.Generic;
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
using TurnipTracker.Helpers;

namespace TurnipTracker.ViewModel
{
    public class FriendsViewModel : ViewModelBase
    {
        public ObservableRangeCollection<FriendStatus> Friends { get; }
        public bool ShowNoFriends => Friends.Count == 0;

        public FriendsViewModel()
        {
            Friends = new ObservableRangeCollection<FriendStatus>();
            RegisterFriendClipboardCommand = new AsyncCommand(RegisterFriendClipboard);
            RegisterFriendCommand = new AsyncCommand<string>(RegisterFriend);
            RefreshCommand = new AsyncCommand(RefreshAsync);
            SendFriendRequestCommand = new AsyncCommand<Xamarin.Forms.View>(SendFriendRequest);
            RemoveFriendCommand = new AsyncCommand<FriendStatus>(RemoveFriend);
            GoToFriendRequestCommand = new AsyncCommand(GoToFriendRequest);
            var cache = DataService.GetCache<IEnumerable<FriendStatus>>(DataService.FriendKey);
            if(cache != null)
                Friends.ReplaceRange(cache.OrderByDescending(s => s.TurnipUpdateTimeUTC));

            
        }

        string requestCount = string.Empty;
        public string RequestCount
        {
            get => requestCount;
            set => SetProperty(ref requestCount, value);
        }

        bool forceRefresh = false;

        public string LastUpdate
        {
            get
            {
                var time = SettingsService.LastFriendsUpdate;
                if (time == DateTime.MinValue)
                    return string.Empty;
                var local = time.ToLocalTime();
                return $"Last update: {local.ToShortDateString()} at {local.ToShortTimeString()}";
            }
        }


        public AsyncCommand<Xamarin.Forms.View> SendFriendRequestCommand { get; set; }

        async Task SendFriendRequest(Xamarin.Forms.View element)
        {
            if(!SettingsService.HasRegistered)
            {
                await DisplayAlert("Register First", "Please register your account on the profile tab.");
                return;
            }

            if(SettingsService.FirstFriendRequest)
            {
                await DisplayAlert("How to send a request", 
                    "Looks like this is your first time sending a friend request! Exciting! Simply share the friend invite link with your friends and have them click the link to open Island Tracker or copy it to their clipboard and go to their friends tab. Once they request you as a friend you will see the request in the 'Request' section above that you can either approve or deny.");
                SettingsService.FirstFriendRequest = false;
            }

            var name = DataService.GetProfile().Name;
            if(string.IsNullOrWhiteSpace(name))
            {
                await DisplayAlert("Update Profile", "Please update profile before sending a friend request.");
                return;
            }

            if (!(await CheckConnectivity("Check connectivity", "Unable to update profile, please check internet and try again")))
                return;

            var key = await SettingsService.GetPublicKey();
            var message = $"acislandtracker://friends/invite?id={key}&name={Uri.EscapeDataString(name)}";

            //#if DEBUG
            //            await Launcher.OpenAsync(message);
            //#else
            var bounds = element.GetAbsoluteBounds();
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = "Island Tracker Invite",
                Text = message,
                PresentationSourceBounds = bounds.ToSystemRectangle()
            });
//#endif
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

                var created = Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var uri);

                if (!created)
                    return false;

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
                Crashes.TrackError(ex);
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
                await DisplayAlert("Register First", "Please register your account on the profile tab.");
                return;
            }

            IsBusy = true;
            var fake = Friends.Count == 0;
            try
            {
                if (fake)
                {
                    Friends.Add(new FriendStatus());
                    OnPropertyChanged(nameof(ShowNoFriends));
                }

                var friendsTask = DataService.GetFriendsAsync(forceRefresh);
                var countTask = DataService.GetFriendRequestCountAsync();
                await Task.WhenAll(friendsTask, countTask);
                if (friendsTask.IsFaulted && friendsTask.Exception != null)
                    throw friendsTask.Exception;

                var statuses = friendsTask.Result;
                forceRefresh = false;
                Friends.ReplaceRange(statuses.OrderByDescending(s=>s.TurnipUpdateTimeUTC));
                SettingsService.LastFriendsUpdate = DateTime.UtcNow;
                OnPropertyChanged(nameof(LastUpdate));

                if (!countTask.IsFaulted)
                    RequestCount = countTask.Result.Count == 0 ? string.Empty : countTask.Result.Count.ToString();
                else
                    RequestCount = string.Empty;


                SettingsService.FriendRequestCount = RequestCount;

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
            OnPropertyChanged(nameof(ShowNoFriends));
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
                forceRefresh = true;
                OnPropertyChanged(nameof(ShowNoFriends));
                DataService.ClearCache(DataService.FriendKey);
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
