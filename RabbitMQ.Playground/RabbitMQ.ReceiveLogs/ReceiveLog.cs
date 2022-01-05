using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using static System.Console;

namespace RabbitMQ.ReceiveLogs
{
    internal class ReceiveLog
    {
        public static void Main()
        {
            const string EXCHANGE = "logs";

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: EXCHANGE, type: ExchangeType.Fanout);

            var model = channel.QueueDeclare();
            channel.QueueBind(queue: model.QueueName, exchange: EXCHANGE, routingKey: string.Empty);

            WriteLine(" [*] Waiting for logs.");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                WriteLine($" [x] {message}");
            };
            
            channel.BasicConsume(queue: model.QueueName,
                autoAck: true,
                consumer: consumer);

            WriteLine(" Press [enter] to exit.");
            ReadLine();
        }
    }
}
