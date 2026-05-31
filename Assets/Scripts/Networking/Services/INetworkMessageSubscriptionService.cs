using System;
using Mirror;

namespace TestTask.Networking.Services
{
    public interface INetworkMessageSubscriptionService
    {
        event Action<NetworkConnectionToClient, string> ClientSubscribed;

        void Subscribe<T>() where T : struct, NetworkMessage;

        void Unsubscribe<T>() where T : struct, NetworkMessage;

        bool IsSubscribed<T>(NetworkConnectionToClient connection)
            where T : struct, NetworkMessage;

        void SendToSubscribed<T>(
            NetworkConnectionToClient connection,
            T message,
            int channelId = Channels.Reliable)
            where T : struct, NetworkMessage;

        void SendToSubscribers<T>(
            T message,
            int channelId = Channels.Reliable)
            where T : struct, NetworkMessage;
    }
}
