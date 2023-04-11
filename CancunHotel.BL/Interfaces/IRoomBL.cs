using CancunHotel.Entities.Rooms;

namespace CancunHotel.BL.Interfaces
{
    public interface IRoomBL
    {
        Task<List<Room>> GetRooms(int? pRoomNumber, bool? pIsActive);
        Task<List<DateTime>> GetAvailableDays(int pRoomNumber);
        Task<decimal> QuoteRoom(int RoomNumber, DateTime StarteDate, DateTime FinalDate, int Adults, int Childs);
    }
}
