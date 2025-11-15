using System.Collections.Generic;
using Fall2025_Project3_nchubbell.Models;

namespace Fall2025_Project3_nchubbell.Models.ViewModels
{
    public class ReviewWithSentiment
    {
        public string Text { get; set; } = string.Empty;

        public double Compound { get; set; }
        public double Positive { get; set; }
        public double Negative { get; set; }
        public double Neutral { get; set; }

        // "Positive", "Neutral", or "Negative"
        public string Label { get; set; } = string.Empty;
    }

    public class MovieDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int Year { get; set; }
        public string? ImdbUrl { get; set; }
        public byte[]? Poster { get; set; }

        public List<Actor> Actors { get; set; } = new();
        public List<ReviewWithSentiment> Reviews { get; set; } = new();

        // NEW: overall sentiment heading fields
        public double OverallCompoundSentiment { get; set; }
        public string OverallSentimentLabel { get; set; } = string.Empty;
    }
}
