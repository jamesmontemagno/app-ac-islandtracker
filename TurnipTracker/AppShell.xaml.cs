using System;
using System.Collections.Generic;
using TurnipTracker.Services;
using TurnipTracker.View;
using TurnipTracker.View.Utils;
using Xamarin.Forms;

namespace TurnipTracker
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

#if DEBUG
            //SettingsService.IsPro = true;
            //SettingsService.NeedsProSync = true;
#endif

            if(!SettingsService.FirstRun)
                MainTabBar.CurrentItem = MainTabBar.Items[1];

            Routing.RegisterRoute("invite", typeof(SubmitFriendRequestPage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("friendrequests", typeof(FriendRequestPage));
            Routing.RegisterRoute("calc-howmanybells", typeof(CalcHowManyBells));
            Routing.RegisterRoute("calc-howmanyturnips", typeof(CalcHowManyTurnips));
            Routing.RegisterRoute("profile", typeof(ProfilePage));
            Routing.RegisterRoute("about", typeof(AboutPage));
            Routing.RegisterRoute("pro", typeof(ProPage));
        }
    }
}