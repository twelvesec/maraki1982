using System;

namespace Maraki1982.Web.Exceptions
{
    public class StatusCodeException : Exception
    {
        public StatusCodeException(string message) : base(message)
        {
        }
    }
}
