namespace CANCUN.Booking.Utility
{
    public class AppSettings
    {
        public class Settings
        {
            public string SecretLogin { get; set; }
            public int ExpireTimeSession { get; set; }
            public string Audience { get; set; }
            public string IssuerLogin { get; set; }
        }
    }
}
