using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnipTracker.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnipTracker.View.Utils
{
    [QueryProperty(nameof(Price), nameof(Price))]
    public partial class CalcHowManyBells : ContentPage
    {
        public CalcHowManyBells()
        {
            InitializeComponent();

            EntryOpenSlots.Value = (double)SettingsService.CalcTurnipsSlots;
        }

        public string Price
        {
            set
            {
                if (int.TryParse(Uri.UnescapeDataString(value ?? string.Empty), out var price) && price >0)
                {
                    EntryTurnipPrice.Value = (double)price;
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ValueChanged(null, null);
        }

        private void ValueChanged(object sender, Syncfusion.SfNumericTextBox.XForms.ValueEventArgs e)
        {
           if (EntryTurnipPrice.Value == null ||
                EntryOpenSlots.Value == null)
                return;

            var turnip = (double)EntryTurnipPrice.Value;
            var slots = (double)EntryOpenSlots.Value;


            if (turnip == 0 || slots == 0)
                return;

            SettingsService.CalcTurnipsSlots = (int)slots;

            var total = (int)(turnip * 10 * slots * 10);
            LabelCalculation.Text = $"You need {total.ToString("n0")} bells";
        }
    }
}