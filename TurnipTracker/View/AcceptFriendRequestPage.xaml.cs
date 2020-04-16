using System;
using System.Collections.Generic;

using Xamarin.Forms;
//using Xamanimation;

namespace TurnipTracker.View
{
    public partial class AcceptFriendRequestPage : ContentPage
    {
        Animation rotation;
        public AcceptFriendRequestPage()
        {
            InitializeComponent();
            rotation = new Animation(v => LabelRotateIcon.Rotation = v, 0, 360);
        }

 
        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (this.AnimationIsRunning("rotate"))
                this.AbortAnimation("rotate");
            else
                rotation.Commit(this, "rotate", 16, 1000, Easing.Linear, (v, c) => LabelRotateIcon.Rotation = 0, () => true);
        }

        async void ButtonClose_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
