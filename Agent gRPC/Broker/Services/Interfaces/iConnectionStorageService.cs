using Broker.Models;

namespace Broker.Services.Interfaces
{
    public interface iConnectionStorageService
    {
        void Add(Connection connection);

        void Remove(string address);

        IList<Connection> GetConnectionsByTopic(string topic);
    }
}
