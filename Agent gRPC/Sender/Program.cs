using Common;
using Grpc.Net.Client;
using GrpcAgent;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Publisher");

            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(EndPointConstants.BrokerAddress);
            var client = new Publisher.PublisherClient(channel);

            while (true)
            {
                Console.WriteLine("Enter the topic:");
                var topic = Console.ReadLine();

                Console.WriteLine("Enter the content");
                var content = Console.ReadLine();

                var request = new PublishRequest() { Topic = topic,  Content = content};

                try 
                {
                    var reply = await client.PyblishMessageAsync(request);
                    Console.WriteLine($"Publish reply: {reply.IsSuccess}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error publishing the message: {ex.Message}");
                }
            }
        }
    }
}