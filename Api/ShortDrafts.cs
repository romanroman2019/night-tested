using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Models;
using System;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using StaticWebAppAuthentication.Api;
using StaticWebAppAuthentication.Models;
using Newtonsoft.Json;

namespace Api
{
    public static class ShortDrafts
    {
        public static object UriFactory { get; private set; }
        [FunctionName($"{nameof(ShortDrafts)}_Get")]
        public static IActionResult GetAllShortDrafts(
            [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "shortdrafts")]
            HttpRequest req,
            [CosmosDB(
            "SwaBlog",
            "ShortContainer",
            Connection = "CosmosDbConnectionString",
            SqlQuery = @"
                SELECT
                s.id,
                s.Port,
                s.Author,
                s.PublishedDate,
                s.Condition,
                s.ObservedFwd,
                s.ObservedAft,
                s.LoamasterFwd,
                s.LoadmasterAft,
                s.SensorFwd,
                s.SensorAft
                FROM s")]
            IEnumerable<ShortDraft> shortDrafts,
            ILogger log)
        {
            return new OkObjectResult(shortDrafts);
        }

            [FunctionName($"{nameof(ShortDrafts)}_GetId")]
    public static IActionResult GetShortDraft(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "shortdrafts/{author}/{id}")]
        HttpRequest req,
        [CosmosDB(
            "SwaBlog",
            "ShortContainer",
            Connection = "CosmosDbConnectionString",
            SqlQuery = @"SELECT
                s.id,
                s.Port,
                s.Author,
                s.PublishedDate,
                s.Condition,
                s.ObservedFwd,
                s.ObservedAft,
                s.LoadmasterFwd,
                s.LoadmasterAft,
                s.SensorFwd,
                s.SensorAft
                FROM s
                WHERE s.id = {id} and s.Author={author}")]
        IEnumerable<ShortDraft> shortdrafts,
        ILogger log)
    {
        if (shortdrafts.ToArray().Length == 0)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(shortdrafts.First());
    }

    [FunctionName($"{nameof(ShortDraft)}_Put")]
    public static IActionResult PutShortDraft(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "put",
            Route = "shortdrafts")]
        ShortDraft updatedShortDraft,
        [CosmosDB(
            "SwaBlog",
            "ShortContainer", 
            Connection = "CosmosDbConnectionString",
            Id = "{Id}",
            PartitionKey ="{Author}")]
        ShortDraft currentShortDraft,
        [CosmosDB(
            "SwaBlog",
            "ShortContainer",
            Connection = "CosmosDbConnectionString")]
            out dynamic savedShortDraft,
            ILogger log)
        {
            if (currentShortDraft is null)
            {
                savedShortDraft = null;
                return new NotFoundResult();
            }

            savedShortDraft = new
            {
                id = updatedShortDraft.Id.ToString(),
                PublishedDate= updatedShortDraft.PublishedDate,
                Author = updatedShortDraft.Author,
                Port = updatedShortDraft.Port,
                Condition = updatedShortDraft.Condition,
                ObservedFwd = updatedShortDraft.ObservedFwd,
                ObservedAft=updatedShortDraft.ObservedAft,
                LoadmasterFwd=updatedShortDraft.LoadmasterFwd,
                LoadmasterAft=updatedShortDraft.LoadmasterAft,
                SensorFwd=updatedShortDraft.SensorFwd,
                SensorAft=updatedShortDraft.SensorAft,
                Status = 2
            };

            return new NoContentResult();
        }

        [FunctionName($"{nameof(ShortDraft)}_Post")]
        public static IActionResult PostShortDraft(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "shortdrafts"
            )]
            ShortDraft shortDraft,
            HttpRequest request,
            [CosmosDB(
                "SwaBlog",
                "ShortContainer",
                Connection = "CosmosDbConnectionString"
            )]
            out dynamic savedShortDraft,
            ILogger log)
   {
        if (shortDraft.Id != default)
        {
            savedShortDraft = null;
            return new BadRequestObjectResult("id must be null");
        }

        //var clientPrincipal = StaticWebAppApiAuthorization.ParseHttpHeaderForClientPrincipal(request.Headers);
        var clientPrincipal = StaticWebAppApiAuthorization.ParseHeaderForClientPrincipal(request.Headers);

        shortDraft.Id = Guid.NewGuid();
        shortDraft.Author = clientPrincipal.UserDetails;

        savedShortDraft = new
        {
                id = shortDraft.Id.ToString(),
                PublishedDate= shortDraft.PublishedDate,
                Author = shortDraft.Author,
                Port = shortDraft.Port,
                Condition = shortDraft.Condition,
                ObservedFwd = shortDraft.ObservedFwd,
                ObservedAft=shortDraft.ObservedAft,
                LoadmasterFwd=shortDraft.LoadmasterFwd,
                LoadmasterAft=shortDraft.LoadmasterAft,
                SensorFwd=shortDraft.SensorFwd,
                SensorAft=shortDraft.SensorAft,
                Status = 2
        };

        return new OkObjectResult(shortDraft);
    }
            


    }
}

