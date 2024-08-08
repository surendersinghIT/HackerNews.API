using HackerNews.API.BackgroundServices;
using HackerNews.API.Service.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace HackerNews.API.Tests.UnitTests.BackgroundServices
{
    [TestClass]
    public class TimedHostedServiceTest
    {
        private TimedHostedService _timedHostedService;
        private Mock<INewStoriesService> _mockNewStoriesService;
        private Mock<IMemoryCache> _mockMemoryCache;
        private Mock<ILogger<TimedHostedService>> _mockLogger;

        public TimedHostedServiceTest() {
            _mockLogger = new Mock<ILogger<TimedHostedService>>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _mockNewStoriesService = new Mock<INewStoriesService>();
            _timedHostedService = new TimedHostedService(_mockNewStoriesService.Object, 
                _mockMemoryCache.Object, _mockLogger.Object);
           
        }

        [TestMethod]
        public void StartReturns_CompletedTask_IfLongRunningTaskIs_Complete()
        {
            //Arrange 
            CancellationToken cancellationToken = new CancellationToken();
            var task = _timedHostedService.StartAsync(cancellationToken);

            //Act
            var cancelledTask = _timedHostedService.StopAsync(cancellationToken);
            cancelledTask.Wait();

            //Assert
            Assert.IsTrue(task.IsCompleted);
        }

    }
}
