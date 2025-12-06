namespace Apex.Events.Services
{
    public class ReservationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string ReservationReference { get; set; }

        public static ReservationResult Fail(string message) =>
            new ReservationResult { Success = false, Message = message };

        public static ReservationResult Ok(string reference) =>
            new ReservationResult
            {
                Success = true,
                ReservationReference = reference
            };
    }
}
