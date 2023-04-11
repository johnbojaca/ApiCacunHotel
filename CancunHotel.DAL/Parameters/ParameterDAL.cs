using System.Data;
using System.Data.SqlClient;
using CancunHotel.Entities.Guests;
using CancunHotel.Entities;
using CancunHotel.Entities.Bookings;

namespace CancunHotel.DAL.Parameters
{
    public static class ParameterDAL
    {
        public static async Task<List<Gender>> GetGenders()
        {
            List<Gender> genders = new List<Gender>();

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                SqlCommand cmd = new SqlCommand(Constant.StoredProcedure.SelectGenders, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                SqlDataReader Rd = await cmd.ExecuteReaderAsync();

                while (Rd.Read())
                {
                    Gender gender = new Gender()
                    {
                        IdGender = Convert.ToInt32(Rd["IdGender"].ToString()),
                        GenderName = Rd["GenderName"].ToString(),
                    };

                    genders.Add(gender);
                }
            }

            return genders;
        }

        public static async Task<List<IdType>> GetIdTypes()
        {
            List<IdType> idTypes = new List<IdType>();

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                SqlCommand cmd = new SqlCommand(Constant.StoredProcedure.SelectIdTypes, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                SqlDataReader Rd = await cmd.ExecuteReaderAsync();

                while (Rd.Read())
                {
                    IdType idType = new IdType()
                    {
                        Id = Convert.ToInt32(Rd["Id"].ToString()),
                        IdTypeName = Rd["IdTypeName"].ToString(),
                        Acronym = Rd["Acronym"].ToString(),
                    };

                    idTypes.Add(idType);
                }
            }

            return idTypes;
        }

        public static async Task<List<BookingStatus>> GetBookingStates()
        {
            List<BookingStatus> bookingStatus = new List<BookingStatus>();

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                SqlCommand cmd = new SqlCommand(Constant.StoredProcedure.SelectBookingStates, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                SqlDataReader Rd = await cmd.ExecuteReaderAsync();

                while (Rd.Read())
                {
                    BookingStatus state = new BookingStatus()
                    {
                        IdStatus = Convert.ToInt32(Rd["IdStatus"].ToString()),
                        StatusName = Rd["StatusName"].ToString(),
                        Description = Rd["Description"].ToString(),
                    };

                    bookingStatus.Add(state);
                }
            }

            return bookingStatus;
        }
    }
}
