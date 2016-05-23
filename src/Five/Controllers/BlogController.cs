using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Five.Models;
using Five.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Five.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogManager manager;

        private readonly ILogger<BlogController> logger;

        public BlogController(IBlogManager manager, ILogger<BlogController> logger)
        {
            this.manager = manager;
            this.logger = logger;
        }


        [Route("api/blog"), HttpPost]
        public async void Add()
        {
            logger.LogInformation("Doing a post");
            await manager.AddAsync();
        }

        [Route("api/blog"), HttpGet]
        [Authorize]
        public string Stuff()
        {
            logger.LogInformation("Made it");
            return "Here I am";
        }
    }
}
