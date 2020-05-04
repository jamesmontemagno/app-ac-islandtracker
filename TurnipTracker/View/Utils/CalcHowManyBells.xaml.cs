using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnipTracker.View.Utils
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalcHowManyBells : ContentPage
    {
        public CalcHowManyBells()
        {
            InitializeComponent();
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

            var total = (int)(turnip * 10 * slots * 10);
            LabelCalculation.Text = $"You need {total.ToString("n0")} bells";
        }
    }
}