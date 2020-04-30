using System;
using System.Collections.Generic;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ComboBoxFruit.SelectedItem = ComboBoxFruit.ComboBoxSource[vm.Profile.Fruit];
        }
    }
}
