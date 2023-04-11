using System;

namespace CancunHotel.Entities.Exceptions
{
    [Serializable]
    public class CANCUNBadRequestException : Exception
    {
        public CANCUNBadRequestException() : base() { }
        public CANCUNBadRequestException(string message) : base(message) { }
        public CANCUNBadRequestException(string message, Exception inner) : base(message, inner) { }
    }
}