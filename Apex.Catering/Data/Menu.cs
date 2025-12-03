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
        
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/navigations
        //Collection types: The underlying collection instance must implement ICollection<T>,
        //and must have a working Add method.
        //It is common to use List<T> or HashSet<T>. List<T> is efficient for small numbers 
        //of related entities and maintains a stable ordering.
        //HashSet<T> has more efficient lookups for large numbers of entities, 
        //but does not have stable ordering.
        
        // Relation property to MenuFoodItem
        public virtual ICollection<MenuFoodItem> MenuFoodItems { get; set; } = new List<MenuFoodItem>();

        // Relation property to FoodBooking
        public virtual ICollection<FoodBooking> FoodBookings { get; set; } = new List<FoodBooking>();
    }
}
