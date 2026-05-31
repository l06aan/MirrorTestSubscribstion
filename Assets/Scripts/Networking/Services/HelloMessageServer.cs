using System;
using Mirror;
using TestTask.Networking.Messages;
using TestTask.Networking.Services;
using UnityEngine;
using Zenject;

namespace TestTask.Networking.Demo
{
    public sealed class HelloMessageServer : IInitializable, IDisposable
    {
        private readonly INetworkMessageSubscriptionService _subscriptionService;

        public HelloMessageServer(INetworkMessageSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void Initialize()
        {
            _subscriptionService.ClientSubscribed += OnClientSubscribed;

            Debug.Log("[HelloMessageServer] Initialized");
        }

        public void Dispose()
        {
            _subscriptionService.ClientSubscribed -= OnClientSubscribed;

            Debug.Log("[HelloMessageServer] Disposed");
        }

        private void OnClientSubscribed(NetworkConnectionToClient connection, string messageType)
        {
            string helloMessageType = MessageTypeKey.Of<HelloMessage>();

            if (messageType != helloMessageType)
            {
                return;
            }

            Debug.Log(
                $"[HelloMessageServer] Client {connection.connectionId} subscribed to HelloMessage. Sending greeting...");

            _subscriptionService.SendToSubscribed(
                connection,
                new HelloMessage
                {
                    Text = "Hello Client!"
                });
        }
    }
}