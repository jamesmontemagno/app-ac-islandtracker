using System;
using System.Collections.Generic;
using TurnipTracker.Services;
using TurnipTracker.ViewModel;
using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class ProfilePage : ContentPage
    {
        ProfileViewModel vm;
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = vm = new ProfileViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            ComboBoxFruit.SelectedItem = ComboBoxFruit.ComboBoxSource[vm.Profile.Fruit];

            if(SettingsService.FirstRun)
            {
                SettingsService.FirstRun = false;
                await DisplayAlert("Welcome!", "Get started with Island Tracker by filling in your profile. Then head over to the tracking section to track turnip prices and get predictions. Finally, sync everything to the cloud and share with your friend.", "OK");
            }
        }
    }
}
