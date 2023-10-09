using Broker.Models;

namespace Broker.Services.Interfaces
{
    public interface iMessageStorageService
    {
        void Add(Message message);

        Message GetNext();

        bool IsEmpty();
    }
}
