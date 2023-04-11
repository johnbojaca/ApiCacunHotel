using System;
using System.Net;

namespace CancunHotel.Api.Models.Errors
{
    /// <summary>
    /// Manejo de errores internos
    /// </summary>
    public class InternalServerError : ApiError
    {
        /// <summary>
        /// StatusCode
        /// </summary>
        public new static int StatusCode { get; } = 500;
        
        /// <summary>
        /// StatusCodeModel
        /// </summary>
        public static int StatusCodeModel { get; } = 400;

        /// <summary>
        /// Manejo de errores internos
        /// </summary>
        public InternalServerError(): base(500, HttpStatusCode.InternalServerError.ToString())
        {

        }

        /// <summary>
        /// Manejo de errores internos
        /// </summary>
        public InternalServerError(Exception exception, string username) : base(500, HttpStatusCode.InternalServerError.ToString(), exception.Message)
        {
            //AppLogs.Set(exception.Message, log4net.Core.Level.Critical, username, exception);
        }
    }
}
