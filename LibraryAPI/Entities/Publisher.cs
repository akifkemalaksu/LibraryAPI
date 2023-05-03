using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Entities
{
    public class Publisher
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
    }
}
