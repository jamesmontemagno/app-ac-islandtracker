using System.Threading.Tasks;
using MvvmHelpers;
using MvvmHelpers.Commands;

namespace TurnipTracker.ViewModel
{
    public class FriendsViewModel : BaseViewModel
    {
        public FriendsViewModel()
        {
            RefreshCommand = new AsyncCommand(RefreshAsync);
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
