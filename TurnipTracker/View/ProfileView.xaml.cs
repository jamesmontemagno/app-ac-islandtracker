using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class ProfileView : ContentView
    {
        public ProfileView()
        {
            InitializeComponent();
        }

        void ButtonSave_TouchUp(System.Object sender, System.EventArgs e)
        {
            LabelSaved.Opacity = 1;
            LabelSaved.FadeTo(0, 2500);
        }
    }
}
