using CancunHotel.BL;
using CancunHotel.Entities.Bookings;
using CancunHotel.Entities.Guests;
using Microsoft.AspNetCore.Mvc;

namespace CancunHotel.Api.Controllers
{
    /// <summary>
    /// Parameters Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ParametersController : ControllerBase
    {
        /// <summary>
        /// Get Genders
        /// </summary>
        /// <returns></returns>
        [HttpGet("Genders")]
        public async Task<ActionResult<List<Gender>>> GetGenders()
        {
            List<Gender> genders = new List<Gender>();

            try
            {
                ParameterBL parametersBL = new ParameterBL();
                genders = await parametersBL.GetGenders();

                if (genders.Count == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"No results");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex}");
            }

            return Ok(genders);
        }

        /// <summary>
        /// Get Identification Types
        /// </summary>
        /// <returns></returns>
        [HttpGet("IdTypes")]
        public async Task<ActionResult<List<IdType>>> GetIdTypes()
        {
            List<IdType> idTypes = new List<IdType>();

            try
            {
                ParameterBL parametersBL = new ParameterBL();
                idTypes = await parametersBL.GetIdTypes();

                if (idTypes.Count == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"No results");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex}");
            }

            return Ok(idTypes);
        }

        /// <summary>
        /// Get Booking States
        /// </summary>
        /// <returns></returns>
        [HttpGet("BookingStates")]
        public async Task<ActionResult<List<BookingStatus>>> GetBookingStates()
        {
            List<BookingStatus> bookingStates = new List<BookingStatus>();

            try
            {
                ParameterBL parametersBL = new ParameterBL();
                bookingStates = await parametersBL.GetBookingStates();

                if (bookingStates.Count == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"No results");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex}");
            }

            return Ok(bookingStates);
        }
    }
}
