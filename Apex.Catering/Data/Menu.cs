using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Apex.Catering.Data
{
    public class Menu
    {
        [Key]
        // Since the key name (MenuCode) does not include
        // "Id", we have to use an annotation (could also
        // specify this using FluitAPI)
        public required int MenuId { get; set; }

        
        [MaxLength(30)]
        public string? MenuName { get; set; }

        // Relation property to MenuFoodItem
        public virtual ICollection<MenuFoodItem> MenuFoodItems { get; set; } = new List<MenuFoodItem>();

        // Relation property to FoodBooking
        public virtual ICollection<FoodBooking> FoodBookings { get; set; } = new List<FoodBooking>();
    }
}
