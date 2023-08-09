using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

using StaticWebAppAuthentication.Api;
using StaticWebAppAuthentication.Models;

using Models;

namespace Api
{
    public static class BlogPosts
    {
        public static object UriFactory { get; private set; }

        [FunctionName($"{nameof(BlogPosts)}_Get")]
        public static IActionResult GetAllBlogPosts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "blogposts")] HttpRequest req,
            [CosmosDB(
                "SwaBlog",
                "BlogContainer",
                Connection = "CosmosDbConnectionString",
                SqlQuery = @"
                    SELECT
                    c.id,
                    c.Title,
                    c.Author,
                    c.PublishedDate,
                    LEFT(c.BlogPostMarkdown, 500)
                        As BlogPostMarkdown,
                    Length(c.BlogPostMarkdown) <= 500    
                        As PreviewOsComplete,
                    c.Tags
                    FROM c
                    WHERE c.Status = 2")
            ] IEnumerable<BlogPost> blogPosts, ILogger log)
        {
            return new OkObjectResult(blogPosts);
        }

        [FunctionName($"{nameof(BlogPosts)}_GetId")]
        public static IActionResult GetBlogPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "blogposts/{author}/{id}")] HttpRequest req,
            [CosmosDB(
                "SwaBlog",
                "BlogContainer",
                Connection = "CosmosDbConnectionString",
                SqlQuery = @"
                    SELECT
                    c.id,
                    c.Title,
                    c.Author,
                    c.PublishedDate,
                    c.BlogPostMarkdown,
                    c.Status,
                    c.Tags
                    FROM c
                    WHERE c.id={id} and c.Author={author}")]
                IEnumerable<BlogPost> blogPosts, ILogger log)
        {
            if (blogPosts.ToArray().Length == 0)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(blogPosts.First());
        }

        [FunctionName($"{nameof(BlogPosts)}_Post")]
        public static IActionResult PostBlogPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post",
            Route = "blogposts")]
            BlogPost blogPost,
            HttpRequest request,
            [CosmosDB("SwaBlog", "BlogContainer", Connection = "CosmosDbConnectionString")]
            out dynamic saveBlogPost, ILogger log)
        {
            if (blogPost.Id != default)
            {
                saveBlogPost = null;
                return new BadRequestObjectResult("id must be null");
            }

            var clientPrincipal = StaticWebAppApiAuthorization
                .ParseHeaderForClientPrincipal(request.Headers);

            blogPost.Id = Guid.NewGuid();
            blogPost.Author = clientPrincipal.UserDetails;

            saveBlogPost = new
            {
                id = blogPost.Id.ToString(),
                Title = blogPost.Title,
                Author = blogPost.Author,
                PublishedDate = blogPost.PublishedDate,
                Tags = blogPost.Tags,
                BlogPostMarkdown = blogPost.BlogPostMarkdown,
                Status = 2
            };
            return new OkObjectResult(blogPost);
        }

        [FunctionName($"{nameof(BlogPosts)}_Put")]
        public static IActionResult PuBlogPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put",
            Route = "blogposts")]
            BlogPost updatedBlogPost,
            [CosmosDB("SwaBlog", "BlogContainer", Connection = "CosmosDbConnectionString",
            Id = "{Id}", PartitionKey = "{Author}")]
            BlogPost currentBlogPost,
            [CosmosDB("SwaBlog", "BlogContainer", Connection = "CosmosDbConnectionString")]
            out dynamic savedBlogPost, ILogger log)
        {
            if (currentBlogPost is null)
            {
                savedBlogPost = null;
                return new NotFoundResult();
            }

            savedBlogPost = new
            {
                id = updatedBlogPost.Id.ToString(),
                Title = updatedBlogPost.Title,
                Author = updatedBlogPost.Author,
                PublishedDate = updatedBlogPost.PublishedDate,
                Tags = updatedBlogPost.Tags,
                BlogPostMarkdown = updatedBlogPost.BlogPostMarkdown,
                Status = 2
            };

            return new NoContentResult();
        }

        [FunctionName($"{nameof(BlogPosts)}_Delete")]
        public static async Task<IActionResult> DeleteBlogPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete",
            Route = "blogposts/{author}/{id}")]
            HttpRequest request,
            string author,
            string id,
            [CosmosDB("SwaBlog", "BlogContainer", Connection = "CosmosDbConnectionString",
            Id = "{id}", PartitionKey = "{author}")]
            BlogPost currentBlogPost,
            [CosmosDB(Connection = "CosmosDbConnectionString")]
            CosmosClient client, ILogger log)
        {
            if (currentBlogPost is null)
            {
                return new NoContentResult();
            }
            Container container = client.GetDatabase("SwaBlog")
                .GetContainer("BlogContainer");
            await container
                .DeleteItemAsync<BlogPost>(id, new PartitionKey(author));
            return new NoContentResult();
        }
    }
}