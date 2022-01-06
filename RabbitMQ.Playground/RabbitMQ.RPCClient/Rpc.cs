using static System.Console;

namespace RabbitMQ.RPCClient
{
    internal class Rpc
    {
        public static void Main(string[] args)
        {
            var rpcClient = new RpcClient();

            var number = 30;
            try
            {
                if (args.Length >= 1)
                {
                    if (int.TryParse(args.First(), out var argNumber))
                    {
                        number = argNumber;
                    }
                }
            }
            catch (Exception exception)
            {
                WriteLine(exception.Message);
                throw;
            }

            WriteLine(" [x] Requesting Fibonacci");
            var response = rpcClient.Call(number.ToString());

            WriteLine($" [.] Got '{response}'");
            rpcClient.Close();
        }
    }
}
