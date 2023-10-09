using Broker.Services.Interfaces;
using Grpc.Core;
using GrpcAgent;

namespace Broker.Services
{
    public class SenderWorker : IHostedService
    {

        private Timer _timer;
        private const int TimeToWait = 2000;
        private readonly iMessageStorageService _messageStorage;
        private readonly iConnectionStorageService _connectionStorage;

        public SenderWorker(IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                _messageStorage =  scope.ServiceProvider.GetRequiredService<iMessageStorageService>();
                _connectionStorage =  scope.ServiceProvider.GetRequiredService<iConnectionStorageService>();

            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoSendWork, null, 0, TimeToWait);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void DoSendWork(object state)
        {
            while (!_messageStorage.IsEmpty())
            {
                var message = _messageStorage.GetNext();
                if(message != null)
                {
                    var connections = _connectionStorage.GetConnectionsByTopic(message.Topic);

                    foreach( var connection in connections)
                    {
                        var client = new Notifier.NotifierClient(connection.Channel);
                        var request = new NotifyRequest() { Content = message.Content };

                        try
                        {
                            var reply = client.Notify(request);
                            Console.WriteLine($"Notified Subscriber: {connection.Address} with {message.Content}. Response: {reply.IsSuccess}");
                        }
                        catch(RpcException rpcException)
                        {
                            if(rpcException.StatusCode == StatusCode.Internal)
                            {
                                _connectionStorage.Remove(connection.Address);
                            }
                            Console.WriteLine($"RPC Error notifying subscriber {connection.Address}. {rpcException.Message}");
                        }
                        catch(Exception ex) 
                        {
                            Console.WriteLine($"Error notifying subscriber:{connection.Address}. {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}
