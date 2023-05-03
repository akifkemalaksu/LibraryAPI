using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Entities
{
    public class Writer
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string SurName { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
