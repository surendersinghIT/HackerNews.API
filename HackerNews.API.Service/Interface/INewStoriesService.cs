using HackerNews.API.Data.Models;

namespace HackerNews.API.Service.Interface
{
    /// <summary>
    /// Interface for new stories service
    /// </summary>
    public interface INewStoriesService
    {
        Task<IEnumerable<Story>> GetNewStories();
    }
}
