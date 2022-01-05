using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static System.Console;

namespace RabbitMQ.ReceivedLogsTopic
{
    internal class ReceivedLogsTopic
    {
        public static void Main(string[] args)
        {
            const string EXCHANGE = "topic-logs";

            var factory = new ConnectionFactory();

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(EXCHANGE, ExchangeType.Topic);

            var model = channel.QueueDeclare();

            if (args.Length < 1)
            {
                Error.WriteLine($"Usage: {Environment.GetCommandLineArgs().First()} [binding key...]");

                WriteLine(" Press ANY Key to exit.");
                ReadLine();

                Environment.ExitCode = 1;
                return;
            }

            foreach (var bindingKey in args)
            {
                channel.QueueBind(model.QueueName, EXCHANGE, bindingKey);
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

            _ = channel.BasicConsume(model.QueueName, true, consumer);

            WriteLine(" Press ANY Key to exit.");
            ReadLine();
        }
    }
}
