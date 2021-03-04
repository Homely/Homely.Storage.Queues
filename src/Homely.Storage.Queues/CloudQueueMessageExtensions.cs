using Azure.Storage.Queues.Models;
using System;

namespace Homely.Storage.Queues
{    internal static class CloudQueueMessageExtensions
    {        internal static Message<T> DeserializeMessage<T>(this QueueMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var model = message.Body.ToObjectFromJson<T>();
            return message.ToMessage(model);
        }

        internal static Message<T> ToMessage<T>(this QueueMessage message, T model)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return new Message<T>(model, 
                                  message.MessageId,
                                  message.PopReceipt, 
                                  message.DequeueCount);
        }
    }
}
