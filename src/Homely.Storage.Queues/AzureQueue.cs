using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MoreLinq;
using Newtonsoft.Json;
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
        private Lazy<Task<CloudQueue>> _queue;

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

            _queue = new Lazy<Task<CloudQueue>>(CreateCloudQueue);
        }

        protected Task<CloudQueue> Queue
        {
            get
            {
                return _queue.Value;
            }
            set
            {
                _queue = new Lazy<Task<CloudQueue>>(() => value);
            }
        }

        private async Task<CloudQueue> CreateCloudQueue()
        {
            // TODO: Add POLLY retrying.

            if (!CloudStorageAccount.TryParse(_connectionString, out CloudStorageAccount storageAccount))
            {
                _logger.LogError($"Failed to create an Azure Storage Account for the provided credentials. Check the connection string in the your configuration (appsettings or environment variables, etc).");
                throw new Exception("Failed to create an Azure Storage Account.");
            }

            var cloudQueueClient = storageAccount.CreateCloudQueueClient();
            var cloudQueue = cloudQueueClient.GetQueueReference(Name);

            var created = await cloudQueue.CreateIfNotExistsAsync();
            if (created)
            {
                _logger.LogInformation("  - No Azure Queue [{queueName}] found - so one was auto created.", Name);
            }
            else
            {
                _logger.LogInformation("  - Using existing Azure Queue [{queueName}].", Name);
            }

            return cloudQueue;
        }

        /// <inheritdoc />
        public async Task AddMessageAsync<T>(T item,
                                             TimeSpan? initialVisibilityDelay = null,
                                             CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            CloudQueueMessage message;

            // Don't waste effort serializing a string. It's already in a format that's ready to go.
            if (Helpers.IsASimpleType(typeof(T)))
            {
                message = new CloudQueueMessage(item.ToString());
            }
            else
            {
                // It's a complex type, so serialize this as Json.
                var messageContent = JsonConvert.SerializeObject(item);
                message = new CloudQueueMessage(messageContent);
            }

            var queue = await Queue;

            await queue.AddMessageAsync(message,
                                        null,
                                        initialVisibilityDelay,
                                        null,
                                        null,
                                        cancellationToken);
        }

        /// <inheritdoc />
        public async Task AddMessagesAsync<T>(IEnumerable<T> contents,
                                              TimeSpan? initialVisibilityDelay = null,
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
                var tasks = batch.Select(content => AddMessageAsync(content, initialVisibilityDelay, cancellationToken));

                // Execute this batch.
                await Task.WhenAll(tasks);
            }
        }

        /// <inheritdoc />
        public async Task DeleteMessageAsync(Message message,
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
        public async Task<Message<T>> GetMessageAsync<T>(TimeSpan? visibilityTimeout = null,
                                                         CancellationToken cancellationToken = default)
        {
            var queue = await Queue;

            var message = await queue.GetMessageAsync(visibilityTimeout,
                                                      null,
                                                      null,
                                                      cancellationToken);

            if (message == null)
            {
                return null;
            }

            if (Helpers.IsASimpleType(typeof(T)))
            {
                var value = (T)Convert.ChangeType(message.AsString, typeof(T));
                return new AzureMessage<T>(value, message);
            }

            // Complex type, so lets assume it was serialized as Json ... so now we deserialize it.
            return message.DeserializeMessage<T>();
        }

        /// <inheritdoc  />
        public async Task<Message> GetMessageAsync(TimeSpan? visibilityTimeout = null,
                                                   CancellationToken cancellationToken = default)
        {
            var queue = await Queue;
            var message = await queue.GetMessageAsync(visibilityTimeout,
                                                      null,
                                                      null,
                                                      cancellationToken);
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
            var messages = await queue.GetMessagesAsync(messageCount,
                                                        visibilityTimeout,
                                                        null,
                                                        null,
                                                        cancellationToken);

            return messages?.Select(message => message.DeserializeMessage<T>()) ?? Enumerable.Empty<Message<T>>();
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
            var messages = await queue.GetMessagesAsync(messageCount,
                                                        visibilityTimeout,
                                                        null,
                                                        null,
                                                        cancellationToken);

            return messages?.Select(message => new AzureMessage(message)) ?? Enumerable.Empty<Message>();
        }

        /// <inheritdoc  />
        public async Task<int> GetMessageCountAsync(CancellationToken cancellationToken = default)
        {
            var queue = await Queue;
            await queue.FetchAttributesAsync(cancellationToken);
            return queue.ApproximateMessageCount ?? 0;
        }
    }
}