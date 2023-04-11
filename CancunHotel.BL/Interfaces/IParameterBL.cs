using CancunHotel.Entities.Bookings;
using CancunHotel.Entities.Guests;

namespace CancunHotel.BL.Interfaces
{
    public interface IParameterBL
    {
        Task<List<Gender>> GetGenders();
        Task<List<IdType>> GetIdTypes();
        Task<List<BookingStatus>> GetBookingStates();
    }
}
