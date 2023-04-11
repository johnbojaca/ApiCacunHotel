using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using CancunHotel.Entities.Exceptions;
using CancunHotel.Entities;
using CancunHotel.Entities.Bookings;
using CancunHotel.Entities.Rooms;
using CancunHotel.Entities.Guests;

namespace CancunHotel.DAL.Bookings
{
    public static class BookingDAL
    {
        public static async Task<Entities.Bookings.Booking> InsertBooking(Entities.Bookings.Booking booking, string login)
        {
            Entities.Bookings.Booking resp = new Entities.Bookings.Booking();

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                SqlTransaction transaction = null;
                try
                {
                    await conn.OpenAsync();
                    transaction = conn.BeginTransaction();

                    #region Insert Booking

                    SqlCommand cmdBooking = new SqlCommand(Constant.StoredProcedure.InsertBooking, conn);
                    cmdBooking.CommandType = CommandType.StoredProcedure;
                    cmdBooking.Transaction = transaction;

                    cmdBooking.Parameters.AddWithValue("@START_DATE", booking.StartDate);
                    cmdBooking.Parameters.AddWithValue("@FINAL_DATE", booking.FinalDate);
                    cmdBooking.Parameters.AddWithValue("@ROOM_NUMBER", booking.room.RoomNumber);

                    SqlDataReader rdBooking = await cmdBooking.ExecuteReaderAsync();

                    while (rdBooking.Read())
                    {
                        resp.IdStatus = new BookingStatus();
                        resp.room = new Room();
                        resp.guests = new List<Guest>();

                        resp.IdBooking = Convert.ToInt64(rdBooking["IdBooking"].ToString());
                        resp.IdStatus.IdStatus = Convert.ToInt32(rdBooking["IdStatus"].ToString());
                        resp.IdStatus.StatusName = rdBooking["StatusName"].ToString();
                        resp.IdStatus.Description = rdBooking["Description"].ToString();
                        resp.room.RoomNumber = Convert.ToInt32(rdBooking["RoomNumber"].ToString());
                        resp.room.BaseValue = Convert.ToDecimal(rdBooking["BaseValue"].ToString());
                        resp.room.HasWifi = Convert.ToBoolean(rdBooking["HasWifi"].ToString());
                        resp.room.HasTV = Convert.ToBoolean(rdBooking["HasTV"].ToString());
                        resp.room.HasBalcony = Convert.ToBoolean(rdBooking["HasBalcony"].ToString());
                        resp.room.MinGuest = Convert.ToInt32(rdBooking["MinGuest"].ToString());
                        resp.room.MaxGuest = Convert.ToInt32(rdBooking["MaxGuest"].ToString());
                        resp.room.IsActive = Convert.ToBoolean(rdBooking["IsActive"].ToString());
                        resp.StartDate = Convert.ToDateTime(rdBooking["StartDate"].ToString());
                        resp.FinalDate = Convert.ToDateTime(rdBooking["FinalDate"].ToString());
                        resp.Price = Convert.ToDecimal(rdBooking["Price"].ToString());
                    }

                    rdBooking.Close();

                    #endregion

                    #region Insert Guests

                    var countPayingGuest = booking.guests.Where(x => x.Age >= 0 && x.Age < 5).Count();
                    if (countPayingGuest >= resp.room.MinGuest && countPayingGuest <= resp.room.MaxGuest)
                    {
                        foreach (Guest guest in booking.guests)
                        {
                            SqlCommand cmdGuest = new SqlCommand(Constant.StoredProcedure.InsertGuest, conn);
                            cmdGuest.CommandType = CommandType.StoredProcedure;
                            cmdGuest.Transaction = transaction;

                            cmdGuest.Parameters.AddWithValue("@ID_BOOKING", resp.IdBooking);
                            cmdGuest.Parameters.AddWithValue("@NAMES", guest.Names);
                            cmdGuest.Parameters.AddWithValue("@SURNAMES", guest.Surnames);
                            cmdGuest.Parameters.AddWithValue("@ID_TYPE", guest.IdType.Id);
                            cmdGuest.Parameters.AddWithValue("@IDENTIFICATION", guest.Identification);
                            cmdGuest.Parameters.AddWithValue("@AGE", guest.Age);
                            cmdGuest.Parameters.AddWithValue("@ID_GENDER", guest.gender.IdGender);

                            if (guest.Email == null) { cmdGuest.Parameters.AddWithValue("@EMAIL", DBNull.Value); } else { cmdGuest.Parameters.AddWithValue("@EMAIL", guest.Email); }
                            if (guest.Phone == null) { cmdGuest.Parameters.AddWithValue("@PHONE", DBNull.Value); } else { cmdGuest.Parameters.AddWithValue("@PHONE", guest.Phone); }

                            SqlDataReader rdGuest = await cmdGuest.ExecuteReaderAsync();

                            Guest guestResp = new Guest();
                            guestResp.IdType = new IdType();
                            guestResp.gender = new Gender();

                            while (rdGuest.Read())
                            {
                                guestResp.IdBooking = Convert.ToInt64(rdGuest["IdBooking"].ToString());
                                guestResp.Names = rdGuest["Names"].ToString();
                                guestResp.Surnames = rdGuest["Surnames"].ToString();
                                guestResp.IdType.Id = Convert.ToInt32(rdGuest["Id"].ToString());
                                guestResp.IdType.IdTypeName = rdGuest["IdTypeName"].ToString();
                                guestResp.IdType.Acronym = rdGuest["Acronym"].ToString();
                                guestResp.Identification = rdGuest["Identification"].ToString();
                                guestResp.Age = Convert.ToInt32(rdGuest["Age"].ToString());
                                guestResp.gender.IdGender = Convert.ToInt32(rdGuest["IdGender"].ToString());
                                guestResp.gender.GenderName = rdGuest["GenderName"].ToString();

                                guestResp.Email = rdGuest["Email"] == DBNull.Value ? null : rdGuest["Email"].ToString();
                                guestResp.Phone = rdGuest["Phone"] == DBNull.Value ? null : rdGuest["Phone"].ToString();
                            }

                            resp.guests.Add(guestResp);

                            rdGuest.Close();
                        }
                    }
                    else
                    {
                        throw new CANCUNBadRequestException($"Guest count greater or equal than {resp.room.MinGuest} and less or equal than {resp.room.MaxGuest}");
                    }

                    #endregion

                    #region Update booking price

                    var adultsNum = resp.guests.Where(guests => guests.Age > 12).Count();
                    var childsNum = resp.guests.Where(guest => guest.Age >= 5 && guest.Age <= 12).Count();

                    SqlCommand cmdPrice = new SqlCommand(Constant.StoredProcedure.UpdatePrice, conn);
                    cmdPrice.CommandType = CommandType.StoredProcedure;
                    cmdPrice.Transaction = transaction;

                    cmdPrice.Parameters.AddWithValue("@ID_BOOKING", resp.IdBooking);
                    cmdPrice.Parameters.AddWithValue("@START_DATE", resp.StartDate);
                    cmdPrice.Parameters.AddWithValue("@FINAL_DATE", resp.FinalDate);
                    cmdPrice.Parameters.AddWithValue("@ROOM_NUMBER", resp.room.RoomNumber);
                    cmdPrice.Parameters.AddWithValue("@ADULTS", adultsNum);
                    cmdPrice.Parameters.AddWithValue("@CHILDS", childsNum);

                    SqlDataReader rdPrice = await cmdPrice.ExecuteReaderAsync();

                    while (rdPrice.Read())
                    {
                        resp.Price = Convert.ToDecimal(rdPrice["Price"].ToString());
                    }

                    rdPrice.Close();

                    #endregion

                    #region Update booking Status

                    SqlCommand cmdStatus = new SqlCommand(Constant.StoredProcedure.UpdateState, conn);
                    cmdStatus.CommandType = CommandType.StoredProcedure;
                    cmdStatus.Transaction = transaction;

                    cmdStatus.Parameters.AddWithValue("@ID_BOOKING", resp.IdBooking);
                    cmdStatus.Parameters.AddWithValue("@NEW_STATE", 1);
                    cmdStatus.Parameters.AddWithValue("@MESSAGE", JsonSerializer.Serialize(resp));
                    cmdStatus.Parameters.AddWithValue("@LOGIN", login);

                    SqlDataReader rdStatus = await cmdStatus.ExecuteReaderAsync();

                    while (rdStatus.Read())
                    {
                        resp.IdStatus.IdStatus = Convert.ToInt32(rdStatus["IdStatus"].ToString());
                        resp.IdStatus.StatusName = rdStatus["StatusName"].ToString();
                        resp.IdStatus.Description = rdStatus["Description"].ToString();
                    }

                    rdStatus.Close();

                    #endregion

                    transaction.Commit();
                }
                catch (CANCUNBadRequestException ex)
                {
                    if (transaction != null) transaction.Rollback();
                    throw new CANCUNBadRequestException(ex.Message, ex);
                }
                catch (SqlException ex)
                {
                    if (transaction != null) transaction.Rollback();
                    if (ex.Number > 50000 && ex.Number < 50050) throw new CANCUNBadRequestException(ex.Message);
                    else throw new Exception("SQL Exception", ex);
                }
                catch (Exception ex)
                {
                    if (transaction != null) transaction.Rollback();
                    throw new Exception("General Exception", ex);
                }

            }

            return resp;
        }

        public static async Task<Entities.Bookings.Booking> GetBookingByIdBooking(long pIdBooking)
        {
            Entities.Bookings.Booking resp = new Entities.Bookings.Booking();

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                await conn.OpenAsync();

                #region Get Booking

                SqlCommand cmdBooking = new SqlCommand(Constant.StoredProcedure.SelectBookingByIdBooking, conn);
                cmdBooking.CommandType = CommandType.StoredProcedure;
                cmdBooking.Parameters.AddWithValue("@ID_BOOKING", pIdBooking);

                SqlDataReader rdBooking = await cmdBooking.ExecuteReaderAsync();

                while (rdBooking.Read())
                {
                    resp.IdStatus = new BookingStatus();
                    resp.room = new Room();

                    resp.IdBooking = Convert.ToInt64(rdBooking["IdBooking"].ToString());
                    resp.IdStatus.IdStatus = Convert.ToInt32(rdBooking["IdStatus"].ToString());
                    resp.IdStatus.StatusName = rdBooking["StatusName"].ToString();
                    resp.IdStatus.Description = rdBooking["Description"].ToString();
                    resp.room.RoomNumber = Convert.ToInt32(rdBooking["RoomNumber"].ToString());
                    resp.room.BaseValue = Convert.ToDecimal(rdBooking["BaseValue"].ToString());
                    resp.room.HasWifi = Convert.ToBoolean(rdBooking["HasWifi"].ToString());
                    resp.room.HasTV = Convert.ToBoolean(rdBooking["HasTV"].ToString());
                    resp.room.HasBalcony = Convert.ToBoolean(rdBooking["HasBalcony"].ToString());
                    resp.room.MinGuest = Convert.ToInt32(rdBooking["MinGuest"].ToString());
                    resp.room.MaxGuest = Convert.ToInt32(rdBooking["MaxGuest"].ToString());
                    resp.room.IsActive = Convert.ToBoolean(rdBooking["IsActive"].ToString());
                    resp.StartDate = Convert.ToDateTime(rdBooking["StartDate"].ToString());
                    resp.FinalDate = Convert.ToDateTime(rdBooking["FinalDate"].ToString());
                    resp.Price = Convert.ToDecimal(rdBooking["Price"].ToString());
                }

                rdBooking.Close();

                #endregion

                #region Get guests

                SqlCommand cmdGuest = new SqlCommand(Constant.StoredProcedure.SelectGuests, conn);
                cmdGuest.CommandType = CommandType.StoredProcedure;
                cmdGuest.Parameters.AddWithValue("@ID_BOOKING", pIdBooking);
                cmdGuest.Parameters.AddWithValue("@ID_TYPE", DBNull.Value);
                cmdGuest.Parameters.AddWithValue("@IDENTIFICATION", DBNull.Value);

                SqlDataReader rdGuest = await cmdGuest.ExecuteReaderAsync();

                resp.guests = new System.Collections.Generic.List<Guest>();

                while (rdGuest.Read())
                {
                    Guest guestResp = new Guest();
                    guestResp.IdType = new IdType();
                    guestResp.gender = new Gender();

                    guestResp.IdBooking = Convert.ToInt64(rdGuest["IdBooking"].ToString());
                    guestResp.Names = rdGuest["Names"].ToString();
                    guestResp.Surnames = rdGuest["Surnames"].ToString();
                    guestResp.IdType.Id = Convert.ToInt32(rdGuest["Id"].ToString());
                    guestResp.IdType.IdTypeName = rdGuest["IdTypeName"].ToString();
                    guestResp.IdType.Acronym = rdGuest["Acronym"].ToString();
                    guestResp.Identification = rdGuest["Identification"].ToString();
                    guestResp.Age = Convert.ToInt32(rdGuest["Age"].ToString());
                    guestResp.gender.IdGender = Convert.ToInt32(rdGuest["IdGender"].ToString());
                    guestResp.gender.GenderName = rdGuest["GenderName"].ToString();

                    guestResp.Email = rdGuest["Email"] == DBNull.Value ? null : rdGuest["Email"].ToString();
                    guestResp.Phone = rdGuest["Phone"] == DBNull.Value ? null : rdGuest["Phone"].ToString();

                    resp.guests.Add(guestResp);
                }

                rdGuest.Close();

                #endregion
            }

            return resp;
        }

        public static async Task<List<Entities.Bookings.Booking>> GetBookingByIdentification(int pIdType, string pIdentification)
        {
            List<long> idBookingsGuest = new List<long>();

            #region Get IdBookings Guests

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                await conn.OpenAsync();

                SqlCommand cmdGuest = new SqlCommand(Constant.StoredProcedure.SelectGuests, conn);
                cmdGuest.CommandType = CommandType.StoredProcedure;
                cmdGuest.Parameters.AddWithValue("@ID_BOOKING", DBNull.Value);
                cmdGuest.Parameters.AddWithValue("@ID_TYPE", pIdType);
                cmdGuest.Parameters.AddWithValue("@IDENTIFICATION", pIdentification);

                SqlDataReader rdGuest = await cmdGuest.ExecuteReaderAsync();

                while (rdGuest.Read())
                {
                    idBookingsGuest.Add(Convert.ToInt64(rdGuest["IdBooking"].ToString()));
                }

                rdGuest.Close();
            }

            #endregion

            #region Get Bookings from IdBookings

            List<Entities.Bookings.Booking> resp = new List<Entities.Bookings.Booking>();
            var idsBooking = idBookingsGuest.Distinct().ToList();

            foreach (long idBooking in idsBooking)
            {
                Entities.Bookings.Booking booking = await GetBookingByIdBooking(idBooking);

                if (booking != null && booking.IdBooking != 0)
                {
                    resp.Add(booking);
                }
            }

            #endregion

            return resp;
        }

        public static async Task<Entities.Bookings.Booking> UpdateBooking(long IdBooking, Entities.Bookings.Booking booking, string login)
        {
            Entities.Bookings.Booking resp = new Entities.Bookings.Booking();

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                SqlTransaction transaction = null;
                try
                {
                    await conn.OpenAsync();
                    transaction = conn.BeginTransaction();

                    #region Update booking Status

                    SqlCommand cmdStatus = new SqlCommand(Constant.StoredProcedure.UpdateState, conn);
                    cmdStatus.CommandType = CommandType.StoredProcedure;
                    cmdStatus.Transaction = transaction;

                    cmdStatus.Parameters.AddWithValue("@ID_BOOKING", IdBooking);
                    cmdStatus.Parameters.AddWithValue("@NEW_STATE", 3);
                    cmdStatus.Parameters.AddWithValue("@MESSAGE", JsonSerializer.Serialize(booking));
                    cmdStatus.Parameters.AddWithValue("@LOGIN", login);

                    SqlDataReader rdStatus = await cmdStatus.ExecuteReaderAsync();

                    while (rdStatus.Read())
                    {
                        resp.IdStatus.IdStatus = Convert.ToInt32(rdStatus["IdStatus"].ToString());
                        resp.IdStatus.StatusName = rdStatus["StatusName"].ToString();
                        resp.IdStatus.Description = rdStatus["Description"].ToString();
                    }

                    rdStatus.Close();

                    #endregion

                    #region Update Booking

                    SqlCommand cmdBooking = new SqlCommand(Constant.StoredProcedure.UpdateBooking, conn);
                    cmdBooking.CommandType = CommandType.StoredProcedure;
                    cmdBooking.Transaction = transaction;

                    cmdBooking.Parameters.AddWithValue("@ID_BOOKING", IdBooking);
                    cmdBooking.Parameters.AddWithValue("@START_DATE", booking.StartDate);
                    cmdBooking.Parameters.AddWithValue("@FINAL_DATE", booking.FinalDate);
                    cmdBooking.Parameters.AddWithValue("@ROOM_NUMBER", booking.room.RoomNumber);

                    SqlDataReader rdBooking = await cmdBooking.ExecuteReaderAsync();

                    while (rdBooking.Read())
                    {
                        resp.IdStatus = new BookingStatus();
                        resp.room = new Room();
                        resp.guests = new List<Guest>();

                        resp.IdBooking = Convert.ToInt64(rdBooking["IdBooking"].ToString());
                        resp.IdStatus.IdStatus = Convert.ToInt32(rdBooking["IdStatus"].ToString());
                        resp.IdStatus.StatusName = rdBooking["StatusName"].ToString();
                        resp.IdStatus.Description = rdBooking["Description"].ToString();
                        resp.room.RoomNumber = Convert.ToInt32(rdBooking["RoomNumber"].ToString());
                        resp.room.BaseValue = Convert.ToDecimal(rdBooking["BaseValue"].ToString());
                        resp.room.HasWifi = Convert.ToBoolean(rdBooking["HasWifi"].ToString());
                        resp.room.HasTV = Convert.ToBoolean(rdBooking["HasTV"].ToString());
                        resp.room.HasBalcony = Convert.ToBoolean(rdBooking["HasBalcony"].ToString());
                        resp.room.MinGuest = Convert.ToInt32(rdBooking["MinGuest"].ToString());
                        resp.room.MaxGuest = Convert.ToInt32(rdBooking["MaxGuest"].ToString());
                        resp.room.IsActive = Convert.ToBoolean(rdBooking["IsActive"].ToString());
                        resp.StartDate = Convert.ToDateTime(rdBooking["StartDate"].ToString());
                        resp.FinalDate = Convert.ToDateTime(rdBooking["FinalDate"].ToString());
                        resp.Price = Convert.ToDecimal(rdBooking["Price"].ToString());
                    }

                    rdBooking.Close();

                    #endregion

                    #region Delete Guest by Id booking

                    SqlCommand cmdDelete = new SqlCommand(Constant.StoredProcedure.DeleteGuestByBooking, conn);
                    cmdDelete.CommandType = CommandType.StoredProcedure;
                    cmdDelete.Transaction = transaction;

                    cmdDelete.Parameters.AddWithValue("@ID_BOOKING", IdBooking);

                    await cmdDelete.ExecuteNonQueryAsync();

                    #endregion

                    #region Update Guests

                    var countPayingGuest = booking.guests.Where(x => x.Age >= 0 && x.Age < 5).Count();

                    if (countPayingGuest >= resp.room.MinGuest && countPayingGuest <= resp.room.MaxGuest)
                    {
                        foreach (Guest guest in booking.guests)
                        {
                            SqlCommand cmdGuest = new SqlCommand(Constant.StoredProcedure.InsertGuest, conn);
                            cmdGuest.CommandType = CommandType.StoredProcedure;
                            cmdGuest.Transaction = transaction;

                            cmdGuest.Parameters.AddWithValue("@ID_BOOKING", resp.IdBooking);
                            cmdGuest.Parameters.AddWithValue("@NAMES", guest.Names);
                            cmdGuest.Parameters.AddWithValue("@SURNAMES", guest.Surnames);
                            cmdGuest.Parameters.AddWithValue("@ID_TYPE", guest.IdType.Id);
                            cmdGuest.Parameters.AddWithValue("@IDENTIFICATION", guest.Identification);
                            cmdGuest.Parameters.AddWithValue("@AGE", guest.Age);
                            cmdGuest.Parameters.AddWithValue("@ID_GENDER", guest.gender.IdGender);

                            if (guest.Email == null) { cmdGuest.Parameters.AddWithValue("@EMAIL", DBNull.Value); } else { cmdGuest.Parameters.AddWithValue("@EMAIL", guest.Email); }
                            if (guest.Phone == null) { cmdGuest.Parameters.AddWithValue("@PHONE", DBNull.Value); } else { cmdGuest.Parameters.AddWithValue("@PHONE", guest.Phone); }

                            SqlDataReader rdGuest = await cmdGuest.ExecuteReaderAsync();

                            Guest guestResp = new Guest();
                            guestResp.IdType = new IdType();
                            guestResp.gender = new Gender();

                            while (rdGuest.Read())
                            {
                                guestResp.IdBooking = Convert.ToInt64(rdGuest["IdBooking"].ToString());
                                guestResp.Names = rdGuest["Names"].ToString();
                                guestResp.Surnames = rdGuest["Surnames"].ToString();
                                guestResp.IdType.Id = Convert.ToInt32(rdGuest["Id"].ToString());
                                guestResp.IdType.IdTypeName = rdGuest["IdTypeName"].ToString();
                                guestResp.IdType.Acronym = rdGuest["Acronym"].ToString();
                                guestResp.Identification = rdGuest["Identification"].ToString();
                                guestResp.Age = Convert.ToInt32(rdGuest["Age"].ToString());
                                guestResp.gender.IdGender = Convert.ToInt32(rdGuest["IdGender"].ToString());
                                guestResp.gender.GenderName = rdGuest["GenderName"].ToString();

                                guestResp.Email = rdGuest["Email"] == DBNull.Value ? null : rdGuest["Email"].ToString();
                                guestResp.Phone = rdGuest["Phone"] == DBNull.Value ? null : rdGuest["Phone"].ToString();
                            }

                            resp.guests.Add(guestResp);

                            rdGuest.Close();
                        }
                    }
                    else
                    {
                        throw new CANCUNBadRequestException($"Guest count greater or equal than {resp.room.MinGuest} and less or equal than {resp.room.MaxGuest}");
                    }

                    #endregion

                    #region Update booking price

                    var adultsNum = resp.guests.Where(guests => guests.Age > 12).Count();
                    var childsNum = resp.guests.Where(guest => guest.Age >= 5 && guest.Age <= 12).Count();

                    SqlCommand cmdPrice = new SqlCommand(Constant.StoredProcedure.UpdatePrice, conn);
                    cmdPrice.CommandType = CommandType.StoredProcedure;
                    cmdPrice.Transaction = transaction;

                    cmdPrice.Parameters.AddWithValue("@ID_BOOKING", resp.IdBooking);
                    cmdPrice.Parameters.AddWithValue("@START_DATE", resp.StartDate);
                    cmdPrice.Parameters.AddWithValue("@FINAL_DATE", resp.FinalDate);
                    cmdPrice.Parameters.AddWithValue("@ROOM_NUMBER", resp.room.RoomNumber);
                    cmdPrice.Parameters.AddWithValue("@ADULTS", adultsNum);
                    cmdPrice.Parameters.AddWithValue("@CHILDS", childsNum);

                    SqlDataReader rdPrice = await cmdPrice.ExecuteReaderAsync();

                    while (rdPrice.Read())
                    {
                        resp.Price = Convert.ToDecimal(rdPrice["Price"].ToString());
                    }

                    rdPrice.Close();

                    #endregion

                    transaction.Commit();
                }
                catch (CANCUNBadRequestException ex)
                {
                    if (transaction != null) transaction.Rollback();
                    throw new CANCUNBadRequestException(ex.Message, ex);
                }
                catch (SqlException ex)
                {
                    if (transaction != null) transaction.Rollback();
                    if (ex.Number > 50000 && ex.Number < 50050) throw new CANCUNBadRequestException(ex.Message);
                    else throw new Exception("SQL Exception", ex);
                }
                catch (Exception ex)
                {
                    if (transaction != null) transaction.Rollback();
                    throw new Exception("General Exception", ex);
                }

            }

            return resp;
        }

        public static async Task<Entities.Bookings.Booking> UpdateGuestsBooking(long IdBooking, ListGuest Guests, string login)
        {
            Entities.Bookings.Booking resp = await GetBookingByIdBooking(IdBooking);

            if (resp == null || resp.IdBooking == 0)
            {
                throw new CANCUNBadRequestException($"IdBooking {IdBooking} not exist");
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        await conn.OpenAsync();
                        transaction = conn.BeginTransaction();

                        #region Update booking Status

                        SqlCommand cmdStatus = new SqlCommand(Constant.StoredProcedure.UpdateState, conn);
                        cmdStatus.CommandType = CommandType.StoredProcedure;
                        cmdStatus.Transaction = transaction;

                        cmdStatus.Parameters.AddWithValue("@ID_BOOKING", IdBooking);
                        cmdStatus.Parameters.AddWithValue("@NEW_STATE", 4);
                        cmdStatus.Parameters.AddWithValue("@MESSAGE", JsonSerializer.Serialize(Guests));
                        cmdStatus.Parameters.AddWithValue("@LOGIN", login);

                        SqlDataReader rdStatus = await cmdStatus.ExecuteReaderAsync();

                        while (rdStatus.Read())
                        {
                            resp.IdStatus.IdStatus = Convert.ToInt32(rdStatus["IdStatus"].ToString());
                            resp.IdStatus.StatusName = rdStatus["StatusName"].ToString();
                            resp.IdStatus.Description = rdStatus["Description"].ToString();
                        }

                        rdStatus.Close();

                        #endregion

                        #region Delete Guest by Id booking

                        SqlCommand cmdDelete = new SqlCommand(Constant.StoredProcedure.DeleteGuestByBooking, conn);
                        cmdDelete.CommandType = CommandType.StoredProcedure;
                        cmdDelete.Transaction = transaction;

                        cmdDelete.Parameters.AddWithValue("@ID_BOOKING", IdBooking);

                        await cmdDelete.ExecuteNonQueryAsync();

                        #endregion

                        #region Insert Guests

                        if (resp.guests.Count != Guests.Guests.Count)
                        {
                            throw new CANCUNBadRequestException($"Guest count should be equal than this booking. Guests Number: {resp.guests.Count}");
                        }

                        for (int i = 0; i < resp.guests.Count; i++)
                        {
                            Guests.Guests[i].IdBooking = IdBooking;
                            Guests.Guests[i].Age = resp.guests[i].Age;
                        }

                        foreach (Guest guest in Guests.Guests)
                        {
                            SqlCommand cmdGuest = new SqlCommand(Constant.StoredProcedure.InsertGuest, conn);
                            cmdGuest.CommandType = CommandType.StoredProcedure;
                            cmdGuest.Transaction = transaction;

                            cmdGuest.Parameters.AddWithValue("@ID_BOOKING", resp.IdBooking);
                            cmdGuest.Parameters.AddWithValue("@NAMES", guest.Names);
                            cmdGuest.Parameters.AddWithValue("@SURNAMES", guest.Surnames);
                            cmdGuest.Parameters.AddWithValue("@ID_TYPE", guest.IdType.Id);
                            cmdGuest.Parameters.AddWithValue("@IDENTIFICATION", guest.Identification);
                            cmdGuest.Parameters.AddWithValue("@ID_GENDER", guest.gender.IdGender);

                            if (guest.Email == null) { cmdGuest.Parameters.AddWithValue("@EMAIL", DBNull.Value); } else { cmdGuest.Parameters.AddWithValue("@EMAIL", guest.Email); }
                            if (guest.Phone == null) { cmdGuest.Parameters.AddWithValue("@PHONE", DBNull.Value); } else { cmdGuest.Parameters.AddWithValue("@PHONE", guest.Phone); }

                            SqlDataReader rdGuest = await cmdGuest.ExecuteReaderAsync();

                            Guest guestResp = new Guest();
                            guestResp.IdType = new IdType();
                            guestResp.gender = new Gender();

                            while (rdGuest.Read())
                            {
                                guestResp.IdBooking = Convert.ToInt64(rdGuest["IdBooking"].ToString());
                                guestResp.Names = rdGuest["Names"].ToString();
                                guestResp.Surnames = rdGuest["Surnames"].ToString();
                                guestResp.IdType.Id = Convert.ToInt32(rdGuest["Id"].ToString());
                                guestResp.IdType.IdTypeName = rdGuest["IdTypeName"].ToString();
                                guestResp.IdType.Acronym = rdGuest["Acronym"].ToString();
                                guestResp.Identification = rdGuest["Identification"].ToString();
                                guestResp.Age = Convert.ToInt32(rdGuest["Age"].ToString());
                                guestResp.gender.IdGender = Convert.ToInt32(rdGuest["IdGender"].ToString());
                                guestResp.gender.GenderName = rdGuest["GenderName"].ToString();

                                guestResp.Email = rdGuest["Email"] == DBNull.Value ? null : rdGuest["Email"].ToString();
                                guestResp.Phone = rdGuest["Phone"] == DBNull.Value ? null : rdGuest["Phone"].ToString();
                            }

                            resp.guests.Add(guestResp);

                            rdGuest.Close();
                        }

                        #endregion

                        #region Update booking price

                        var adultsNum = resp.guests.Where(guests => guests.Age > 12).Count();
                        var childsNum = resp.guests.Where(guest => guest.Age >= 5 && guest.Age <= 12).Count();

                        SqlCommand cmdPrice = new SqlCommand(Constant.StoredProcedure.UpdatePrice, conn);
                        cmdPrice.CommandType = CommandType.StoredProcedure;
                        cmdPrice.Transaction = transaction;

                        cmdPrice.Parameters.AddWithValue("@ID_BOOKING", resp.IdBooking);
                        cmdPrice.Parameters.AddWithValue("@START_DATE", resp.StartDate);
                        cmdPrice.Parameters.AddWithValue("@FINAL_DATE", resp.FinalDate);
                        cmdPrice.Parameters.AddWithValue("@ROOM_NUMBER", resp.room.RoomNumber);
                        cmdPrice.Parameters.AddWithValue("@ADULTS", adultsNum);
                        cmdPrice.Parameters.AddWithValue("@CHILDS", childsNum);

                        SqlDataReader rdPrice = await cmdPrice.ExecuteReaderAsync();

                        while (rdPrice.Read())
                        {
                            resp.Price = Convert.ToDecimal(rdPrice["Price"].ToString());
                        }

                        rdPrice.Close();

                        #endregion

                        transaction.Commit();
                    }
                    catch (CANCUNBadRequestException ex)
                    {
                        if (transaction != null) transaction.Rollback();
                        throw new CANCUNBadRequestException(ex.Message, ex);
                    }
                    catch (SqlException ex)
                    {
                        if (transaction != null) transaction.Rollback();
                        if (ex.Number > 50000 && ex.Number < 50050) throw new CANCUNBadRequestException(ex.Message);
                        else throw new Exception("SQL Exception", ex);
                    }
                    catch (Exception ex)
                    {
                        if (transaction != null) transaction.Rollback();
                        throw new Exception("General Exception", ex);
                    }

                }
            }

            

            return resp;
        }

        public static async Task<Entities.Bookings.Booking> CancelBooking(long IdBooking, string login)
        {
            Entities.Bookings.Booking resp = await GetBookingByIdBooking(IdBooking);

            if (resp == null || resp.IdBooking == 0)
            {
                throw new CANCUNBadRequestException($"IdBooking {IdBooking} not exist");
            }
            else
            {
                int newStatus = 0;
                switch(resp.IdStatus.IdStatus)
                {
                    case 1:
                    case 3:
                        newStatus = 5;
                        break;
                    case 2:
                    case 4:
                        newStatus = 6;
                        break;
                    default:
                        newStatus = resp.IdStatus.IdStatus;
                        break;
                }

                using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        await conn.OpenAsync();
                        transaction = conn.BeginTransaction();

                        #region Update booking Status

                        SqlCommand cmdStatus = new SqlCommand(Constant.StoredProcedure.UpdateState, conn);
                        cmdStatus.CommandType = CommandType.StoredProcedure;
                        cmdStatus.Transaction = transaction;

                        cmdStatus.Parameters.AddWithValue("@ID_BOOKING", resp.IdBooking);
                        cmdStatus.Parameters.AddWithValue("@NEW_STATE", newStatus);
                        cmdStatus.Parameters.AddWithValue("@MESSAGE", JsonSerializer.Serialize(resp));
                        cmdStatus.Parameters.AddWithValue("@LOGIN", login);

                        SqlDataReader rdStatus = await cmdStatus.ExecuteReaderAsync();

                        while (rdStatus.Read())
                        {
                            resp.IdStatus.IdStatus = Convert.ToInt32(rdStatus["IdStatus"].ToString());
                            resp.IdStatus.StatusName = rdStatus["StatusName"].ToString();
                            resp.IdStatus.Description = rdStatus["Description"].ToString();
                        }

                        rdStatus.Close();

                        #endregion

                        transaction.Commit();
                    }
                    catch (CANCUNBadRequestException ex)
                    {
                        if (transaction != null) transaction.Rollback();
                        throw new CANCUNBadRequestException(ex.Message, ex);
                    }
                    catch (SqlException ex)
                    {
                        if (transaction != null) transaction.Rollback();
                        if (ex.Number > 50000 && ex.Number < 50050) throw new CANCUNBadRequestException(ex.Message);
                        else throw new Exception("SQL Exception", ex);
                    }
                    catch (Exception ex)
                    {
                        if (transaction != null) transaction.Rollback();
                        throw new Exception("General Exception", ex);
                    }

                }
            }

            return resp;
        }

        public static async Task<List<BookingLog>> GetBookingLogs(long IdBooking)
        {
            List<BookingLog> bookingLogs = new List<BookingLog>();

            using (SqlConnection conn = new SqlConnection(Connections.GetConnectionString(Connections.TYPE.CANCUN)))
            {
                SqlCommand cmd = new SqlCommand(Constant.StoredProcedure.SelectBookingLogs, conn);
                cmd.Parameters.AddWithValue("@ID_BOOKING", IdBooking);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                SqlDataReader Rd = await cmd.ExecuteReaderAsync();

                while (Rd.Read())
                {
                    BookingLog log = new BookingLog();
                    log.Status = new BookingStatus();

                    log.IdBooking = Convert.ToInt64(Rd["IdBooking"].ToString());
                    log.Status.IdStatus = Convert.ToInt32(Rd["IdStatus"].ToString());
                    log.Status.StatusName = Rd["StatusName"].ToString();
                    log.Status.Description = Rd["Description"].ToString();
                    log.LogMessage = Rd["LogMessage"].ToString();
                    log.EventDate = Convert.ToDateTime(Rd["EventDate"].ToString());

                    bookingLogs.Add(log);
                }
            }

            return bookingLogs;
        }
    }
}