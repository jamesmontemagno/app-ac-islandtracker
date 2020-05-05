using System.Collections.Generic;
using MvvmHelpers;
using TurnipTracker.Shared;

namespace TurnipTracker.Model
{
    public class FriendGroup : ObservableRangeCollection<FriendStatus>
    {
        public string Key { get; }
        /// <summary>
		/// Returns list of items in the grouping.
		/// </summary>
		public new IList<FriendStatus> Items => base.Items;
        public FriendGroup(string key, IEnumerable<FriendStatus> items)
        {
            Key = key;
            AddRange(items);
        }
    }
}
