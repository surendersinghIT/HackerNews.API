using HackerNews.API.Data.Models;
using Refit;

namespace HackerNews.API.Service.API
{
    /// <summary>
    /// Interface for calling Hacker news api per the requirement
    /// </summary>
    public interface IHackerNewsApi
    {
        [Get("/newstories.json")]
        Task<int[]> GetNewStories();

        [Get("/item/{id}.json")]
        Task<Story> GetStoryById(int id);
    }
}
