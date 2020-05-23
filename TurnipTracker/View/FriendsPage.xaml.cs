using System;
using System.Collections.Generic;
using TurnipTracker.Services;
using TurnipTracker.ViewModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class FriendsPage : ContentPage
    {
        FriendsViewModel vm;
        public FriendsPage()
        {
            InitializeComponent();
            BindingContext = vm = new FriendsViewModel();
        }

        protected override async void OnAppearing()
        {


            vm.RequestCount = SettingsService.FriendRequestCount;


            if(!App.ReceivedAppLink && !(await vm.RegisterFriendClipboard()))
            {
#if !DEBUG
                // if older than 1 hour then refresh
                if (SettingsService.HasRegistered && SettingsService.AutoRefreshFriends && SettingsService.LastFriendsUpdate < DateTime.UtcNow.AddHours(-SettingsService.RefreshAfterHours))
                    _ = vm.RefreshCommand.ExecuteAsync().ContinueWith((r) => { });
                else if(SettingsService.ForceRefreshFriends)
                {
                    SettingsService.ForceRefreshFriends = false;
                    vm.ForceRefresh = true;
                    _ = vm.RefreshCommand.ExecuteAsync().ContinueWith((r) => { });
                }
#endif         
            }

            App.ReceivedAppLink = false;


            base.OnAppearing();
        }


    }
}
