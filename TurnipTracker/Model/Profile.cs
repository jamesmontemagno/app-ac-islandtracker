using System;
using MvvmHelpers;
using Newtonsoft.Json;

namespace TurnipTracker.Model
{
    public class Profile : ObservableObject
    {
        [JsonIgnore]
        public Action SaveProfileAction { get; set; }

        string name = string.Empty;
        public string Name
        {
            get => name;
            set
            {
                var force = false;
                if (value.Length > 15)
                {
                    value = value.Substring(0, 15);
                    force = true;
                }
                SetProperty(ref name, value, onChanged: SaveProfileAction);
                if (force)
                    OnPropertyChanged();
            }
        }

        string friendCode = string.Empty;
        public string FriendCode
        {
            get => friendCode;
            set
            {                
                SetProperty(ref friendCode, value, onChanged: SaveProfileAction);
            }
        }

        string islandName = string.Empty;
        public string IslandName
        {
            get => islandName;
            set
            {
                var force = false;
                if (value.Length > 15)
                {
                    value = value.Substring(0, 15);
                    force = true;
                }
                SetProperty(ref islandName, value, onChanged: SaveProfileAction);
                if (force)
                    OnPropertyChanged();
            }
        }

        string status = "😍";
        public string Status
        {
            get => status;
            set
            {
                var force = false;
                if (value.Length > 4)
                {
                    value = value.Substring(0, 4);
                    force = true;
                }
                SetProperty(ref status, value, onChanged: SaveProfileAction);
                if (force)
                    OnPropertyChanged();
            }
        }

        int fruit = (int)Model.Fruit.Apple;
        public int Fruit
        {
            get => fruit;
            set => SetProperty(ref fruit, value, onChanged: SaveProfileAction);
        }

        int gateStatus = (int)Model.GateStatus.Closed;
        public int GateStatus
        {
            get => gateStatus;
            set => SetProperty(ref gateStatus, value, onChanged: SaveProfileAction);
        }

        string dodoCode = string.Empty;
        public string DodoCode
        {
            get => dodoCode;
            set => SetProperty(ref dodoCode, value, onChanged: SaveProfileAction);
        }

        double gatesOpenLength = 0.5;
        public double GatesOpenLength
        {
            get => gatesOpenLength;
            set
            {
                GateClosesAtUTC = DateTime.UtcNow.AddMinutes(60 * value);
                SetProperty(ref gatesOpenLength, value, onChanged: SaveProfileAction);
            }
        }

        [JsonIgnore]
        public DateTime GateClosesAtUTC { get; set; } = DateTime.UtcNow;

        public string TimeZone { get; set; }
    }
}
