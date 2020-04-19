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
                if(value.Length > 15)
                {
                    value = value.Substring(0, 15);
                }
                SetProperty(ref name, value, onChanged: SaveProfileAction);
            }
        }

        string islandName = string.Empty;
        public string IslandName
        {
            get => islandName;
            set
            {
                if (value.Length > 15)
                {
                    value = value.Substring(0, 15);
                }
                SetProperty(ref islandName, value, onChanged: SaveProfileAction);
            }
        }

        string status = "😍";
        public string Status
        {
            get => status;
            set
            {
                if (value.Length > 4)
                {
                    value = value.Substring(0, 4);
                }
                SetProperty(ref status, value, onChanged: SaveProfileAction);
            }
        }

        int fruit = (int)Model.Fruit.Apple;
        public int Fruit
        {
            get => fruit;
            set => SetProperty(ref fruit, value, onChanged: SaveProfileAction);
        }
    }
}
