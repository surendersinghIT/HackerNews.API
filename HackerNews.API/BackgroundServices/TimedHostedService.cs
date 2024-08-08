namespace HackerNews.API.BackgroundServices
{
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using HackerNews.API.Service.Interface;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// This is a timed hosted service executing every five minute.
    /// It will run and cache newest stories so that we do not 
    /// have to wait longer to get result
    /// </summary>
    public class TimedHostedService : BackgroundService
    {
        private readonly ILogger<TimedHostedService> _logger;
        private readonly INewStoriesService _newStoriesService;
        private readonly IMemoryCache _memoryCache;
        private Timer _timer;

        public TimedHostedService(INewStoriesService newStoriesService, 
            IMemoryCache memoryCache, ILogger<TimedHostedService> logger)
        {
            _newStoriesService = newStoriesService;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Execute Async
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            _logger.LogDebug("Background Timed (Every 5 Minutes) service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        /// <summary>
        /// DoWrok
        /// It run New stories service to fetch latest 200 stories and cache to memory
        /// It check for cache memory refresh time and fetch again if time is more than 1 min
        /// </summary>
        /// <param name="state"></param>
        private async void DoWork(object state)
        {
            _logger.LogDebug(message: "{0} | Background Timed service started >>>", DateTime.Now.ToString());
            // Lets find out previous cache refresh time and skip if it is less than 3 min
            DateTime? cachedRefreshTime;
            bool skipRun = false;
            if(_memoryCache.TryGetValue(Constants.NewStoriesMemoryRefreshKey, out cachedRefreshTime))
            {
                if(cachedRefreshTime != null)
                {
                    // Skip run if cached refreshed previoulsy within 1 min
                    if(DateTime.Now.Subtract(cachedRefreshTime.Value).TotalMinutes < 1)
                    {
                        skipRun = true;
                    }
                }
            }
            if (!skipRun)
            {
                // Output not consumed here as this just to update our Cache
                await _newStoriesService.GetNewStories();
            } else
            {
                _logger.LogDebug(message: "{0} | Background Timed service run skipped", DateTime.Now.ToString());
            }
            _logger.LogDebug(message: "{0} | Background Timed service Completed <<<", DateTime.Now.ToString());

        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            await base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }

}
