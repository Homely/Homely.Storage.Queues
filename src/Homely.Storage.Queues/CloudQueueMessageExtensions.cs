using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;

namespace Homely.Storage.Queues
{
    internal static class CloudQueueMessageExtensions
    {
        internal static Message<T> DeserializeMessage<T>(this CloudQueueMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var model = JsonConvert.DeserializeObject<T>(message.AsString);
            return message.ToMessage(model);
        }

        internal static Message<T> ToMessage<T>(this CloudQueueMessage message, T model)
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
                                  message.Id, 
                                  message.PopReceipt, 
                                  message.DequeueCount);
        }
    }
}
