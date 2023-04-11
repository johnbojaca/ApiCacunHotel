using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancunHotel.Entities.Bookings
{
    public class BookingLog
    {
        public long IdBooking { get; set; }
        public BookingStatus Status { get; set; }
        public string LogMessage { get; set; }
        public DateTime EventDate { get; set; }

        public BookingLog()
        {
            this.Status = new BookingStatus();
            this.LogMessage = string.Empty;
        }
    }
}
