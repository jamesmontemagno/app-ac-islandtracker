using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MvvmHelpers.Commands;
using TurnipTracker.Services;

namespace TurnipTracker.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public AsyncCommand CloseCommand { get; }

        public SettingsViewModel()
        {
            CloseCommand = new AsyncCommand(Close);
        }

        Task Close() =>
            Shell.Current.GoToAsync("..");

        public bool HideFirstTimeBuying
        {
            get => SettingsService.HideFirstTimeBuying;
            set => SettingsService.HideFirstTimeBuying = value;
        }

        public bool AutoRefreshFriends
        {
            get => SettingsService.AutoRefreshFriends;
            set => SettingsService.AutoRefreshFriends = value;
        }

        public int RefreshAfterHours
        {
            get => SettingsService.RefreshAfterHours switch
            {
                1 => 0,
                2 => 1,
                4 => 2,
                6 => 3,
                12 => 4,
                24 => 5,
                _ => 0
            };
            set
            {
                var val = value switch
                {
                    0 => 1,
                    1 => 2,
                    2 => 4,
                    3 => 6,
                    4 => 12,
                    5 => 24,
                    _ => 1
                };
                SettingsService.RefreshAfterHours = val;
            }
        }
    }
}
