using System.Net;

namespace CancunHotel.Api.Models.Errors
{
    /// <summary>
    /// UnauthorizedError
    /// </summary>
    public class UnauthorizedError :ApiError
    {
        /// <summary>
        /// UnauthorizedError
        /// </summary>
        public UnauthorizedError(): base(401, HttpStatusCode.Unauthorized.ToString())
        {

        }

        /// <summary>
        /// UnauthorizedError
        /// </summary>
        public UnauthorizedError(string message): base(401, HttpStatusCode.Unauthorized.ToString(), message)
        {

        }
    }
}
