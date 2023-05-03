using LibraryAPI.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Dtos
{
    public class AddBookDto
    {
        public int PublisherId { get; set; }
        public short PageSize { get; set; }
        public string Name { get; set; }
        public ICollection<int>? Writers { get; set; }
    }
}
