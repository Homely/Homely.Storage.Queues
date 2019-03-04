using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Homely.Storage.Queues
{
    public interface IQueue
    {
        /// <summary>
        /// Name of the Queue.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Initiates an asynchronous operation to add an item to the queue.
        /// </summary>
        /// <typeparam name="T">Type of item.</typeparam>
        /// <param name="item">An item to add to the queue.</param>
        /// <param name="initialVisibilityDelay">How long to initially hide the message.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>A System.Threading.Tasks.Task object that represents the asynchronous operation.</returns>
        /// <remarks>If the item is a IsPrimitive (int, etc) or a string then it's stored -as is-. Otherwise, it is serialized to Json and then stored as Json.(</remarks>
        Task AddMessageAsync<T>(T item, 
                                TimeSpan? initialVisibilityDelay = null, 
                                CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiates an asynchronous operation to add a batch messages to the queue.
        /// </summary>
        /// <typeparam name="T">Type of item.</typeparam>
        /// <param name="contents">Collection of content to add to the queue.</param>
        /// <param name="initialVisibilityDelay">How long to initially hide the message.</param>
        /// <param name="batchSize">Number of messages per batch, to store as one parallel execution.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>A System.Threading.Tasks.Task object that represents the asynchronous operation.</returns>
        /// <remarks>If any item is a IsPrimitive (int, etc) or a string then it's stored -as is-. Otherwise, it is serialized to Json and then stored as Json.(</remarks>
        Task AddMessagesAsync<T>(IEnumerable<T> contents, 
                                 TimeSpan? initialVisibilityDelay = null, 
                                 int batchSize = 25, 
                                 CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiates an asynchronous operation to delete a message.
        /// </summary>
        /// <param name="messageId">A string specifying the message ID.</param>
        /// <param name="receipt">A string specifying the pop receipt value.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>A System.Threading.Tasks.Task object that represents the asynchronous operation.</returns>
        Task DeleteMessageAsync(string messageId, 
                                string receipt, 
                                CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a message from a queue and wraps it in a simple Message class.
        /// </summary>
        /// <typeparam name="T">Type of item.</typeparam>
        /// <param name="visibilityTimeout">A System.TimeSpan specifying the visibility timeout interval.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>A System.Threading.Tasks.Task object that represents the asynchronous operation.</returns>
        /// <remarks>The content of the message will attempt to be deserialized from Json. If the message is a Primitive type or a string, then the Json deserialization will be still run but no error should occur.</remarks>
        Task<Message> GetMessageAsync(TimeSpan? visibilityTimeout = null, 
                                      CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a message from a queue and wraps it in a simple Message class.
        /// </summary>
        /// <param name="visibilityTimeout">A System.TimeSpan specifying the visibility timeout interval.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>A System.Threading.Tasks.Task object that represents the asynchronous operation.</returns>
        /// <remarks>The content of the message will attempt to be deserialized from Json. If the message is a Primitive type or a string, then the Json deserialization will be still run but no error should occur.</remarks>
        Task<Message<T>> GetMessageAsync<T>(TimeSpan? visibilityTimeout = null,
                                            CancellationToken cancellationToken = default);

        /// <summary>
        /// An estimated number of messages in the queue.
        /// </summary>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>A System.Threading.Tasks.Task object of type integer that represents the asynchronous operation.</returns>
        Task<int> GetMessageCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a collection of messages from a queue and wraps each one in a simple Message class.
        /// </summary>
        /// <typeparam name="T">Type of item.</typeparam>
        /// <param name="messageCount">The number of messages to retrieve from the queue.</param>
        /// <param name="visibilityTimeout">A System.TimeSpan specifying the visibility timeout interval.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>A System.Threading.Tasks.Task object that represents the asynchronous operation.</returns>
        /// <remarks>The content of the message will attempt to be deserialized from Json. If the message is a Primitive type or a string, then the Json deserialization will be still run but no error should occur.</remarks>
        Task<IEnumerable<Message>> GetMessagesAsync(int messageCount, 
                                                    TimeSpan? visibilityTimeout = null, 
                                                    CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a collection of messages from a queue and wraps each one in a simple Message class.
        /// </summary>
        /// <param name="messageCount">The number of messages to retrieve from the queue.</param>
        /// <param name="visibilityTimeout">A System.TimeSpan specifying the visibility timeout interval.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>A System.Threading.Tasks.Task object that represents the asynchronous operation.</returns>
        /// <remarks>The content of the message will attempt to be deserialized from Json. If the message is a Primitive type or a string, then the Json deserialization will be still run but no error should occur.</remarks>
        Task<IEnumerable<Message<T>>> GetMessagesAsync<T>(int messageCount, 
                                                          TimeSpan? visibilityTimeout = null, 
                                                          CancellationToken cancellationToken = default);
    }
}