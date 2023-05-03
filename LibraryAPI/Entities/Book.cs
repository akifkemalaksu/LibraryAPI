using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace LibraryAPI.Entities
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public int PublisherId { get; set; }
        public short PageSize { get; set; }
        [StringLength(100)]
        public string Name { get; set; }

        [ForeignKey("PublisherId")]
        public virtual Publisher Publisher { get; set; }
        public virtual ICollection<Writer> Writers { get; set; }
    }
}
