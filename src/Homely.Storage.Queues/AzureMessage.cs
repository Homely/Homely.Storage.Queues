using Microsoft.WindowsAzure.Storage.Queue;

namespace Homely.Storage.Queues
{
    /// <summary>
    /// An Azure queue message content.
    /// </summary>
    /// <remarks>The <code>Model</code> is a specific type, provided.</remarks>
    public class AzureMessage : Message
    {
        public AzureMessage(CloudQueueMessage message) : base(message.AsString, message.Id, message.PopReceipt, message.DequeueCount)
        {
        }
    }

    /// <summary>
    /// An Azure queue message content.
    /// </summary>
    /// <remarks>The <code>Model</code> is a specific type, provided.</remarks>
    public class AzureMessage<T> : Message<T>
    {
        public AzureMessage(T model, CloudQueueMessage message) : base(model, message.Id, message.PopReceipt, message.DequeueCount)
        {
        }
    }
}
