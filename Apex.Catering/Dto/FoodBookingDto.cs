namespace Apex.Catering.Dto
{
    public class FoodBookingDto
    {

        public int FoodBookingId { get; set; }
        public int? ClintReferenceId { get; set; }
        public int NumberOfGuests { get; set; }
        public int MenuId { get; set; }
    }
}
