using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancunHotel.Entities.Bookings
{
    public class BookingStatus
    {
        public int IdStatus { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }

        public BookingStatus()
        {
            this.StatusName = string.Empty;
            this.Description = string.Empty;
        }
    }
}
