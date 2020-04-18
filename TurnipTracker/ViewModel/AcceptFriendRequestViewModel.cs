using System;
using System.Threading.Tasks;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Xamarin.Forms;

namespace TurnipTracker.ViewModel
{
    [QueryProperty("Name", "name")]
    [QueryProperty("Id", "id")]
    public class AcceptFriendRequestViewModel : BaseViewModel
    {
        public AsyncCommand RequestFriendCommand { get; }
        public AsyncCommand CloseCommand { get; }
        public AcceptFriendRequestViewModel()
        {
            RequestFriendCommand = new AsyncCommand(RequestFriend);
            CloseCommand = new AsyncCommand(Close);
        }

        bool submitted;
        public bool Submitted
        {
            get => submitted;
            set => SetProperty(ref submitted, value);
        }

        bool needsVerification = true;
        public bool NeedsVerification
        {
            get => needsVerification;
            set => SetProperty(ref needsVerification, value);
        }

        bool showClose;
        public bool ShowClose
        {
            get => showClose;
            set => SetProperty(ref showClose, value);
        }

        bool error;
        public bool Error
        {
            get => error;
            set => SetProperty(ref error, value);
        }

        string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, Uri.UnescapeDataString(value));
        }

        string id;
        public string Id
        {
            get => id;
            set => SetProperty(ref id, Uri.UnescapeDataString(value));
        }


        async Task RequestFriend()
        {
            if (IsBusy)
                return;

            NeedsVerification = false;


            

            IsBusy = true;

            await Task.Delay(2000);

            Submitted = true;

            ShowClose = true;

            IsBusy = false;
        }

        async Task Close()
        {
            await Shell.Current.GoToAsync("//friends");
        }
    }
}
