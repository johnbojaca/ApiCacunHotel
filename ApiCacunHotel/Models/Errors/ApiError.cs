using Newtonsoft.Json;

namespace CancunHotel.Api.Models.Errors
{
    /// <summary>
    /// ApiError
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// StatusCode
        /// </summary>
        public int StatusCode { get; private set; }

        /// <summary>
        /// StatusDescription
        /// </summary>
        public string StatusDescription { get; private set; }

        /// <summary>
        /// Message
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object Message { get; private set; }

        /// <summary>
        /// ApiError
        /// </summary>
        public ApiError(int statusCode, string statusDescription)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = statusDescription;
        }

        /// <summary>
        /// ApiError
        /// </summary>
        public ApiError(int statusCode, string statusDescription, object message) : this(statusCode, statusDescription)
        {
            this.Message = message;
        }
    }
}
