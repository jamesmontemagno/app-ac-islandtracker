using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace TurnipTracker.Shared
{
    /// <summary>
    /// This is a friend request record
    /// It is never exposed to the client and only for internal.
    /// </summary>
    public class FriendRequestEntity : TableEntity
    {
        public FriendRequestEntity()
        {

        }
        public FriendRequestEntity(string requesterPublicKey, string requesteePublicKey)
        {
            PartitionKey = requesteePublicKey;
            RowKey = requesterPublicKey;
        }

        /// <summary>
        /// This person has a friend code and is requesting to be
        /// friends and the Requestee need to accept it
        /// </summary>
        /// PartitionKey
        public string ApproverPublicKey => PartitionKey;

        /// <summary>
        /// This is the person that shared the code, and has to do final approval
        /// </summary>
        /// RowKey
        public string RequesterPublicKey => RowKey;

    }
}
