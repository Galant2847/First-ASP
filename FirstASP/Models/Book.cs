using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstASP.Models
{
    public class Book
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; } = null!;

        [StringLength(50)]
        public string Author { get; set; } = null!;

        [Range(0, 2100)]
        public int YearPublished { get; set; }
    }
    public class CreateBookDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string Author { get; set; }

        [Range(0, 2100)]
        public int YearPublished { get; set; }
    }
}
