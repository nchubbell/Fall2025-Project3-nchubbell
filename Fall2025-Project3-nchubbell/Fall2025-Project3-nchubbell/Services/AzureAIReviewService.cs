using System;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel;

namespace Fall2025_Project3_nchubbell.Services
{
    public class AzureAIReviewService : IAIReviewService
    {
        private readonly ChatClient _chatClient;

        public AzureAIReviewService(IConfiguration configuration)
        {
            string endpoint = configuration["AzureOpenAI:Endpoint"]
                ?? throw new InvalidOperationException("AzureOpenAI:Endpoint not configured.");
            string apiKey = configuration["AzureOpenAI:ApiKey"]
                ?? throw new InvalidOperationException("AzureOpenAI:ApiKey not configured.");
            string deploymentName = configuration["AzureOpenAI:DeploymentName"]
                ?? throw new InvalidOperationException("AzureOpenAI:DeploymentName not configured.");

            // Azure.AI.OpenAI 2.x style
            var azureClient = new AzureOpenAIClient(
                new Uri(endpoint),
                new ApiKeyCredential(apiKey));

            _chatClient = azureClient.GetChatClient(deploymentName);
        }

        // Generate N reviews for a movie (we'll call it with N = 3)
        public async Task<string> GenerateMovieReviewsAsync(string title, string? description, int reviewCount)
        {
            var systemMessage = new SystemChatMessage(
                "You are a helpful movie critic who writes short, distinct reviews.");

            var userPrompt = new StringBuilder();
            userPrompt.AppendLine($"Write {reviewCount} short, distinct reviews for the movie \"{title}\".");
            if (!string.IsNullOrWhiteSpace(description))
            {
                userPrompt.AppendLine($"Here is the plot/description: {description}");
            }
            userPrompt.AppendLine("Return ONLY the reviews as plain text, with a blank line between each review.");

            var userMessage = new UserChatMessage(userPrompt.ToString());

            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                new ChatMessage[] { systemMessage, userMessage });

            // All reviews in one big string; controllers will split on blank lines
            return completion.Content[0].Text;
        }

        // Generate N tweets/comments for an actor (we'll call it with N = 5)
        public async Task<string> GenerateActorTweetsAsync(string actorName, int tweetCount)
        {
            var systemMessage = new SystemChatMessage(
                "You write short, positive social-media style comments about actors.");

            var userMessage = new UserChatMessage(
                $"Write {tweetCount} short, enthusiastic social-media style comments about the actor \"{actorName}\". " +
                "Each comment should be its own paragraph. Return ONLY the comments separated by blank lines.");

            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                new ChatMessage[] { systemMessage, userMessage });

            return completion.Content[0].Text;
        }
    }
}
