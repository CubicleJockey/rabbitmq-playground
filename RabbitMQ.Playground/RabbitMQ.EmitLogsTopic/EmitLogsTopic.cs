using System.Text;
using RabbitMQ.Client;

using static System.Console;

namespace RabbitMQ.EmitLogsTopic
{
    internal class EmitLogsTopic
    {
        public static void Main(string[] args)
        {
            const string EXCHANGE = "topic-logs";

            var factory = new ConnectionFactory { HostName = "localhost" };
            
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(EXCHANGE, ExchangeType.Topic);

            var routingKey = args.Length > 0 ? args.First() : "anonymous.info";
            var message = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "<args are empty.>";

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(EXCHANGE, routingKey, default, body);

            WriteLine($" [x] Sent '{routingKey}':'{message}'");
        }
    }
}
