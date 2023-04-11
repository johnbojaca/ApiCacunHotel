using CancunHotel.Utility;

namespace CancunHotel.DAL
{
    public static class Connections
    {
        public enum TYPE
        {
            CANCUN
        }

        public static string GetConnectionString(TYPE type)
        {
            string connectionString = string.Empty;

            switch (type)
            {
                case TYPE.CANCUN:
                    connectionString = ApiConnectionStrings.CancunDB;
                    break;
            }

            return connectionString;
        }
    }
}
