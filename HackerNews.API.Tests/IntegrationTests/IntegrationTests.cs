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
        /// Test method to get top 200 new stories from API
        /// This method can be failed if Hack news api dosn't 
        /// return exact 200 stories
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task HackerNews_FetchNewStories_Returns200Stories()
        {
            // Arrange
            var client = _webFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/newstories");

            // Assert
            List<Story>? responseContent = JsonSerializer.Deserialize<List<Story>>(await response.Content.ReadAsStringAsync());

            Assert.IsNotNull(responseContent);
            Assert.AreEqual(200, responseContent.Count);
        }
    }
}
