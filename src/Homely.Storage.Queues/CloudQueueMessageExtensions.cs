using Azure.Storage.Queues.Models;
using Newtonsoft.Json;
using System;

namespace Homely.Storage.Queues
{    
    internal static class CloudQueueMessageExtensions
    {        
        internal static Message<T> DeserializeMessage<T>(this QueueMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var model = JsonConvert.DeserializeObject<T>(message.Body.ToString());
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
