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

namespace TurnipTracker.Functions.FriendRequests
{
    public static class RejectFriendRequest
    {
        [FunctionName(nameof(RejectFriendRequest))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Table("FriendRequest")] CloudTable friendRequestTable,
            [Table("User")] CloudTable userTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(RejectFriendRequest)} function processed a request.");


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
                log.LogInformation("Unable to deserialize friend request: " + ex.Message);

            }

            if (friendRequest == null ||
                string.IsNullOrWhiteSpace(friendRequest.MyPublicKey) ||
                string.IsNullOrWhiteSpace(friendRequest.FriendPublicKey))
            {
                return new BadRequestResult();
            }

            try
            {
                var user = await Utils.FindUserEntity(userTable, privateKey, friendRequest.MyPublicKey);
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
                var myPublicKey = friendRequest.MyPublicKey;
                var friendPublicKey = friendRequest.FriendPublicKey;

                var removeOperation1 = TableOperation.Delete(new FriendRequestEntity
                {
                    PartitionKey = myPublicKey,
                    RowKey = friendPublicKey,
                    ETag = "*"
                });


                // Execute the operation.
                var result = await friendRequestTable.ExecuteAsync(removeOperation1);
                if (result == null)
                    return new InternalServerErrorResult();

            }
            catch (Exception ex)
            {
                log.LogInformation($"Error {nameof(RejectFriendRequest)} - Error: " + ex.Message);
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("Friend Request Removed");
        }
    }
}
