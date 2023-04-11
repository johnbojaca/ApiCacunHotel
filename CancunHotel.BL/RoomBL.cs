using CancunHotel.BL.Interfaces;
using CancunHotel.DAL.Rooms;
using CancunHotel.Entities.Rooms;

namespace CancunHotel.BL
{
    public class RoomBL : IRoomBL
    {
        public async Task<List<Room>> GetRooms(int? pRoomNumber, bool? pIsActive)
        {
            return await RoomDAL.GetRooms(pRoomNumber, pIsActive);
        }

        public async Task<List<DateTime>> GetAvailableDays(int pRoomNumber)
        {
            return await RoomDAL.GetAvailableDays(pRoomNumber);
        }
        public async Task<decimal> QuoteRoom(int RoomNumber, DateTime StarteDate, DateTime FinalDate, int Adults, int Childs)
        {
            return await RoomDAL.QuoteRoom(RoomNumber, StarteDate, FinalDate, Adults, Childs);
        }
    }
}