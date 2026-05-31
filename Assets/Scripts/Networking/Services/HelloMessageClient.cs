using System;
using Mirror;
using TestTask.Networking.Messages;
using TestTask.Networking.Services;
using UnityEngine;
using Zenject;

namespace TestTask.Networking.Demo
{
    public sealed class HelloMessageClient : IInitializable, ITickable, IDisposable
    {
        private readonly INetworkMessageSubscriptionService _subscriptionService;

        private bool _subscribed;

        public HelloMessageClient(INetworkMessageSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void Initialize()
        {
            NetworkClient.RegisterHandler<HelloMessage>(
                OnHelloMessageReceived,
                requireAuthentication: false);

            NetworkClient.OnConnectedEvent += OnClientConnected;

            Debug.Log("[HelloMessageClient] Initialized");
        }

        public void Tick()
        {
            if (_subscribed)
            {
                return;
            }

            if (!NetworkClient.isConnected)
            {
                return;
            }

            SubscribeToHelloMessage();
        }

        public void Dispose()
        {
            NetworkClient.UnregisterHandler<HelloMessage>();
            NetworkClient.OnConnectedEvent -= OnClientConnected;

            Debug.Log("[HelloMessageClient] Disposed");
        }

        private void OnClientConnected()
        {
            Debug.Log("[HelloMessageClient] Connected to server");

            SubscribeToHelloMessage();
        }

        private void SubscribeToHelloMessage()
        {
            if (_subscribed)
            {
                return;
            }

            _subscribed = true;

            Debug.Log("[HelloMessageClient] Subscribing to HelloMessage...");

            _subscriptionService.Subscribe<HelloMessage>();
        }

        private void OnHelloMessageReceived(HelloMessage message)
        {
            Debug.Log($"[HelloMessageClient] Received HelloMessage: {message.Text}");
        }
    }
}