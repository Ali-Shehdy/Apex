using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Apex.Catering.Data
{
    public class Menu
    {
        [Key]
        // Since the key name (MenuCode) does not include
        // "Id", we have to use an annotation (could also
        // specify this using FluitAPI)
        [MaxLength(15)]
        public required string MenuId { get; set; }

        
        [MaxLength(30)]
        public string? MenuName { get; set; }
    }
}
