using System;
using MvvmHelpers;
using TurnipTracker.Model;
using TurnipTracker.Services;

namespace TurnipTracker.ViewModel
{
    public class ProfileViewModel : BaseViewModel
    {
        public Profile Profile { get; }
        public ProfileViewModel()
        {
            Profile = DataService.GetProfile();
            Profile.SaveProfileAction = SaveProfile;
        }

        void SaveProfile()
        {
            DataService.SaveProfile(Profile);
        }
    }
}
