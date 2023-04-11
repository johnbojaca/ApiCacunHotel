using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancunHotel.Entities.Guests
{
    public class ListGuest
    {
        public List<Guest> Guests { get; set; }

        public ListGuest()
        {
            this.Guests = new List<Guest>();
        }
    }
}
