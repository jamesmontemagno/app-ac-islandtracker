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
    public partial class CalcHowManyTurnips : ContentPage
    {
        public CalcHowManyTurnips()
        {
            InitializeComponent();
        }

        private void ValueChanged(object sender, Syncfusion.SfNumericTextBox.XForms.ValueEventArgs e)
        {
            if (EntryTurnipPrice.Value == null ||
                 EntryBellsToSpend.Value == null)
                return;

            var turnip = (double)EntryTurnipPrice.Value;
            var bells = (double)EntryBellsToSpend.Value;


            if (turnip == 0 || bells == 0)
                return;

            var total = (int)(bells / (turnip * 10));
            LabelCalculation.Text = $"You can buy {total.ToString("n0")} bundles ({total/10} slots)";
        }
    }
}