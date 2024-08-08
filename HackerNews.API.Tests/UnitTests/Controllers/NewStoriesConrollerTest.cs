using HackerNews.API.Controllers;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using HackerNews.API.Service.Interface;
using HackerNews.API.Data.Models;
using HackerNews.API.Service;

namespace HackerNews.API.Tests.UnitTests.Controllers
{
    /// <summary>
    /// Test class for New stories Controller
    /// </summary>
    [TestClass]
    public class NewStoriesConrollerTest
    {
        private NewStoriesController _newStoriesController;
        private Mock<INewStoriesService> _mockNewStoriesService;
        private Mock<ILogger<NewStoriesController>> _mockLogger;
        public NewStoriesConrollerTest()
        {
            _mockLogger = new Mock<ILogger<NewStoriesController>>();
            _mockNewStoriesService = new Mock<INewStoriesService>();
            _newStoriesController = new NewStoriesController(_mockNewStoriesService.Object, _mockLogger.Object);
            _newStoriesController.ControllerContext = new ControllerContext();
            
        }

        [TestMethod]
        public async Task Controller_Get_Default_ReturnsEmtpyList()
        {
            //Arrange
            var emptyList = new List<Story>();
            _mockNewStoriesService.Setup(s => s.GetNewStories()).ReturnsAsync(emptyList);

            //Act
            var stories = await _newStoriesController.GetNewStories();

            //Asset
            Assert.IsNotNull(stories);
            Assert.AreEqual(0, stories.Count());
        }

        [TestMethod]
        public async Task Controller_Get_Default_ReturnsSingleStory()
        {
            //Arrange
            var singleStoryList = new List<Story>();
            singleStoryList.Add(new Story());
            _mockNewStoriesService.Setup(s => s.GetNewStories()).ReturnsAsync(singleStoryList);

            //Act
            var stories = await _newStoriesController.GetNewStories();

            //Asset
            Assert.IsNotNull(stories);
            Assert.AreEqual(1, stories.Count());
        }

        [TestMethod]
        public async Task Controller_Get_Default_ReturnsExcception()
        {
            //Arrange
            var exceptionMessage = "An error occurred";
            _mockNewStoriesService.Setup(s => s.GetNewStories()).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _newStoriesController.GetNewStories());
            Assert.AreEqual(exceptionMessage, exception.Message);
        }
    }

    /// exception test case
}
