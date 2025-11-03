using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Catering.Data
{
    public class FoodItem
    {
        [Key]
        public int FoodItemId { get; set; } // Primary key


        [MaxLength(50)]
        public required string Description { get; set; } // Nullable
        public decimal UnitPrice { get; set; }
    }
}
