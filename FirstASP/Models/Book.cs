using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstASP.Models
{
    public class Book : BookDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
    public class BookDto
    {
        [StringLength(100, ErrorMessage = "Максимальная длина = 100 символов")]
        [Required(ErrorMessage = "Название обязательно")]
        public required string Title { get; set; }
        

        [StringLength(50)]
        [Required(ErrorMessage = "ФИО автора обязательно")]
        public required string Author { get; set; }
        
        
        [Range(1, int.MaxValue, ErrorMessage = "Год должен быть положительным")]
        public int YearPublished { get; set; }
    }
}
