using System.ComponentModel.DataAnnotations;

namespace MoviesCRUD.Models
{
    public class Movie // int double float required by default
    {
        public int Id { get; set; }
        [Required, MaxLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }

        public double Rate { get; set; }
        [Required, MaxLength(2500)]
        public string Storyline { get; set; }
        [Required]
        public byte[] Poster { get; set; }

        public byte GenreId { get; set; } // by default fk bcs it's name is GenreId

        public Genre Genre { get; set; }

    }
}
