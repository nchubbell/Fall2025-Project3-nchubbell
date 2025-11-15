using System.Text;
using System.Threading.Tasks;

namespace Fall2025_Project3_nchubbell.Services
{
    // NOTE: This implementation does NOT call Azure OpenAI.
    // It just returns placeholder text that looks like AI output,
    // while still matching the IAIReviewService interface.
    public class AzureAIReviewService : IAIReviewService
    {
        public AzureAIReviewService()
        {
        }

        public Task<string> GenerateMovieReviewsAsync(string title, string? description, int reviewCount)
        {
            var sb = new StringBuilder();

            for (int i = 1; i <= reviewCount; i++)
            {
                sb.AppendLine($"Review {i} for \"{title}\":");
                sb.AppendLine("This is a sample review generated locally instead of using Azure OpenAI.");
                if (!string.IsNullOrWhiteSpace(description))
                {
                    sb.AppendLine($"Plot hint: {description}");
                }
                sb.AppendLine("Overall, this movie is entertaining and worth watching.");
                sb.AppendLine(); // blank line between reviews
            }

            return Task.FromResult(sb.ToString());
        }

        public Task<string> GenerateActorTweetsAsync(string actorName, int tweetCount)
        {
            var sb = new StringBuilder();

            for (int i = 1; i <= tweetCount; i++)
            {
                sb.AppendLine($"Comment {i} about {actorName}:");
                sb.AppendLine($"{actorName} absolutely stole the show in their latest role!");
                sb.AppendLine("Such a fun performance – can’t wait to see what they do next.");
                sb.AppendLine(); // blank line between comments
            }

            return Task.FromResult(sb.ToString());
        }
    }
}
