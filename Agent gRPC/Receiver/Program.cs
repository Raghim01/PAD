using Common;
using Grpc.Net.Client;
using GrpcAgent;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Receiver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var host = WebHost.CreateDefaultBuilder(args)
                 .UseStartup<Startup>()
                 .UseUrls(EndPointConstants.SubscribersAddress)
                 .Build();

            host.Start();

            Subscribe(host);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        private static void Subscribe(IWebHost host)
        {
            var channel = GrpcChannel.ForAddress(EndPointConstants.BrokerAddress);
            var client = new Subscriber.SubscriberClient(channel);


            var address = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();
            Console.WriteLine($"Subscriber listening on address: {address}");

            Console.WriteLine("Enter topic:");
            var topic = Console.ReadLine().ToLower();

            var request = new SubscribeRequest() { Topic = topic, Address = address };

            try
            {
                var reply = client.Subscribe(request);
                Console.WriteLine($"Subscribed reply: {reply.IsSuccess}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error subscribe: {ex.Message}");
            }
        }
    } 
}
