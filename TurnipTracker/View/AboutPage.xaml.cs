using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnipTracker.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        async void ButonAddJames_Clicked(object sender, EventArgs e)
        {
            var key = string.Empty;
            var message = $"acislandtracker://friends/invite?id={key}&name=James";

            await Launcher.OpenAsync(message);
        }
    }
}