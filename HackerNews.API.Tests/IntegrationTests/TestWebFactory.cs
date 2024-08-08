using HackerNews.API.BackgroundServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HackerNews.API.Tests.IntegrationTests
{
    public class TestWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        /// <summary>
        /// We are remoming background Timed service so that we can skip running it
        /// while testing
        /// </summary>
        /// <param name="builder"></param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(TimedHostedService));

                if (descriptor != null)
                    services.Remove(descriptor);

            });
        }
    }
}
