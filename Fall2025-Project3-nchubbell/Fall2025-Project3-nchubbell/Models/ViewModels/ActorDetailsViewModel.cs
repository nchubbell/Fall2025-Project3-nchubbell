using System.Collections.Generic;

namespace Fall2025_Project3_nchubbell.Models.ViewModels
{
    public class ActorDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? ImdbUrl { get; set; }
        public byte[]? Photo { get; set; }

        public List<Movie> Movies { get; set; } = new();

        public List<ReviewWithSentiment> Tweets { get; set; } = new();
        public double OverallCompoundSentiment { get; set; }
        public string OverallSentimentLabel { get; set; } = string.Empty;
    }
}
