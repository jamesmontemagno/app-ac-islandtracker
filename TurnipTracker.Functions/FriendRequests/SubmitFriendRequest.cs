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
using System.Linq;

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
                string.IsNullOrWhiteSpace(friendRequest.MyPublicKey) ||
                string.IsNullOrWhiteSpace(friendRequest.FriendPublicKey))
            {
                return new BadRequestErrorMessageResult("Invalid data to process request");
            }

            try
            {
                if(await Utils.ReachedMaxFriends(friendTable, friendRequest.MyPublicKey, friendRequest.ProUser))
                {
                    if (friendRequest.ProUser)
                        return new BadRequestErrorMessageResult("You have reached the max friend count at this time.");
                    else
                        return new BadRequestErrorMessageResult("You have reached the max friend count at this time. Please upgrade to Island Tracker Pro.");
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
                log.LogInformation("User doesn't exist: " + ex.Message);
                return new BadRequestErrorMessageResult("Unable to locate your user account.");
            }

            try
            {
                var requester = friendRequest.MyPublicKey;
                var requestee = friendRequest.FriendPublicKey;
                if (await Utils.HasFriend(friendTable, requester, requestee))
                    return new BadRequestErrorMessageResult("You are already friends!");

                // Create the InsertOrReplace table operation
                var insertOperation = TableOperation.InsertOrMerge(new FriendRequestEntity
                {
                    PartitionKey = requestee,
                    RowKey = requester
                });

                var result = await friendRequestTable.ExecuteAsync(insertOperation);
                if (result == null)
                    return new InternalServerErrorResult();

            }
            catch (Exception ex)
            {
                log.LogInformation($"Error {nameof(SubmitFriendRequest)} - Error: " + ex.Message);
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("Friend Request Created");
        }
    }
}
