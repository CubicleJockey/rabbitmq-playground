using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using static System.Console;

namespace RabbitMQ.ReceiveLogsDirect
{
    internal class ReceiveLogsDirect
    {
        public static void Main(string[] args)
        {
            const string EXCHANGE = "direct-logs";

            var factory = new ConnectionFactory();

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(EXCHANGE, ExchangeType.Direct);

            var model = channel.QueueDeclare();

            if (args.Length < 1)
            {
                Error.WriteLine($"Usage: {Environment.GetCommandLineArgs().First()} [info] [warning] [error]");
                WriteLine(" Press ANY Key to exit.");
                ReadLine();

                Environment.ExitCode = 1;
                return;
            }

            foreach (var severity in args)
            {
                channel.QueueBind(model.QueueName, EXCHANGE, severity);
            }

            WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                WriteLine($" [x] Received '{routingKey}':'{message}'");
            };

            channel.BasicConsume(model.QueueName, true, consumer);

            WriteLine(" Press ANY Key to exit.");
            ReadLine();
        }
    }
}
