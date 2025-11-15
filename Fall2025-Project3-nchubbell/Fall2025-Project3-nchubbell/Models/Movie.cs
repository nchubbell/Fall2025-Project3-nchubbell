using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_nchubbell.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Display(Name = "IMDB Link")]
        [Url]
        public string ImdbUrl { get; set; }

        [Required]
        [StringLength(100)]
        public string Genre { get; set; }

        [Display(Name = "Year of Release")]
        public int Year { get; set; }

        public byte[] Poster { get; set; }


        public ICollection<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();
    }
}
