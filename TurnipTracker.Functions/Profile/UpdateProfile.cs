using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TurnipTracker.Functions.Helpers;
using Microsoft.WindowsAzure.Storage.Table;
using TurnipTracker.Shared;
using System.Linq;
using System.Web.Http;

namespace TurnipTracker.Functions
{
    public static class UpdateProfile
    {
        [FunctionName(nameof(UpdateProfile))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
            [Table("User")] CloudTable cloudTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(UpdateProfile)} function processed a request.");


            var privateKey = Utils.ParseToken(req);
            if (privateKey == null)
                return new UnauthorizedResult();

            User user = null;

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                user = JsonConvert.DeserializeObject<User>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogWarning("Unable to deserialize user: " + ex.Message);

            }

            if (user == null ||
                string.IsNullOrWhiteSpace(user.PublicKey) ||
                string.IsNullOrWhiteSpace(user.Name) ||
                string.IsNullOrWhiteSpace(user.IslandName) ||
                user.TimeZone == null)
            {
                return new BadRequestErrorMessageResult("Invalid data to process request");
            }

            UserEntity userEntity = null;
            try
            {
                userEntity = await Utils.FindUserEntity(cloudTable, privateKey, user.PublicKey);
            }
            catch (Exception ex)
            {
                log.LogError($" User doesn't exist - Error {nameof(UpdateProfile)} - Error: " + ex.Message);
                //user does not exist? correct error?
                return new BadRequestErrorMessageResult("Unable to locate your user account.");
            }

            if (userEntity == null)
                return new BadRequestErrorMessageResult("Unable to locate your user account.");

            var encryptedFriendCode = string.Empty;
            try
            {
                encryptedFriendCode = Cipher.Encrypt(user.FriendCode, Utils.FriendCodePassword, user.PublicKey);
            }
            catch (Exception ex)
            {
                log.LogError("Unable to decrypt friendcode: " + ex.Message);
            }
            userEntity.Name = user.Name;
            userEntity.IslandName = user.IslandName;
            userEntity.Fruit = user.Fruit;
            userEntity.TimeZone = user.TimeZone;
            userEntity.Status = user.Status ?? string.Empty;
            userEntity.FriendCode = encryptedFriendCode;
            userEntity.GateClosesAtUTC = user.GateClosesAtUTC.HasValue ? user.GateClosesAtUTC.Value : DateTime.UtcNow;
            userEntity.GateStatus = user.GateStatus;
            userEntity.DodoCode = user.DodoCode;

            try
            {
                await Utils.MergeUserEntity(cloudTable, userEntity);
            }
            catch (Exception ex)
            {
                log.LogError($"Error {nameof(UpdateProfile)} - Error: " + ex.Message);
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("User Updated");
        }


    }
}
