using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos;
using Models;
using StaticWebAppAuthentication.Api;

namespace Api
{
    public static class DraftReadings
    {
        public static object UriFactory { get; private set; }

        [FunctionName($"{nameof(DraftReadings)}_Get")]
        public static IActionResult GetAllDraftReadings(
            [HttpTrigger(AuthorizationLevel.Anonymous,
            "get",
            Route = "draftreadings")]
        HttpRequest req,
         [CosmosDB(
            "SwaBlog",
            "DraftsContainer",
            Connection ="CosmosDbConnectionString",
            SqlQuery = @"
                SELECT d.id,
                d.Port,
                d.Author,
                d.DatePublished,
                d.Condition,
                d.ObservedFwd,
                d.ObservedAft,
                d.LoadmasterFwd,
                d.LoadmasterAft,
                d.SensorFwd,
                d.SensorAft,
                d.CorCargo
                FROM d")]
         IEnumerable<DraftReading> draftReadings,
            ILogger log)
        {
            return new OkObjectResult(draftReadings);
        }

        [FunctionName($"{nameof(DraftReadings)}_Post")]
        public static IActionResult PostDraftReading(
    [HttpTrigger(AuthorizationLevel.Anonymous,
            "post",
            Route = "draftreadings")]
    DraftReading draftReading,
    HttpRequest request,
        [CosmosDB(
            "SwaBlog",
            "DraftsContainer",
            Connection ="CosmosDbConnectionString")]
    out dynamic savedDraftReading,
    ILogger log)
        {
            if(draftReading != default)
            {
                savedDraftReading = null;
                return new BadRequestObjectResult("id must be null");
            }

            var clientPrincipal =
                StaticWebAppApiAuthorization.ParseHeaderForClientPrincipal(request.Headers);

            draftReading.Id = Guid.NewGuid();
            draftReading.Author = clientPrincipal.UserDetails;

            savedDraftReading = new
            {
                id = draftReading.Id.ToString(),
                Author = draftReading.Author,
                Port = draftReading.Port,
                DatePublished = draftReading.DatePublished,
                Condition = draftReading.Condition,
                ObservedFwd = draftReading.ObservedFwd,
                ObservedAft = draftReading.ObservedAft,
                LoadmasterFwd = draftReading.LoadmasterFwd,
                LoadmasterAft = draftReading.LoadmasterAft,
                SensorFwd = draftReading.SensorFwd,
                SensorAft = draftReading.SensorAft,
                CorCargo = draftReading.CorCargo
            };

            return new OkObjectResult(draftReading);
        }
    }
}

