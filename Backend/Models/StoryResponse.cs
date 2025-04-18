using System.Collections.Generic;

namespace Backend.Models
{
    public class StoryResponse
    {
        public IEnumerable<Story> Stories { get; set; } = new List<Story>();
        public int TotalCount { get; set; }
    }
}
