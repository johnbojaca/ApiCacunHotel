using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancunHotel.Entities.Rooms
{
    public class Room
    {
        public int RoomNumber { get; set; }
        public decimal BaseValue { get; set; }
        public bool HasWifi { get; set; }
        public bool HasTV { get; set; }
        public bool HasBalcony { get; set; }
        public int MinGuest { get; set; }
        public int MaxGuest { get; set; }
        public bool IsActive { get; set; }
    }
}
