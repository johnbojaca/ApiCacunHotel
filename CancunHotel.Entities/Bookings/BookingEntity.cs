using CancunHotel.Entities.Guests;
using CancunHotel.Entities.Rooms;

namespace CancunHotel.Entities.Bookings
{
    public class Booking
    {
        public long IdBooking { get; set; }
        public BookingStatus IdStatus { get; set; }
        public Room room { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinalDate { get; set; }
        public decimal Price { get; set; }
        public List<Guest> guests { get; set; }
    }
}
