using CancunHotel.BL;
using CancunHotel.Entities.Exceptions;
using CancunHotel.Entities.Rooms;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CancunHotel.Api.Controllers
{
    /// <summary>
    /// Room Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RoomsController : ControllerBase
    {
        /// <summary>
        /// Get rooms
        /// </summary>
        /// <param name="RoomNumber"></param>
        /// <param name="IsActive"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Room>>> GetRooms(int? RoomNumber = null, bool? IsActive = null)
        {
            List<Room> rooms = new List<Room>();

            try
            {
                RoomBL roomBL = new RoomBL();
                rooms = await roomBL.GetRooms(RoomNumber, IsActive);

                if (rooms.Count == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"No results");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex}");
            }

            return Ok(rooms);
        }

        /// <summary>
        /// Get Available Days the next 30 days
        /// </summary>
        /// <param name="RoomNumber"></param>
        /// <returns></returns>
        [HttpGet("AvailableDays")]
        public async Task<ActionResult<List<DateTime>>> GetAvailableDays(int RoomNumber)
        {
            List<DateTime> dates = new List<DateTime>();

            try
            {
                if (RoomNumber <= 0) return BadRequest("Room number should be greater that 0");

                RoomBL roomBL = new RoomBL();
                dates = await roomBL.GetAvailableDays(RoomNumber);

                if (dates.Count == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"No results");
                }
            }
            catch (CANCUNBadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex}");
            }

            return Ok(dates);
        }

        /// <summary>
        /// Get quote of room
        /// </summary>
        /// <param name="RoomNumber"></param>
        /// <param name="StarteDate"></param>
        /// <param name="FinalDate"></param>
        /// <param name="Adults"></param>
        /// <param name="Childs"></param>
        /// <returns></returns>
        [HttpGet("Quote")]
        public async Task<ActionResult<decimal>> QuoteRoom(int RoomNumber, DateTime StarteDate, DateTime FinalDate, int Adults, int Childs)
        {
            decimal price = 0;
            try
            {
                StringBuilder error = new StringBuilder();

                if (RoomNumber <= 0) error.Append("Room number should be greater that 0 - ");
                if (Adults < 0) error.Append("Adults number should be greater or equal that 0 - ");
                if (Adults < 0) error.Append("Room number should be greater or equal that 0 ");
                if (!string.IsNullOrEmpty(error.ToString())) return BadRequest(error.ToString());

                RoomBL roomBL = new RoomBL();
                price = await roomBL.QuoteRoom(RoomNumber, StarteDate, FinalDate, Adults, Childs);

                if (price == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"No results");
                }
            }
            catch (CANCUNBadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex}");
            }

            return Ok(price);
        }
    }
}
