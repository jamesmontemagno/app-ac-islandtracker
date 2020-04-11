using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TurnipTracker.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Switcher.SelectedIndex = 1;
        }
    }
}
