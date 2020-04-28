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

        public string TimeZone { get; set; }
    }
}
