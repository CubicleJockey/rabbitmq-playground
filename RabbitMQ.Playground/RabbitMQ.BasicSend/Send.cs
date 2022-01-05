using System.Text;
using RabbitMQ.Client;

using static System.Console;

namespace RabbitMQ.BasicSend
{
    internal class Send
    {
        public static void Main()
        {
            const string QUEUE = "hello";

            var factory = new ConnectionFactory { HostName = "localhost" };
            
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: QUEUE
                                ,durable: false
                                ,exclusive: false
                                ,autoDelete: false
                                ,arguments: default);

            const string message = "Hello, World!";

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty, routingKey: QUEUE, basicProperties: default, body: body);

            WriteLine($" [x] Send {message}");

            WriteLine(" Press [any key] to exit.");
            ReadLine();
        }
    }
}
