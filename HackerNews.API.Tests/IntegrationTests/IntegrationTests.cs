using HackerNews.API.Data.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace HackerNews.API.Tests.IntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        private readonly WebApplicationFactory<Program> _webFactory;
        public IntegrationTests()
        {
            _webFactory = new TestWebAppFactory<Program>();
        }

        /// <summary>
        /// Test method to get top upto 200 new stories from API
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task HackerNews_FetchNewStories_ReturnsMax200Stories()
        {
            // Arrange
            var client = _webFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/newstories");
            var responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            List<Story>? responseContent = JsonSerializer.Deserialize<List<Story>>(responseData);

            Assert.IsNotNull(responseContent);
            Assert.IsTrue(200 >= responseContent.Count, "Total Number of new stories are not more than 200");
        }
    }
}
