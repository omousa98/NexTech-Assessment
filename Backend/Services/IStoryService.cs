using Backend.Models;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IStoryService
    {
        Task<StoryResponse> GetNewestStoriesAsync(string? searchTerm, int pageNumber, int pageSize);
    }
}
