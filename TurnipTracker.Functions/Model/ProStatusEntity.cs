using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace TurnipTracker.Functions.Model
{
    public class ProStatusEntity : TableEntity
    {
        public ProStatusEntity()
        {

        }
        public ProStatusEntity(string publicKey, string privateKey)
        {
            PartitionKey = publicKey;
            RowKey = privateKey;
        }

        public string Receipt { get; set; }
    }
}
