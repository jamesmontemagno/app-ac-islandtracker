using System;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers.Commands;
using System.Threading.Tasks;
using TurnipTracker.Services;
using TurnipTracker.Shared;
using MvvmHelpers;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;

namespace TurnipTracker.ViewModel
{
    public class FriendRequestViewModel : ViewModelBase
    {
        public ObservableRangeCollection<PendingFriendRequest> FriendRequests { get; }

        bool forceRefresh = true;

        public bool ShowNoFriends => FriendRequests.Count == 0;
        public string LastUpdate
        {
            get
            {
                var time = SettingsService.LastFriendRequestsUpdate;
                if (time == DateTime.MinValue)
                    return string.Empty;
                var local = time.ToLocalTime();
                return $"Last update: {local.ToShortDateString()} at {local.ToShortTimeString()}";
            }
        }
        public FriendRequestViewModel()
        {
            FriendRequests = new ObservableRangeCollection<PendingFriendRequest>();
            RefreshCommand = new AsyncCommand(RefreshAsync);
            DenyFriendRequestCommand = new AsyncCommand<PendingFriendRequest>(DenyFriendRequest);
            ApproveFriendRequestCommand = new AsyncCommand<PendingFriendRequest>(ApproveFriendRequest);
            //var cache = DataService.GetCache<IEnumerable<PendingFriendRequest>>("get_friend_requests");
            //if (cache != null)
            //    FriendRequests.ReplaceRange(cache);
        }

        public AsyncCommand<PendingFriendRequest> ApproveFriendRequestCommand { get; }


        async Task ApproveFriendRequest(PendingFriendRequest pendingFriendRequest)
        {
            if (IsBusy)
                return;

            if (!(await CheckConnectivity("Check connectivity", "Unable to update profile, please check internet and try again")))
                return;

            if (!await DisplayAlert("Approve friend request?", $"Are you sure you want to approve {pendingFriendRequest.Name}'s request?", "Yes, approve", "Cancel"))
                return;

            Analytics.TrackEvent("FriendRequest", new Dictionary<string, string>
            {
                ["type"] = "approve"
            });

            IsBusy = true;
            try
            {

                await DataService.ApproveFriendRequestAsync(pendingFriendRequest.RequesterPublicKey);
                FriendRequests.Remove(pendingFriendRequest);
                DataService.ClearCache(DataService.FriendRequestKey);
                forceRefresh = true;

                SettingsService.FriendRequestCount = FriendRequests.Count == 0 ? string.Empty : FriendRequests.Count.ToString();
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

            OnPropertyChanged(nameof(ShowNoFriends));
        }

        public AsyncCommand RefreshCommand { get; set; }
        async Task RefreshAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            var fake = FriendRequests.Count == 0;
            try
            {
                if (fake)
                {
                    FriendRequests.Add(new PendingFriendRequest());

                    OnPropertyChanged(nameof(ShowNoFriends));
                }

                var requests = await DataService.GetFriendRequestsAsync(forceRefresh);
                forceRefresh = false;
                FriendRequests.ReplaceRange(requests);
                SettingsService.LastFriendRequestsUpdate = DateTime.UtcNow;
                OnPropertyChanged(nameof(LastUpdate));
                SettingsService.FriendRequestCount = FriendRequests.Count == 0 ? string.Empty : FriendRequests.Count.ToString();
            }
            catch (Exception ex)
            {
                if (fake)
                    FriendRequests.Clear();

                await DisplayAlert("Uh oh, turbulence", "Looks like something went wrong. Check internet and try again.");

                Crashes.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }
            OnPropertyChanged(nameof(ShowNoFriends));
        }

        public AsyncCommand<PendingFriendRequest> DenyFriendRequestCommand { get; set; }
        async Task DenyFriendRequest(PendingFriendRequest pendingFriendRequest)
        {
            if (IsBusy)
                return;

            if (!(await CheckConnectivity("Check connectivity", "Unable to update profile, please check internet and try again")))
                return;

            if (!await DisplayAlert("Delete friend request?", $"Are you sure you want to deny {pendingFriendRequest.Name}'s request?", "Yes, deny", "Cancel"))
                return;

            Analytics.TrackEvent("FriendRequest", new Dictionary<string, string>
            {
                ["type"] = "deny"
            });

            IsBusy = true;
            try
            {

                await DataService.RemoveFriendRequestAsync(pendingFriendRequest.RequesterPublicKey);
                FriendRequests.Remove(pendingFriendRequest);
                forceRefresh = true;
                DataService.ClearCache(DataService.FriendRequestKey);
                OnPropertyChanged(nameof(ShowNoFriends));
                SettingsService.FriendRequestCount = FriendRequests.Count == 0 ? string.Empty : FriendRequests.Count.ToString();
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
