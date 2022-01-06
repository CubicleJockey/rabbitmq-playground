using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

namespace RabbitMQ.RPCClient
{
    internal class RpcClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> responseQueue = new();
        private readonly IBasicProperties properties;

        public RpcClient()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            var queue = channel.QueueDeclare();
            replyQueueName = queue.QueueName;

            consumer = new EventingBasicConsumer(channel);
            properties = channel.CreateBasicProperties();

            var correlationId = Guid.NewGuid().ToString();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    responseQueue.Add(response);
                }
            };

            channel.BasicConsume(consumer, replyQueueName, true);
        }

        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(string.Empty, "rpc-queue", properties, messageBytes);
            return responseQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
