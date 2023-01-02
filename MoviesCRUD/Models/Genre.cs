using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesCRUD.Models
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // auto increament the id bcs it's byte will not set by default
        public byte Id { get; set; } // byte = tinyint
        [Required, MaxLength(100)]
        public string Name { get; set; }

    }
}
