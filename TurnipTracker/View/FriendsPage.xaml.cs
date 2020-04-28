using System;
using System.Collections.Generic;
using TurnipTracker.ViewModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class FriendsPage : ContentPage
    {
        public FriendsPage()
        {
            InitializeComponent();
            BindingContext = new FriendsViewModel();
        }

        protected override async void OnAppearing()
        {
          
            await ((FriendsViewModel)BindingContext).RegisterFriendClipboardCommand.ExecuteAsync();

            base.OnAppearing();
        }

    }
}
