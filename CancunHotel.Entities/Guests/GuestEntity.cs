﻿namespace CancunHotel.Entities.Guests
{
    public class Guest
    {
        public long IdBooking { get; set; }
        public string Names { get; set; }
        public string Surnames { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public IdType IdType { get; set; }
        public string Identification { get; set; }
        public int Age { get; set; }
        public Gender gender { get; set; }
    }
}
