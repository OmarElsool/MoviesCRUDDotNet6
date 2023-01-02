using MoviesCRUD.Models;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MoviesCRUD.ViewModels
{
    public class MovieFormViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(250)] // better to use stringlength in viewModel and maxLength in domainModel
        public string Title { get; set; }

        public int Year { get; set; }

        [Range(1, 10)]
        public double Rate { get; set; }

        [Required, StringLength(2500)]
        public string Storyline { get; set; }

        public byte[]? Poster { get; set; }

        [Display(Name = "Genre")] //Change Name That will display to user
        public byte GenreId { get; set; }

        public IEnumerable<Genre>? Genres { get; set; }
    }
}
