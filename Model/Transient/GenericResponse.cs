using System;
using System.Net;

namespace Planner.Model.Transient
{
    public class GenericResponse : IResponse
    {
        /// <summary>
        /// The HTTP status code.
        /// </summary>
        public int? Code { get; set; }

        /// <summary>
        /// The internal status code of the message.
        /// See .\Documentation\StatusCodes.md for the list of codes.
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// A reason for a possible problem with the authorization process.
        /// </summary>
        public string Message { get; set; }


        public static GenericResponse TimeoutResponse()
        {
            return new GenericResponse
            {
                Code = (int)HttpStatusCode.BadRequest,
                Status = (int)StatusCode.Timeout
            };
        }

        public static GenericResponse ExceptionResponse(Exception ex)
        {
            return new GenericResponse
            {
                Code = (int)HttpStatusCode.BadRequest,
                Status = (int)StatusCode.UnknowException,
                Message = ex.Message
            };
        }
    }
}