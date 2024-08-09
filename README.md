Hacker News API is based on https://hacker-news.firebaseio.com/v0/newstories.json , open source api.

Steps to run:
1. Download code
2. Get All dependencies
3. Make sure to have .Net8 and Visual Studio 2022 bening used for development Env.
4. Run all test cases
5. Run the application


Features:
1. It fetches top 200 news stories from API and cache into memory
2. A background service (timed service) configured to run every 5 min and get new stories and store to memory
3. so that our memory cache can be 5 min older in max case
4. Background service checks for last refresh time of memory cache and skip if it is less than a min
5. Background service bening used to referesh data to avoid long response time
