using System;
using Newegg.FrameworkAPI.SDK.MQ;

namespace Newegg.MIS.API.Utilities.Helpers
{
    public class MessageQueueHelper
    {
        public static void SendMessageJson<T>(T message, string queueName)
        {
            SendMessageJson(message, queueName, string.Empty);
        }

        public static void SendMessageJson<T>(T message, string queueName, string password)
        {
            PublishResultInfo result;

            if (string.IsNullOrWhiteSpace(password))
            {
                result = MessagePublisher.SendMessageJson(message, queueName, null);
            }
            else
            {
                result = MessagePublisher.SendMessageJson(message, queueName, password, null, null);
            }

            if (!result.IsSucceed)
            {
                throw new ApplicationException(
                    string.Format("Failed to send response message {0}[{1}] to queue [{2}] due to {3}(Code:{4}).",
                    message.GetType().Name,
                    message.GetHashCode(),
                    queueName,
                    result.ErrorMessage,
                    result.ErrorCode));
            }
        }
    }
}
