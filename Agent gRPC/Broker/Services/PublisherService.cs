using System;
using GrpcAgent;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Broker.Models;
using Broker.Services.Interfaces;

namespace Broker.Services
{
    public class PublisherService : Publisher.PublisherBase
    {
        private readonly iMessageStorageService _messageStorageService;

        public PublisherService(iMessageStorageService messageStorageService) 
        {
            _messageStorageService = messageStorageService;
        }

        public override Task<PublishReply> PyblishMessage(PublishRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Reveived: {request.Topic} {request.CalculateSize}");

            var message = new Message(request.Topic, request.Content);
            _messageStorageService.Add(message);

            return Task.FromResult(new PublishReply()
            {
                IsSuccess = true
            });
        }
    }
}
