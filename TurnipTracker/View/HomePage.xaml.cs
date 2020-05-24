using System;
using System.Collections.Generic;
using TurnipTracker.Services;
using TurnipTracker.ViewModel;
using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class HomePage : ContentPage
    {
        HomeViewModel vm;
        public HomePage()
        {
            InitializeComponent();
            BindingContext = vm = new HomeViewModel();
        }

        protected override bool OnBackButtonPressed()
        {
            if (vm.IsBusy)
                return false;

            return base.OnBackButtonPressed();
        }

        bool first = true;
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if(first)
                ComboBoxGateStatus.SelectedItem = ComboBoxGateStatus.ComboBoxSource[vm.Profile.GateStatus];

            first = false;

            await vm.FirstRunCommand.ExecuteAsync();
           
        }
    }
}
