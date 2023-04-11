using System.Data;
using System.Data.SqlClient;
using CancunHotel.Entities.Rooms;
using CancunHotel.Entities.Exceptions;
using CancunHotel.Entities;

namespace CancunHotel.DAL.Rooms
{
    public static class RoomDAL
    {
        public static async Task<List<Room>> GetRooms(int? pRooNumber, bool? pIsActive)
        {
            List<Room> rooms = new List<Room>();

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                SqlCommand cmd = new SqlCommand(Constant.StoredProcedure.SelectRooms, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (pRooNumber == null) { cmd.Parameters.AddWithValue("@ROOM_NUMBER", DBNull.Value); } else { cmd.Parameters.AddWithValue("@ROOM_NUMBER", pRooNumber); }
                if (pIsActive == null) { cmd.Parameters.AddWithValue("@IS_ACTIVE", DBNull.Value); } else { cmd.Parameters.AddWithValue("@IS_ACTIVE", pIsActive); }

                await conn.OpenAsync();
                SqlDataReader Rd = await cmd.ExecuteReaderAsync();

                while (Rd.Read())
                {
                    Room room = new Room()
                    {
                        RoomNumber = Convert.ToInt32(Rd["RoomNumber"].ToString()),
                        BaseValue = Convert.ToDecimal(Rd["BaseValue"].ToString()),
                        HasWifi = Convert.ToBoolean(Rd["HasWifi"].ToString()),
                        HasTV = Convert.ToBoolean(Rd["HasTV"].ToString()),
                        HasBalcony = Convert.ToBoolean(Rd["HasBalcony"].ToString()),
                        MinGuest = Convert.ToInt32(Rd["MinGuest"].ToString()),
                        MaxGuest = Convert.ToInt32(Rd["MaxGuest"].ToString()),
                        IsActive = Convert.ToBoolean(Rd["IsActive"].ToString()),
                    };

                    rooms.Add(room);
                }
            }

            return rooms;
        }

        public static async Task<List<DateTime>> GetAvailableDays(int pRooNumber)
        {
            List<DateTime> availableDates = new List<DateTime>();

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(Constant.StoredProcedure.SelectAvailableDays, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ROOM_NUMBER", pRooNumber);

                    await conn.OpenAsync();
                    SqlDataReader Rd = await cmd.ExecuteReaderAsync();

                    while (Rd.Read())
                    {
                        availableDates.Add(Convert.ToDateTime(Rd["AvalaibleDate"].ToString()));
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number > 50000 && ex.Number < 50050) throw new CANCUNBadRequestException(ex.Message);
                    else throw new Exception("SQL Exception", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("General Exception", ex);
                }
            }

            return availableDates;
        }

        public static async Task<decimal> QuoteRoom(int RoomNumber, DateTime StarteDate, DateTime FinalDate, int Adults, int Childs)
        {
            decimal price = 0;

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(Constant.StoredProcedure.QuoteRoom, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ROOM_NUMBER", RoomNumber);
                    cmd.Parameters.AddWithValue("@START_DATE", StarteDate);
                    cmd.Parameters.AddWithValue("@FINAL_DATE", FinalDate);
                    cmd.Parameters.AddWithValue("@ADULTS", Adults);
                    cmd.Parameters.AddWithValue("@CHILDS", Childs);

                    await conn.OpenAsync();
                    SqlDataReader Rd = await cmd.ExecuteReaderAsync();

                    while (Rd.Read())
                    {
                        price = Convert.ToDecimal(Rd["Price"].ToString());
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number > 50000 && ex.Number < 50050) throw new CANCUNBadRequestException(ex.Message);
                    else throw new Exception("SQL Exception", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("General Exception", ex);
                }
            }

            return price;
        }
    }
}
