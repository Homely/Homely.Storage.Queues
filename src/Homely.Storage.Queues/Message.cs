using Microsoft.WindowsAzure.Storage.Queue;
using System;

namespace Homely.Storage.Queues
{
    public class Message : Message<string>
    {
        public Message(CloudQueueMessage message) : base(message.AsString, message)
        { }
    }

    public class Message<T>
    {
        public Message(T model,
                       CloudQueueMessage message)
        {
            Model = model;
            CloudQueueMessage = message
                                ?? throw new ArgumentNullException(nameof(message));
        }

        public CloudQueueMessage CloudQueueMessage { get; }
        public string Id { get => CloudQueueMessage?.Id; }
        public string Receipt { get => CloudQueueMessage?.PopReceipt; }

        public int DeQueueCount
        {
            get => CloudQueueMessage == null
                      ? default
                      : CloudQueueMessage.DequeueCount;
        }

        public T Model { get; }
    }
}
