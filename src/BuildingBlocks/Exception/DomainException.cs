using BuildingBlocks.Exception;
using System.Net;

namespace SmartCharging.Infrastructure.Exceptions
{
    public class DomainException : CustomException
    {
        public DomainException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message, statusCode)
        {
        }

        public DomainException(string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.BadRequest, int? code = null) : base(message, innerException, statusCode, code)
        {
        }
    }
}
