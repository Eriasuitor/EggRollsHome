using System;
using System.Text;
using Newegg.API.Client;

namespace Newegg.MIS.API.Utilities.Exceptions
{
    public class APIResourceProxyException : Exception
    {
        public APIResourceProxyException(string message) : base(message)
        {
        }

        public APIResourceProxyException(string message, Exception innterException)
            : base(message, innterException)
        {
        }

        public APIResourceProxyException(Exception exception) : 
            base(BuildMessage(exception), exception)
        { }

        private static string BuildMessage(
            Exception exception)
        {
            var webServiceException = exception as WebServiceException;

            if (null == webServiceException) return exception.Message;

            var messageContent = new StringBuilder();
            if (webServiceException.HasValidationError)
            {
                messageContent.Append("Validation Error occurred");

                if (null != webServiceException.ResponseDto)
                {
                    foreach (var validationErrorResponse in
                        webServiceException.ResponseDto.ValidationErrors)
                    {
                        messageContent.Append(", ");
                        messageContent.Append(
                            validationErrorResponse.ErrorMessage);
                    }
                }
                else
                {
                    messageContent.Append(", but no detailed information provided.");
                }
            }
            else
            {
                messageContent.Append(webServiceException.Message);
            }

            return messageContent.ToString();
        }
    }
}
