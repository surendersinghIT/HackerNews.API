using Microsoft.AspNetCore.Mvc;
using HackerNews.API.Service.Interface;
using HackerNews.API.Data.Models;

namespace HackerNews.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewStoriesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly INewStoriesService _newStoriesService;
        public NewStoriesController(INewStoriesService newStoriesService, ILogger<NewStoriesController> logger)
        {
            _newStoriesService = newStoriesService;
            _logger = logger;
        }
        /// <summary>
        /// Get New stories from Hacker news API
        /// It returns only top 200 new stories per requirement
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces(typeof(IEnumerable<Story>))]
        public async Task<IEnumerable<Story>> GetNewStories()
        {
            _logger.LogInformation(message: "{0} | NewStories - Get started >>>", DateTime.Now.ToString());
            var result = await _newStoriesService.GetNewStories();
            _logger.LogInformation(message: "{0} | NewStories - Get completed <<<", DateTime.Now.ToString());
            return result;
        }


    }
}
