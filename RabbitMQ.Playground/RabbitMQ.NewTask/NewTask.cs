using System.Text;
using RabbitMQ.Client;

using static System.Console;

namespace RabbitMQ.NewTask
{
    internal class NewTask
    {
        public static void Main(string[] args)
        {
            const string QUEUE = "task_queue";

            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: QUEUE
                , durable: false
                , exclusive: false
                , autoDelete: false
                , arguments: default);

            var message = GetMessage(args);

            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: string.Empty, routingKey: QUEUE, basicProperties: default, body: body);

            WriteLine($" [x] Send {message}");
            WriteLine("Press ANY Key to exit.");
            ReadLine();
        }

        private static string GetMessage(string[] args) => args.Length > 0 ? string.Join(" ", args) : "<Arguments were null so, Hello World!>";
    }
}
 