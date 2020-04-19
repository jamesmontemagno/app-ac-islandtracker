using System;
namespace TurnipTracker.Shared
{
    /// <summary>
    /// This is a friend request record
    /// It is never exposed to the client and only for internal.
    /// </summary>
    public class FriendRequest
    {
        /// <summary>
        /// This person has a friend code and is requesting to be
        /// friends and the Requestee need to accept it
        /// </summary>
        /// PartitionKey
        public Guid RequesterPublicKey { get; set; }

        /// <summary>
        /// This is the person that shared the code, and has to do final approval
        /// </summary>
        /// RowKey
        public Guid RequesteePublicKey { get; set; }

        public DateTime CreationDateUTC { get; set; }
    }
}
