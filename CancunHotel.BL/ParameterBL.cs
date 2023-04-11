using CancunHotel.BL.Interfaces;
using CancunHotel.DAL.Parameters;
using CancunHotel.Entities.Bookings;
using CancunHotel.Entities.Guests;

namespace CancunHotel.BL
{
    public class ParameterBL : IParameterBL
    {
        public async Task<List<Gender>> GetGenders()
        {
            return await ParameterDAL.GetGenders();
        }

        public async Task<List<IdType>> GetIdTypes()
        {
            return await ParameterDAL.GetIdTypes();
        }

        public async Task<List<BookingStatus>> GetBookingStates()
        {
            return await ParameterDAL.GetBookingStates();
        }
    }
}