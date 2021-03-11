using Azure.Storage.Queues.Models;

namespace Homely.Storage.Queues
{
    /// <summary>
    /// An Azure queue message content.
    /// </summary>
    /// <remarks>The <code>Model</code> is a specific type, provided.</remarks>
    public class AzureMessage : Message
    {
        public AzureMessage(QueueMessage message) : base(message.Body.AsString(),
                                                         message.MessageId,
                                                         message.PopReceipt,
                                                         message.DequeueCount)
        {
        }
    }

    /// <summary>
    /// An Azure queue message content.
    /// </summary>
    /// <remarks>The <code>Model</code> is a specific type, provided.</remarks>
    public class AzureMessage<T> : Message<T>
    {
        public AzureMessage(T model, QueueMessage message) : base(model,
                                                                  message.MessageId,
                                                                  message.PopReceipt,
                                                                  message.DequeueCount)
        {
        }
    }
}
