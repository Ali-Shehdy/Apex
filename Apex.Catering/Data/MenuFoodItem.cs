using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Catering.Data
{
    public class MenuFoodItem
    {
        public int MenuId { get; set; } // Primary  key

        public int FoodItemId { get; set; } // Primary  key

        //Navigation properties
        public virtual Menu? Menu { get; set; } // Relation property to Menu
        public virtual  FoodItem? FoodItem { get; set; } // Relation property to FoodItem
    }
}
