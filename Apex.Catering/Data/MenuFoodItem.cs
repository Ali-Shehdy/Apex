using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Catering.Data
{
    public class MenuFoodItem
    {
        public int MenuId { get; set; } // Forigen key

        public int FoodItemId { get; set; } // Forigen key

        //Navigation properties
        public virtual Menu? Menu { get; set; }
        public virtual FoodItem FoodItem { get; set; }
    }
}
