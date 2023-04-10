using System;

namespace CancunHotel.Entities.Security
{
    public class UserInfo
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
