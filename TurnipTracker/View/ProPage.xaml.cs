using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnipTracker.ViewModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnipTracker.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProPage : ContentPage
    {
        public ProPage()
        {
            InitializeComponent();
            BindingContext = new ProViewModel();
        }

    }
}