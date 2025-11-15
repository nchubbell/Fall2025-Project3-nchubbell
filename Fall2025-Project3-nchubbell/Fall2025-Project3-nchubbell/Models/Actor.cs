using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_nchubbell.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Gender { get; set; }

        public int Age { get; set; }

        [Display(Name = "IMDB Link")]
        [Url]
        public string ImdbUrl { get; set; }

        public byte[] Photo { get; set; }

        public ICollection<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();
    }
}
