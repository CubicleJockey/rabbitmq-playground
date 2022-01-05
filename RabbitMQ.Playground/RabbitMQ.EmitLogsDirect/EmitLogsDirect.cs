using System.Text;
using RabbitMQ.Client;

using static System.Console;

namespace RabbitMQ.EmitLogsDirect
{
    internal class EmitLogsDirect
    {
        public static void Main(string[] args)
        {
            const string EXCHANGE = "direct-logs";

            var factory = new ConnectionFactory {  HostName = "localhost" };
            
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(EXCHANGE, ExchangeType.Direct);

            var severity = args.Length > 0 ? args.First() : "info";
            var message = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "<args is empty>";

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(EXCHANGE, severity,  default, body);

            WriteLine($" [x] Sent '{severity}':'{message}'");

            WriteLine(" Press ANY Key to exit.");
            ReadLine();
        }
    }
}
