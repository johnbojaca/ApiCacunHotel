using CancunHotel.Entities.Bookings;
using CancunHotel.Entities.Guests;

namespace CancunHotel.BL.Interfaces
{
    public interface IBookingBL
    {
        Task<Booking> InsertBooking(Entities.Bookings.Booking booking, string login);
        Task<Booking> GetBookingByIdBooking(long pIdBooking);
        Task<List<Booking>> GetBookingByIdentification(int pIdType, string pIdentification);
        Task<Booking> UpdateBooking(long IdBooking, Entities.Bookings.Booking booking, string login);
        Task<Booking> UpdateGuestsBooking(long IdBooking, ListGuest Guests, string login);
        Task<Booking> CancelBooking(long IdBooking, string login);
        Task<List<BookingLog>> GetBookingLogs(long IdBooking);
    }
}
