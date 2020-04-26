using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using TurnipTracker.Shared;
using System.Web.Http;
using TurnipTracker.Functions.Helpers;

namespace TurnipTracker.Functions
{
    public static class SubmitFriendRequest
    {
        [FunctionName(nameof(SubmitFriendRequest))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Table("FriendRequest")] CloudTable friendRequestTable,
            [Table("Friend")] CloudTable friendTable,
            [Table("User")] CloudTable userTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(SubmitFriendRequest)} function processed a request.");


            var privateKey = Utils.ParseToken(req);
            if (privateKey == null)
                return new UnauthorizedResult();

            FriendRequest friendRequest = null;

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                friendRequest = JsonConvert.DeserializeObject<FriendRequest>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogInformation("Unable to deserialize user: " + ex.Message);

            }

            if (friendRequest == null ||
                string.IsNullOrWhiteSpace(friendRequest.RequesteePublicKey) ||
                string.IsNullOrWhiteSpace(friendRequest.RequesterPublicKey))
            {
                return new BadRequestResult();
            }

            try
            {
                var user = await Utils.FindUserEntity(userTable, privateKey, friendRequest.RequesterPublicKey);
                if (user == null)
                    return new BadRequestResult();
            }
            catch (Exception ex)
            {
                log.LogInformation("User doesn't exist: " + ex.Message);
                return new BadRequestResult();
            }

            try
            {
                var requester = friendRequest.RequesterPublicKey;
                var requestee = friendRequest.RequesteePublicKey;
                if (await Utils.HasFriend(friendTable, requester, requestee))
                    return new BadRequestResult();

                var batch = new TableBatchOperation();

                // Create the InsertOrReplace table operation
                var insertOperation = TableOperation.InsertOrMerge(new FriendRequestEntity
                {
                    PartitionKey = requester,
                    RowKey = requestee
                });

                batch.Add(insertOperation);
              
                // Create the InsertOrReplace table operation
                var insertOperation2 = TableOperation.InsertOrMerge(new FriendRequestEntity
                {
                    PartitionKey = requestee,
                    RowKey = requester
                });

                batch.Add(insertOperation2);
                // Execute the operation.
                var result = await friendRequestTable.ExecuteBatchAsync(batch);
                if (result == null || result.Count != 2)
                    return new InternalServerErrorResult();

            }
            catch (Exception ex)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("Friend Request Created");
        }
    }
}
