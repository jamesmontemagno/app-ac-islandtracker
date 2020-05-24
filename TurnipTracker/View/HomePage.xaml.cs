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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //ComboBoxGateStatus.ComboBoxSource = new List<string>
            //{
            //    "Closed",
            //    "Open to All Friends",
            //    "Open to Best Friends",
            //    "Dodo Code"
            //};

            ComboBoxGateStatus.SelectedItem = ComboBoxGateStatus.ComboBoxSource[vm.Profile.GateStatus];

            

            await vm.FirstRunCommand.ExecuteAsync();
           
        }
    }
}
