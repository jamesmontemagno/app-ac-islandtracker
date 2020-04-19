using System;
namespace TurnipTracker.Shared
{
    /// <summary>
    /// This is a friend request that will be visible to the
    /// person that needs to accept it, not in the database at all
    /// </summary>
    public class PendingFriendRequest
    {
        public string Name { get; set; }
        public string IslandName { get; set; }
        public Guid RequesterPublicKey { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
