
using System.Threading.Tasks;

namespace Fall2025_Project3_nchubbell.Services
{
    public interface IAIReviewService
    {
        Task<string> GenerateMovieReviewsAsync(string title, string? description, int reviewCount);
        Task<string> GenerateActorTweetsAsync(string actorName, int tweetCount);
    }
}
