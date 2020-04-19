using System;
namespace TurnipTracker.Shared
{
    public class Friend
    {
        // PartitionKey
        public Guid RequesterPublicKey { get; set; }

        // RowKey
        public Guid RequesteePublicKey { get; set; }
    }
}
