using System;
using System.Collections.Generic;
using TurnipTracker.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
//using Xamanimation;

namespace TurnipTracker.View
{
    public partial class AcceptFriendRequestPage : ContentPage
    {
        readonly Animation rotation;
        AcceptFriendRequestViewModel vm;
        AcceptFriendRequestViewModel VM => vm ??= (BindingContext as AcceptFriendRequestViewModel);

        public AcceptFriendRequestPage()
        {
            InitializeComponent();
            rotation = new Animation(v => LabelRotateIcon.Rotation = v, 0, 360);

            VM.PropertyChanged += OnPropertyChanged;
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

       

       

        protected override void OnSizeAllocated(double width, double height)
        {
            var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
            safeInsets.Bottom = 0;
            Padding = safeInsets;
            base.OnSizeAllocated(width, height);
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(VM.IsBusy))
            {
                if (!VM.IsBusy)
                    this.AbortAnimation("rotate");
                else
                    rotation.Commit(this, "rotate", 16, 1000, Easing.Linear, (v, c) => LabelRotateIcon.Rotation = 0, () => true);
            }
        }
        

        protected override bool OnBackButtonPressed()
        {
            if (IsBusy)
                return false;

            return base.OnBackButtonPressed();
        }


        async void ButtonClose_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
