using Backend.Models;
using Backend.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Backend.Tests
{
    public class StoryServiceTests
    {
        private readonly StoryService _storyService;

        public StoryServiceTests()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var logger = NullLogger<StoryService>.Instance;
            _storyService = new StoryService(memoryCache, logger);
        }

        [Fact]
        public async Task GetNewestStoriesAsync_ReturnsPagedStories()
        {
            var storiesResponse = await _storyService.GetNewestStoriesAsync(null, 1, 10);
            Assert.Equal(10, storiesResponse.Stories.Count());
            Assert.True(storiesResponse.Stories.First().Id > storiesResponse.Stories.Last().Id);
        }

        [Fact]
        public async Task GetNewestStoriesAsync_FiltersBySearchTerm()
        {
            var storiesResponse = await _storyService.GetNewestStoriesAsync("Title 1", 1, 20);
            Assert.All(storiesResponse.Stories, s => Assert.Contains("Title 1", s.Title));
        }
    }
}
