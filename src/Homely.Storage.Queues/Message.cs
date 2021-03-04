using System;

namespace Homely.Storage.Queues
{
    /// <summary>
    /// Queue message content.
    /// </summary>
    /// <remarks>The <code>Model</code> is a string.</remarks>
    public class Message : Message<string>
    {
        public Message(string content,
                       string id,
                       string receipt,
                       long dequeueCount) : base(content,
                                                 id,
                                                 receipt,
                                                 dequeueCount)
        { }
    }

    /// <summary>
    /// Queue message content.
    /// </summary>
    /// <remarks>The <code>Model</code> is a specific type, provided.</remarks>
    public class Message<T>
    {
        public Message(T model,
                       string id,
                       string receipt,
                       long dequeueCount)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("message", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(receipt))
            {
                throw new ArgumentException("message", nameof(receipt));
            }

            if (dequeueCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dequeueCount));
            }

            Model = model;
            Id = id;
            Receipt = receipt;
            DequeueCount = dequeueCount;
        }
        public string Id { get; }
        public string Receipt { get; }
        public long DequeueCount { get; }
        public T Model { get; }
    }
}
