using Backend.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class StoryService : IStoryService
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<StoryService> _logger;
        private const string CacheKey = "NewestStories";
        private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private const string HackerNewsApiBase = "https://hacker-news.firebaseio.com/v0";

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public StoryService(IMemoryCache cache, ILogger<StoryService> logger)
        {
            _cache = cache;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        private async Task<List<int>> GetNewestStoryIdsAsync()
        {
            _logger.LogInformation("Fetching newest story IDs from Hacker News API");
            var response = await _httpClient.GetAsync($"{HackerNewsApiBase}/newstories.json");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var ids = JsonSerializer.Deserialize<List<int>>(json, _jsonOptions);
            _logger.LogInformation($"Fetched {ids?.Count ?? 0} story IDs");
            return ids ?? new List<int>();
        }

        private async Task<Story?> GetStoryByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching story details for ID {id}");
            var response = await _httpClient.GetAsync($"{HackerNewsApiBase}/item/{id}.json");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Failed to fetch story ID {id}: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var hnStory = JsonSerializer.Deserialize<HackerNewsStory>(json, _jsonOptions);
            if (hnStory == null || hnStory.Type != "story" || string.IsNullOrEmpty(hnStory.Title))
            {
                _logger.LogInformation($"Story ID {id} is invalid or missing title/type");
                return null;
            }

            return new Story
            {
                Id = hnStory.Id,
                Title = hnStory.Title,
                Url = hnStory.Url
            };
        }

        public async Task<StoryResponse> GetNewestStoriesAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            if (!_cache.TryGetValue(CacheKey, out List<Story> cachedStories))
            {
                _logger.LogInformation("Cache miss: fetching stories from Hacker News API");
                var ids = await GetNewestStoryIdsAsync();
                var stories = new List<Story>();

                var fetchCount = Math.Min(200, ids.Count);
                var tasks = new List<Task<Story?>>();
                for (int i = 0; i < fetchCount; i++)
                {
                    tasks.Add(GetStoryByIdAsync(ids[i]));
                }
                var results = await Task.WhenAll(tasks);
                stories = results.Where(s => s != null).Cast<Story>().ToList();

                _logger.LogInformation($"Fetched {stories.Count} valid stories");
                _cache.Set(CacheKey, stories, CacheDuration);
                cachedStories = stories;
            }
            else
            {
                _logger.LogInformation("Cache hit: using cached stories");
            }

            IEnumerable<Story> filteredStories = cachedStories;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredStories = filteredStories
                    .Where(s => s.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                _logger.LogInformation($"Filtered stories with search term '{searchTerm}', count: {filteredStories.Count()}");
            }

            var totalCount = filteredStories.Count();

            filteredStories = filteredStories
                .OrderByDescending(s => s.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return new StoryResponse
            {
                Stories = filteredStories,
                TotalCount = totalCount
            };
        }

        private class HackerNewsStory
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Url { get; set; }
            public string? Type { get; set; }
        }
    }
}
