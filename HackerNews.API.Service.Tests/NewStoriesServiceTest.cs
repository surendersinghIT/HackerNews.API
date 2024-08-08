using HackerNews.API.Data.Models;
using HackerNews.API.Service.API;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace HackerNews.API.Service.Tests
{

    /// <summary>
    /// Test class to test new stories service
    /// </summary>
    [TestClass]
    public class NewStoriesServiceTest
    {
        private NewStoriesService _newStoriesService;
        private Mock<IHackerNewsApi> _mockHackerNewsApi;
        private Mock<ILogger<NewStoriesService>> _mockLogger;
        private Mock<IMemoryCache> _mockMemoryCache;

        public NewStoriesServiceTest()
        {
            _mockHackerNewsApi = new Mock<IHackerNewsApi>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<NewStoriesService>>();
            _newStoriesService = new NewStoriesService(
                _mockHackerNewsApi.Object,
                _mockMemoryCache.Object,
                _mockLogger.Object);
        }
        /// <summary>
        /// New Stories Service - Returns stories correct data test
        /// </summary>
        /// <returns></returns>        
        [TestMethod]
        public async Task GetNewStories_ReturnsStories_CorrectData()
        {
            //Arrange
            var firstStory = new Story() { Title = "First Story" };
            var secondStory = new Story() { Title = "Second Story" };
            var outputs = new List<Story> { firstStory, secondStory };
            object? expectedCacheValue = null;

            _mockMemoryCache.Setup(s => s.TryGetValue(It.IsAny<string>(), out expectedCacheValue)).Returns(false);
            _mockMemoryCache.Setup(s => s.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);
            _mockHackerNewsApi.Setup(s => s.GetNewStories()).ReturnsAsync(new int[] { 1, 2 });
            _mockHackerNewsApi.Setup(s => s.GetStoryById(It.Is<int>(idVal => idVal == 1))).ReturnsAsync(firstStory);
            _mockHackerNewsApi.Setup(s => s.GetStoryById(It.Is<int>(idVal => idVal == 2))).ReturnsAsync(secondStory);

            //Act
            var stories = await _newStoriesService.GetNewStories();

            //Asset
            Assert.IsNotNull(stories);
            Assert.AreEqual(2, stories.Count());
            Assert.AreEqual("First Story", stories.First().Title);
            Assert.AreEqual("Second Story", stories.Last().Title);
        }

        /// <summary>
        /// New Stories Service - When Cache is empty
        /// </summary>
        /// <returns></returns>        
        [TestMethod]
        public async Task GetNewStories_ShouldReturnStories_WhenCacheIsEmpty()
        {
            // Arrange
            int[] storyIds = Enumerable.Range(1, 200).ToArray();
            var stories = storyIds.Select(id => new Story { Id = id }).ToList();
            _mockHackerNewsApi.Setup(api => api.GetNewStories()).ReturnsAsync(storyIds);
            _mockHackerNewsApi.Setup(api => api.GetStoryById(It.IsAny<int>())).ReturnsAsync((int id) => new Story { Id = id });

            object cacheValue;
            _mockMemoryCache.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);
            _mockMemoryCache.Setup(s => s.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);

            // Act
            var result = await _newStoriesService.GetNewStories();

            // Assert
            Assert.AreEqual(200, result.Count());
            _mockHackerNewsApi.Verify(api => api.GetNewStories(), Times.Once);
            _mockHackerNewsApi.Verify(api => api.GetStoryById(It.IsAny<int>()), Times.Exactly(200));
        }

        [TestMethod]
        public async Task GetNewStories_ShouldReturnCachedStories_WhenCacheIsNotEmpty()
        {
            // Arrange
            int[] storyIds = Enumerable.Range(1, 200).ToArray();
            var cachedStories = Enumerable.Range(1, 200).Select(id => new Story { Id = id }).ToList();
            object cacheValue = cachedStories;
            _mockMemoryCache.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);
            _mockMemoryCache.Setup(s => s.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>); 
            _mockHackerNewsApi.Setup(api => api.GetNewStories()).ReturnsAsync(storyIds);
            _mockHackerNewsApi.Setup(api => api.GetStoryById(It.IsAny<int>())).ReturnsAsync((int id) => new Story { Id = id });

            // Act
            var result = await _newStoriesService.GetNewStories();

            // Assert
            Assert.AreEqual(200, result.Count());
        }

        [TestMethod]
        public async Task GetNewStories_ShouldHandleException_WhenErrorOccurs()
        {
            // Arrange
            var exceptionMessage = "An error occurred";
            _mockHackerNewsApi.Setup(api => api.GetNewStories()).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _newStoriesService.GetNewStories());
            Assert.AreEqual(exceptionMessage, exception.Message);
           
        }

    }
}