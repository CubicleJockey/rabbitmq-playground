using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using static System.Console;

namespace RabbitMQ.RPCServer
{
    internal class RPCServer
    {
        public static void Main()
        {
            const string QUEUE = "rpc-queue";

            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(QUEUE, false, autoDelete: false, arguments: default);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(QUEUE, false, consumer);

            WriteLine(" [x] Awaiting RPC requests");

            consumer.Received += (model, ea) =>
            {
                string response = null!;

                var body = ea.Body.ToArray();
                var properties = ea.BasicProperties;
                var replyProperties = channel.CreateBasicProperties();
                replyProperties.CorrelationId = properties.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    var number = int.Parse(message);
                    WriteLine($" [.] Fibonacci({message})");
                    response = Fibonacci(number).ToString();
                }
                catch (Exception exception)
                {
                    WriteLine($" [.] {exception.Message}");
                    response = string.Empty;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(string.Empty, properties.ReplyTo, replyProperties, responseBytes);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            WriteLine(" Press ANY Key to exit.");
            ReadLine();
        }

        private static int Fibonacci(int n)
        {
            if (n is < 0 or 0 or 1) { return n; }
            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }
    }
}
