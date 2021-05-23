using System;

namespace TransnationalLanka.ThreePL.Core.Exceptions
{
    public class ServiceException : Exception
    {
        public ErrorMessage[] Messages { get; set; }

        public ServiceException(ErrorMessage[] messages)
        {
            Messages = messages;
        }

        public ServiceException(ErrorMessage[] messages, Exception exception) 
            : base(innerException: exception, message: null)
        {
            Messages = messages;
        }
    }

    public class ErrorMessage
    {
        public string Code { get; set; }
        public object Meta { get; set; }
        public string Message { get; set; }
    }
}
