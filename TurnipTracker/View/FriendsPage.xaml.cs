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
            await vm.RegisterFriendClipboardCommand.ExecuteAsync();

            base.OnAppearing();
        }


    }
}
