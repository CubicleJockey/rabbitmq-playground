using System.Text;
using RabbitMQ.Client;

using static System.Console;

namespace RabbitMQ.EmitLogs
{
    internal class EmitLogs
    {
        public static void Main(string[] args)
        {
            const string EXCHANGE = "logs";

            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: EXCHANGE, type: ExchangeType.Fanout);


            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);


            channel.BasicPublish(exchange: EXCHANGE, routingKey: string.Empty, basicProperties: default, body: body);

            WriteLine($" [x] Send {message}");

            WriteLine(" Press ANY Key to exit.");
            ReadLine();

        }

        private static string GetMessage(string[] args) => 
            args.Length > 0 ? string.Join(" ", args) : "info: <args was empty>";
    }
}
