using System.Collections.Generic;

namespace Fall2025_Project3_nchubbell.Models.ViewModels
{
    public class ActorDetailsViewModel
    {
        public int Id { get; set; }

        // Basic actor info
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? ImdbUrl { get; set; }
        public byte[]? Photo { get; set; }

        // Movies this actor appears in
        public List<Movie> Movies { get; set; } = new();

        // AI-generated tweets + sentiment (reuse ReviewWithSentiment)
        public List<ReviewWithSentiment> Tweets { get; set; } = new();

        // Overall sentiment heading
        public double OverallCompoundSentiment { get; set; }
        public string OverallSentimentLabel { get; set; } = string.Empty;
    }
}
