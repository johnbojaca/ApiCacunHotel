using CancunHotel.Api.Models.Errors;
using CancunHotel.BL;
using CancunHotel.Entities.Bookings;
using CancunHotel.Entities.Exceptions;
using CancunHotel.Entities.Guests;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace CancunHotel.Api.Controllers
{
    /// <summary>
    /// Booking Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private IValidator<Booking> _bookingValidator { get; }
        private IValidator<ListGuest> _listGuestValidator { get; }

        /// <summary>
        /// Booking Controller Constructor
        /// </summary>
        /// <param name="bookingValidator"></param>
        /// <param name="listGuestValidator"></param>
        public BookingsController(IValidator<Entities.Bookings.Booking> bookingValidator, IValidator<ListGuest> listGuestValidator)
        {
            _bookingValidator = bookingValidator;
            _listGuestValidator = listGuestValidator;
        }

        /// <summary>
        /// Get a Booking by IdBooking
        /// </summary>
        /// <param name="IdBooking"></param>
        /// <returns></returns>
        [HttpGet("GetBookingByIdBooking")]
        public async Task<ActionResult<Entities.Bookings.Booking>> GetBookingByIdBooking(long IdBooking)
        {
            Entities.Bookings.Booking booking = new Entities.Bookings.Booking();

            try
            {
                if (IdBooking <= 0) return BadRequest("IdBooking parameter should be greater that 0");

                BookingBL bookingBL = new BookingBL();
                booking = await bookingBL.GetBookingByIdBooking(IdBooking);

                if (booking == null || booking.IdBooking == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"No results");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex}");
            }
            return Ok(booking);
        }

        /// <summary>
        /// Get Bookings by Identification of Guest
        /// </summary>
        /// <param name="IdType"></param>
        /// <param name="Identification"></param>
        /// <returns></returns>
        [HttpGet("GetBookingByGuestIdentification")]
        public async Task<ActionResult<List<Entities.Bookings.Booking>>> GetBookingByIdentification(int IdType, string Identification)
        {
            List<Entities.Bookings.Booking> bookings = new List<Entities.Bookings.Booking>();

            try
            {
                StringBuilder error = new StringBuilder();

                if (IdType <= 0) error.Append("IdType parameter should be greater that 0 - ");
                if (string.IsNullOrEmpty(Identification)) error.Append("Identification is required");

                if (!string.IsNullOrEmpty(error.ToString())) return BadRequest(error.ToString());
                
                BookingBL bookingBL = new BookingBL();
                bookings = await bookingBL.GetBookingByIdentification(IdType, Identification);

                if (bookings.Count == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"No results");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex}");
            }
            return Ok(bookings);
        }

        /// <summary>
        /// Insert a Booking
        /// </summary>
        /// <param name="Booking"></param>
        /// <returns></returns>
        [HttpPost("InsertBooking")]
        public async Task<ActionResult<Entities.Bookings.Booking>> InsertBooking([FromBody] Entities.Bookings.Booking Booking)
        {
            Entities.Bookings.Booking booking = new Entities.Bookings.Booking();

            try
            {
                var validation = await _bookingValidator.ValidateAsync(Booking);

                if (validation.IsValid)
                {
                    var claimsIdentity = User?.Identity as ClaimsIdentity;
                    var login = claimsIdentity?.FindFirst("Username")?.Value;

                    if (login == null) login = "TEST_CANCUN";

                    BookingBL bookingBL = new BookingBL();
                    booking = await bookingBL.InsertBooking(Booking, login);
                }
                else
                {
                    var errors = validation.Errors?.Select(x => new { x.PropertyName, x.ErrorMessage }).ToList();
                    return StatusCode(InternalServerError.StatusCodeModel, new InvalidModelError(errors));
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

            return Ok(booking);
        }

        /// <summary>
        /// Update a Booking without payment
        /// </summary>
        /// <param name="Booking"></param>
        /// <param name="IdBooking"></param>
        /// <returns></returns>
        [HttpPut("UpdateBooking")]
        public async Task<ActionResult<Entities.Bookings.Booking>> UpdateBooking([FromBody] Entities.Bookings.Booking Booking, long IdBooking)
        {
            Entities.Bookings.Booking booking = new Entities.Bookings.Booking();

            try
            {
                var validation = await _bookingValidator.ValidateAsync(Booking);

                if (validation.IsValid)
                {
                    var claimsIdentity = User?.Identity as ClaimsIdentity;
                    var login = claimsIdentity?.FindFirst("Username")?.Value;

                    if (login == null) login = "TEST_CANCUN";

                    BookingBL bookingBL = new BookingBL();
                    booking = await bookingBL.UpdateBooking(IdBooking, Booking, login);
                }
                else
                {
                    var errors = validation.Errors?.Select(x => new { x.PropertyName, x.ErrorMessage }).ToList();
                    return StatusCode(InternalServerError.StatusCodeModel, new InvalidModelError(errors));
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

            return Ok(booking);
        }

        /// <summary>
        /// Update Guest Booking with Payment
        /// </summary>
        /// <param name="Guests"></param>
        /// <param name="IdBooking"></param>
        /// <returns></returns>
        [HttpPut("UpdateGuestsBooking")]
        public async Task<ActionResult<Entities.Bookings.Booking>> UpdateGuestsBooking([FromBody] ListGuest Guests, long IdBooking)
        {
            Entities.Bookings.Booking booking = new Entities.Bookings.Booking();

            try
            {
                var validation = await _listGuestValidator.ValidateAsync(Guests);

                if (validation.IsValid)
                {
                    var claimsIdentity = User?.Identity as ClaimsIdentity;
                    var login = claimsIdentity?.FindFirst("Username")?.Value;

                    if (login == null) login = "TEST_CANCUN";

                    BookingBL bookingBL = new BookingBL();
                    booking = await bookingBL.UpdateGuestsBooking(IdBooking, Guests, login);
                }
                else
                {
                    var errors = validation.Errors?.Select(x => new { x.PropertyName, x.ErrorMessage }).ToList();
                    return StatusCode(InternalServerError.StatusCodeModel, new InvalidModelError(errors));
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

            return Ok(booking);
        }

        /// <summary>
        /// Cancel a Booking
        /// </summary>
        /// <param name="IdBooking"></param>
        /// <returns></returns>
        [HttpPut("CancelBooking")]
        public async Task<ActionResult<Entities.Bookings.Booking>> CancelBooking(long IdBooking)
        {
            Entities.Bookings.Booking booking = new Entities.Bookings.Booking();

            try
            {
                var claimsIdentity = User?.Identity as ClaimsIdentity;
                var login = claimsIdentity?.FindFirst("Username")?.Value;
                
                if (login == null) login = "TEST_CANCUN";
                
                BookingBL bookingBL = new BookingBL();
                booking = await bookingBL.CancelBooking(IdBooking, login);
            }
            catch (CANCUNBadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex}");
            }

            return Ok(booking);
        }

        /// <summary>
        /// Get booking logs
        /// </summary>
        /// <param name="IdBooking"></param>
        /// <returns></returns>
        [HttpGet("GetBookingLogs")]
        public async Task<ActionResult<List<BookingLog>>> GetBookingLogs(long IdBooking)
        {
            List<BookingLog> bookingLogs = new List<BookingLog>();

            try
            {
                BookingBL bookingBL = new BookingBL();
                bookingLogs = await bookingBL.GetBookingLogs(IdBooking);

                if (bookingLogs == null || bookingLogs.Count == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"No results");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex}");
            }

            return Ok(bookingLogs);
        }
    }
}
