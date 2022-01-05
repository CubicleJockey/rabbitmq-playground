using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

using static System.Console;

namespace RabbitMQ.Worker
{
    internal class Worker
    {
        public static void Main()
        {
            const string QUEUE = "task_queue";

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

                var dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                WriteLine(" [x] Done.");

            };

            channel.BasicConsume(queue: QUEUE, autoAck: true, consumer: consumer);

            WriteLine(" Press [any key] to exit.");
            ReadLine();
        }
    }
}
