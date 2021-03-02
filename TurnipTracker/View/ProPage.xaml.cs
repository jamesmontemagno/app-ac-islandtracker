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
        ProViewModel vm;
        public ProPage()
        {
            InitializeComponent();
            BindingContext = vm = new ProViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            vm.GetPriceCommand.ExecuteAsync().ContinueWith((t) => { });
        }

    }
}