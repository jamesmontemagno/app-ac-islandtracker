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

            if(!SettingsService.FirstRun)
                TabBar.CurrentItem = TabBar.Items[1];

            Routing.RegisterRoute("invite", typeof(SubmitFriendRequestPage));
            Routing.RegisterRoute("friendrequests", typeof(FriendRequestPage));
            Routing.RegisterRoute("calc-howmanybells", typeof(CalcHowManyBells));
            Routing.RegisterRoute("calc-howmanyturnips", typeof(CalcHowManyTurnips));
        }
    }
}