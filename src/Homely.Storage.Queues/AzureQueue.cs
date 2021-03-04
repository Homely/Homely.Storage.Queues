using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Homely.Storage.Queues
{
    public class AzureQueue : IQueue
    {
        private readonly ILogger<AzureQueue> _logger;
        private readonly string _connectionString;
        private Lazy<Task<QueueClient>> _queue;

        public string Name { get; }

        public AzureQueue(string connectionString,
                          string queueName,
                          ILogger<AzureQueue> logger)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException(nameof(queueName));
            }

            _connectionString = connectionString;
            Name = queueName;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _queue = new Lazy<Task<QueueClient>>(CreateCloudQueue);
        }

        protected Task<QueueClient> Queue
        {
            get
            {
                return _queue.Value;
            }
            set
            {
                _queue = new Lazy<Task<QueueClient>>(() => value);
            }
        }

        private async Task<QueueClient> CreateCloudQueue()
        {
            var queue = new QueueClient(_connectionString, Name);
            var createIfNotExistsResponse = await queue.CreateIfNotExistsAsync();
            if (createIfNotExistsResponse != null)
            {
                _logger.LogInformation("  - No Azure Queue [{queueName}] found - so one was auto created.", Name);
            }
            else
            {
                _logger.LogInformation("  - Using existing Azure Queue [{queueName}].", Name);
            }

            return queue;
        }

        /// <inheritdoc />
        public async Task AddMessageAsync<T>(T item,
                                             TimeSpan? visibilityTimeout = null,
                                             TimeSpan? timeToLive = null,
                                             CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var queue = await Queue;

            if (Helpers.IsASimpleType(typeof(T))) // Don't waste effort serializing a string. It's already in a format that's ready to go.
            {
                await queue.SendMessageAsync(item.ToString(),
                                             visibilityTimeout,
                                             timeToLive,
                                             cancellationToken);
            }
            else // It's a complex type, so serialize this as Json.
            {
                await queue.SendMessageAsync(BinaryData.FromObjectAsJson(item),
                                             visibilityTimeout,
                                             timeToLive,
                                             cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task AddMessagesAsync<T>(IEnumerable<T> contents,
                                              TimeSpan? initialVisibilityDelay = null,
                                              TimeSpan? timeToLive = null,
                                              int batchSize = 25,
                                              CancellationToken cancellationToken = default)
        {
            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            // Lets batch up these messages to make sure the awaiting of all the tasks doesn't go too crazy.
            var contentsSize = contents.Count();
            var finalBatchSize = contentsSize > batchSize
                                     ? batchSize
                                     : contentsSize;

            foreach (var batch in contents.Batch(finalBatchSize))
            {
                var tasks = batch.Select(content => AddMessageAsync(content,
                                                                    initialVisibilityDelay,
                                                                    timeToLive,
                                                                    cancellationToken));

                // Execute this batch.
                await Task.WhenAll(tasks);
            }
        }

        /// <inheritdoc />
        public async Task DeleteMessageAsync<T>(Message<T> message,
                                                CancellationToken cancellationToken = default)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrWhiteSpace(message.Id))
            {
                throw new ArgumentException(nameof(message.Id));
            }

            if (string.IsNullOrWhiteSpace(message.Receipt))
            {
                throw new ArgumentException(nameof(message.Receipt));
            }

            var queue = await Queue;

            await queue.DeleteMessageAsync(message.Id,
                                           message.Receipt,
                                           cancellationToken);
        }

        /// <inheritdoc  />
        public async Task<Message<T>> GetMessageAsync<T>(TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default)
        {
            var message = await ReceiveMessageAsync(visibilityTimeout, cancellationToken);

            if (Helpers.IsASimpleType(typeof(T)))
            {
                var value = (T)Convert.ChangeType(message.Body, typeof(T));
                return new AzureMessage<T>(value, message);
            }

            // Complex type, so lets assume it was serialized as Json ... so now we deserialize it.
            return message.DeserializeMessage<T>();
        }

        /// <inheritdoc  />
        public async Task<Message> GetMessageAsync(TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default)
        {
            var message = await ReceiveMessageAsync(visibilityTimeout, cancellationToken);

            return message == null
                ? null
                : new AzureMessage(message);
        }

        /// <inheritdoc  />
        public async Task<IEnumerable<Message<T>>> GetMessagesAsync<T>(int messageCount,
                                                                       TimeSpan? visibilityTimeout = null,
                                                                       CancellationToken cancellationToken = default)
        {
            if (messageCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(messageCount));
            }

            var queue = await Queue;
            var messages = await ReceiveMessagesAsync(messageCount,
                                                      visibilityTimeout,
                                                      cancellationToken);

            return messages.Select(message => message.DeserializeMessage<T>());
        }

        /// <inheritdoc  />
        public async Task<IEnumerable<Message>> GetMessagesAsync(int messageCount,
                                                                 TimeSpan? visibilityTimeout = null,
                                                                 CancellationToken cancellationToken = default)
        {
            if (messageCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(messageCount));
            }

            var queue = await Queue;
            var messages = await ReceiveMessagesAsync(messageCount,
                                                      visibilityTimeout,
                                                      cancellationToken);

            return messages.Select(message => new AzureMessage(message));
        }

        private async Task<QueueMessage> ReceiveMessageAsync(TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default)
        {
            var messages = await ReceiveMessagesAsync(1,
                                                      visibilityTimeout,
                                                      cancellationToken);
            return messages.FirstOrDefault();
        }

        private async Task<QueueMessage[]> ReceiveMessagesAsync(int messageCount,
                                                                TimeSpan? visibilityTimeout = null,
                                                                CancellationToken cancellationToken = default)
        {
            var queue = await Queue;
            var receiveMessagesResponse = await queue.ReceiveMessagesAsync(messageCount,
                                                                           visibilityTimeout,
                                                                           cancellationToken);
            return receiveMessagesResponse == null ||
                   receiveMessagesResponse.Value == null
                ? Array.Empty<QueueMessage>()
                : receiveMessagesResponse.Value;
        }

        /// <inheritdoc  />
        public async Task<int> GetMessageCountAsync(CancellationToken cancellationToken = default)
        {
            var queue = await Queue;
            var queueProperties = await queue.GetPropertiesAsync(cancellationToken);
            return queueProperties == null ||
                   queueProperties.Value == null
                ? 0
                : queueProperties.Value.ApproximateMessagesCount;
        }
    }
}
