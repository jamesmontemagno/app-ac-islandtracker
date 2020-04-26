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
using System.Web.Http;
using System.Linq;

namespace TurnipTracker.Functions
{
    public static class UpdateTurnipPrices
    {
        [FunctionName(nameof(UpdateTurnipPrices))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
            [Table("User")] CloudTable cloudTable,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger {nameof(UpdateTurnipPrices)} function processed a request.");


            var privateKey = Utils.ParseToken(req);
            if (privateKey == null)
                return new UnauthorizedResult();

            TurnipUpdate turnipUpdate = null;

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                turnipUpdate = JsonConvert.DeserializeObject<TurnipUpdate>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogInformation("Unable to deserialize turnip update: " + ex.Message);

            }

            if (turnipUpdate == null || string.IsNullOrWhiteSpace(turnipUpdate.PublicKey))
            {
                return new BadRequestResult();
            }

            UserEntity userEntity = null;
            try
            {
                userEntity = await Utils.FindUserEntity(cloudTable, privateKey, turnipUpdate.PublicKey);
            }
            catch (Exception ex)
            {
                //user does not exist? correct error?
                return new InternalServerErrorResult();
            }

            if(userEntity == null)
                return new BadRequestResult();

            userEntity.AMPrice = turnipUpdate.AMPrice;
            userEntity.PMPrice = turnipUpdate.PMPrice;
            userEntity.BuyPrice = turnipUpdate.BuyPrice;
            userEntity.TurnipUpdateDayOfYear = turnipUpdate.DayOfYear;
            userEntity.TurnipUpdateYear = turnipUpdate.Year;
            userEntity.TurnipUpdateTimeUTC = turnipUpdate.TurnipUpdateTimeUTC;

            try
            {
                await Utils.MergeUserEntity(cloudTable, userEntity);
            }
            catch (Exception ex)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("User Turnip Prices Updated");
        }
    }
}
