using System.Net;

namespace CancunHotel.Api.Models.Errors
{
    /// <summary>
    /// InvalidModelError
    /// </summary>
    public class InvalidModelError :ApiError
    {
        /// <summary>
        /// InvalidModelError
        /// </summary>
        public InvalidModelError(object modelState) : base(400, HttpStatusCode.BadRequest.ToString(), modelState)
        {

        }
    }
}
