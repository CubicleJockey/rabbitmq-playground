using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using static System.Console;

namespace RabbitMQ.BasicReceive
{
    public class Receive
    {
        public static void Main()
        {
            const string QUEUE = "hello";

            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: QUEUE, durable: false, exclusive: false, autoDelete: false, arguments: default);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                WriteLine($" [x] Received '{message}'");
            };

            channel.BasicConsume(queue: QUEUE, autoAck: true, consumer: consumer);

            WriteLine($" Press [any key] to exit.");
            ReadLine();
        }
    }
}
