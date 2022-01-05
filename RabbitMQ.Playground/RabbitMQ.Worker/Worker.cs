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

            //Even out the work load between multiple workers.
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                WriteLine($" [x] Received '{message}'");

                var dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                WriteLine(" [x] Done.");

                /*
                 *Manual message acknowledgments are turned on by default.
                 * In previous examples we explicitly turned them off by setting the autoAck ("automatic acknowledgement mode") parameter to true.
                 * It's time to remove this flag and manually send a proper acknowledgment from the worker, once we're done with a task.
                 */

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: QUEUE, autoAck: true, consumer: consumer);

            WriteLine(" Press [any key] to exit.");
            ReadLine();
        }
    }
}
