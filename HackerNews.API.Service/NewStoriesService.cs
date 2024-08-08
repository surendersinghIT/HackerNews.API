using HackerNews.API.Data.Models;
using HackerNews.API.Service.API;
using HackerNews.API.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HackerNews.API.Service
{
    public class NewStoriesService : INewStoriesService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHackerNewsApi _hackerNewsApi;
        private readonly ILogger<NewStoriesService> _logger;


        public NewStoriesService([FromServices] IHackerNewsApi hackerNewsApi,
            IMemoryCache memoryCache, ILogger<NewStoriesService> logger)
        {
            _hackerNewsApi = hackerNewsApi;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Get top 200 new stories from Hacker News api
        /// It also get story details from Hacker news api
        /// for top 200 stories
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Story>> GetNewStories()
        {
            try
            {

                _logger.LogInformation("{0} | Fetching New Stories >>>", DateTime.Now.ToString());
                var newStories = await _hackerNewsApi.GetNewStories();
                var newStoriesDetails = new List<Story>();
                // Take only first 200 new stories per requirement
                // First check in memory cache if stories data already present 
                // and if not then get story data only for new ids
                List<Story>? cachedStories;
                int topCachedStoryId = 0;
                if (_memoryCache.TryGetValue(Constants.NewStoriesMemoryCacheKey, out cachedStories))
                {
                    if (cachedStories != null)
                    {
                        topCachedStoryId = cachedStories.First().Id;
                        newStoriesDetails.AddRange(cachedStories);
                    }
                }
                _logger.LogInformation("Fetching New Stories Data for first 200 stories >>>");

                // Select only latest stories to get details and it should not be more than 200
                var tasks = newStories.TakeWhile(storyId => storyId > topCachedStoryId)
                    .Take(200)
                    .Select(async storyId =>
                {
                    Story story = await GetStoryDetails(storyId);
                    newStoriesDetails.Add(story);
                }).ToList();

                // Wait to complete getting details for all 200 stories
                Task.WaitAll(tasks.ToArray());

                // Final latest 200 stories
                List<Story> finalStories = newStoriesDetails.Take(200).ToList();
                // Save latest stories to Cache 
                _memoryCache.Set(Constants.NewStoriesMemoryCacheKey, finalStories);
                // Cache Refresh time to avoid re running Background Job immediately
                _memoryCache.Set(Constants.NewStoriesMemoryRefreshKey, DateTime.Now);
                _logger.LogInformation("Fetching New Stories Data for first 200 stories <<<");
                return finalStories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Story dateils from Hacker News API based on sotry ID
        /// </summary>
        /// <param name="storyId"></param>
        /// <returns>Story</returns>
        private async Task<Story> GetStoryDetails(int storyId)
        {
            Story story;
            _logger.LogInformation("{0} | Fetching New Stories Data for ID : {1} >>>", DateTime.Now.ToString(), storyId);
            story = await _hackerNewsApi.GetStoryById(storyId);
            _logger.LogInformation("{0} | Fetching New Stories Data for ID : {1} <<<", DateTime.Now.ToString(), storyId);
            return story;
        }
    }
}
