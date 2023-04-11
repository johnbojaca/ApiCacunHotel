
using System.Collections.Generic;

namespace CancunHotel.Entities
{
    public static class Constant
    {
        public static class StoredProcedure
        {
            public const string SelectUserByUsername = "SelectUserByUsername";
            public const string SelectRooms = "SelectRooms";
            public const string InsertBooking = "InsertBooking";
            public const string InsertGuest = "InsertGuest";
            public const string UpdatePrice = "UpdatePrice";
            public const string SelectBookingByIdBooking = "SelectBookingByIdBooking";
            public const string SelectGuests = "SelectGuests";
            public const string SelectGenders = "SelectGenders";
            public const string SelectIdTypes = "SelectIdTypes";
            public const string SelectBookingStates = "SelectBookingStates";
            public const string UpdateBooking = "UpdateBooking";
            public const string DeleteGuestByBooking = "DeleteGuestByBooking";
            public const string UpdateState = "UpdateState";
            public const string SelectAvailableDays = "SelectAvailableDays";
            public const string QuoteRoom = "QuoteRoom";
            public const string SelectBookingLogs = "SelectBookingLogs";
        }
    }
}
