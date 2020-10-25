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
using System.Web.Http;
using TurnipTracker.Shared;
using TurnipTracker.Functions.Helpers;

namespace TurnipTracker.Functions
{
    public static class ApproveFriendRequest
    {
        [FunctionName(nameof(ApproveFriendRequest))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Table("FriendRequest")] CloudTable friendRequestTable,
            [Table("Friend")] CloudTable friendTable,
            [Table("User")] CloudTable userTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(ApproveFriendRequest)} function processed a request.");


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
                log.LogError("Unable to deserialize friend request: " + ex.Message);
            }

            if (friendRequest == null ||
                string.IsNullOrWhiteSpace(friendRequest.MyPublicKey) ||
                string.IsNullOrWhiteSpace(friendRequest.FriendPublicKey))
            {
                return new BadRequestErrorMessageResult("Invalid data to process request");
            }


            try
            {
                if (await Utils.ReachedMaxFriends(friendTable, friendRequest.MyPublicKey, friendRequest.ProUser))
                {
                    return new BadRequestErrorMessageResult("You have reached the max friend count at this time.");
                }
            }
            catch (Exception ex)
            {
                log.LogInformation("Unable to count existing friends: " + ex.Message);
                return new InternalServerErrorResult();
            }

            try
            {
                var user = await Utils.FindUserEntitySlim(userTable, privateKey, friendRequest.MyPublicKey);
                if (user == null)
                    return new BadRequestErrorMessageResult("Unable to locate your user account.");
            }
            catch (Exception ex)
            {
                log.LogError("User doesn't exist: " + ex.Message);
                return new BadRequestErrorMessageResult("Unable to locate your user account.");
            }


            try
            {
                var myPublicKey = friendRequest.MyPublicKey;
                var friendPublicKey = friendRequest.FriendPublicKey;


                // Create the InsertOrReplace table operation
                var insertOperation1 = TableOperation.InsertOrMerge(new FriendEntity
                {
                    PartitionKey = myPublicKey,
                    RowKey = friendPublicKey
                });
                // Execute the operation.
                var result = await friendTable.ExecuteAsync(insertOperation1);
                if (result == null)
                {
                    log.LogWarning("Insertion 1 of friend failed");
                    return new InternalServerErrorResult();
                }

                // Create the InsertOrReplace table operation
                var insertOperation2 = TableOperation.InsertOrMerge(new FriendEntity
                {
                    PartitionKey = friendPublicKey,
                    RowKey = myPublicKey
                });

                // Execute the operation.
                result = await friendTable.ExecuteAsync(insertOperation2);
                if (result == null)
                {
                    log.LogWarning("Insertion 2 of friend failed");
                    return new InternalServerErrorResult();
                }


                var removeOperation1 = TableOperation.Delete(new FriendRequestEntity
                {
                    PartitionKey = myPublicKey,
                    RowKey = friendPublicKey,
                    ETag = "*"
                });
                

                // Execute the operation.
                result = await friendRequestTable.ExecuteAsync(removeOperation1);
                if (result == null)
                {
                    log.LogWarning("Failed to remove friend request.");
                    return new InternalServerErrorResult();
                }

            }
            catch (Exception ex)
            {
                log.LogError($"Error {nameof(ApproveFriendRequest)} - Error: " + ex.Message);
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("Friend Request Approved");
        }
    }
}
