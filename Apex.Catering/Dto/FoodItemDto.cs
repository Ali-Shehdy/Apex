namespace Apex.Catering.Dto
{
    public class FoodItemDto
    { 
        public int FoodItemId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }
}
