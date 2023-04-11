using CancunHotel.BL.Interfaces;
using CancunHotel.DAL.Bookings;
using CancunHotel.Entities.Bookings;
using CancunHotel.Entities.Guests;

namespace CancunHotel.BL
{
    public class BookingBL : IBookingBL
    {
        public async Task<Booking> InsertBooking(Booking booking, string login)
        {
            return await BookingDAL.InsertBooking(booking, login);
        }

        public async Task<Booking> GetBookingByIdBooking(long pIdBooking)
        {
            return await BookingDAL.GetBookingByIdBooking(pIdBooking);
        }

        public async Task<List<Booking>> GetBookingByIdentification(int pIdType, string pIdentification)
        {
            return await BookingDAL.GetBookingByIdentification(pIdType, pIdentification);
        }

        public async Task<Booking> UpdateBooking(long IdBooking, Booking booking, string login)
        {
            return await BookingDAL.UpdateBooking(IdBooking, booking, login);
        }

        public async Task<Booking> UpdateGuestsBooking(long IdBooking, ListGuest Guests, string login)
        {
            return await BookingDAL.UpdateGuestsBooking(IdBooking, Guests, login);
        }

        public async Task<Booking> CancelBooking(long IdBooking, string login)
        {
            return await BookingDAL.CancelBooking(IdBooking, login);
        }

        public async Task<List<BookingLog>> GetBookingLogs(long IdBooking)
        {
            return await BookingDAL.GetBookingLogs(IdBooking);
        }
    }
}