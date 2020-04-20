using System;
using System.Collections.Generic;
using System.Text;

namespace TurnipTracker.Model
{
    public class ChartDataModel
    {
        public string Name { get; set; }
        public string Day { get; set; }

        public double Low { get; set; }

        public double High { get; set; }
        public double PurchasePrice { get; set; }

        public bool Purchased { get; set; }
        public bool Prediction { get; set; }

        public ChartDataModel(string name, string day, double high, double low)
        {
            Name = name;
            Day = day;
            Low = low;
            High = high;
            Prediction = true;
        }

        public ChartDataModel(string name, string day, double purchasePrice)
        {
            Name = name;
            Day = day;
            PurchasePrice = Low = High = purchasePrice;
            Purchased = true;
        }
    }
}
