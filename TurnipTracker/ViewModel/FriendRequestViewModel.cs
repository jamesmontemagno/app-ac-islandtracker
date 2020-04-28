using System;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers.Commands;
using System.Threading.Tasks;
using TurnipTracker.Services;
using TurnipTracker.Shared;
using MvvmHelpers;

namespace TurnipTracker.ViewModel
{
    public class FriendRequestViewModel : ViewModelBase
    {
        public ObservableRangeCollection<PendingFriendRequest> FriendRequests { get; }
        public FriendRequestViewModel()
        {
            FriendRequests = new ObservableRangeCollection<PendingFriendRequest>();
            RefreshCommand = new AsyncCommand(RefreshAsync);
            DenyFriendRequestCommand = new AsyncCommand<PendingFriendRequest>(DenyFriendRequest);
            ApproveFriendRequestCommand = new AsyncCommand<PendingFriendRequest>(ApproveFriendRequest);
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

            IsBusy = true;
            try
            {

                await DataService.ApproveFriendRequestAsync(pendingFriendRequest.RequesterPublicKey);
                FriendRequests.Remove(pendingFriendRequest);
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
                    FriendRequests.Add(new PendingFriendRequest());

                var requests = await DataService.GetFriendRequestsAsync();
                FriendRequests.ReplaceRange(requests);
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

            IsBusy = true;
            try
            {

                await DataService.RemoveFriendRequestAsync(pendingFriendRequest.RequesterPublicKey);
                FriendRequests.Remove(pendingFriendRequest);
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
