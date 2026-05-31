using System;
using System.Collections.Generic;
using Mirror;
using TestTask.Networking.Messages;
using UnityEngine;
using Zenject;

namespace TestTask.Networking.Services
{
    public sealed class MirrorNetworkMessageSubscriptionService :
        INetworkMessageSubscriptionService,
        IInitializable,
        IDisposable
    {
        private readonly Dictionary<string, HashSet<int>> _subscriptionsByMessageType = new();

        public event Action<NetworkConnectionToClient, string> ClientSubscribed;

        public void Initialize()
        {
            NetworkServer.RegisterHandler<SubscribeToMessageRequest>(
                OnSubscribeToMessageRequest,
                requireAuthentication: false);

            NetworkServer.OnDisconnectedEvent += OnServerClientDisconnected;

            Debug.Log("[SubscriptionService] Initialized");
        }

        public void Dispose()
        {
            NetworkServer.UnregisterHandler<SubscribeToMessageRequest>();
            NetworkServer.OnDisconnectedEvent -= OnServerClientDisconnected;

            _subscriptionsByMessageType.Clear();

            Debug.Log("[SubscriptionService] Disposed");
        }

        public void Subscribe<T>() where T : struct, NetworkMessage
        {
            if (!NetworkClient.isConnected)
            {
                Debug.LogWarning(
                    $"[SubscriptionService] Can not subscribe to {typeof(T).Name}: client is not connected.");

                return;
            }

            string messageType = MessageTypeKey.Of<T>();

            NetworkClient.Send(new SubscribeToMessageRequest
            {
                MessageType = messageType,
                Subscribe = true
            });

            Debug.Log($"[SubscriptionService] Client sent subscribe request for {messageType}");
        }

        public void Unsubscribe<T>() where T : struct, NetworkMessage
        {
            if (!NetworkClient.isConnected)
            {
                Debug.LogWarning(
                    $"[SubscriptionService] Can not unsubscribe from {typeof(T).Name}: client is not connected.");

                return;
            }

            string messageType = MessageTypeKey.Of<T>();

            NetworkClient.Send(new SubscribeToMessageRequest
            {
                MessageType = messageType,
                Subscribe = false
            });

            Debug.Log($"[SubscriptionService] Client sent unsubscribe request for {messageType}");
        }

        public bool IsSubscribed<T>(NetworkConnectionToClient connection)
            where T : struct, NetworkMessage
        {
            if (connection == null)
            {
                return false;
            }

            string messageType = MessageTypeKey.Of<T>();

            return _subscriptionsByMessageType.TryGetValue(messageType, out HashSet<int> subscribers)
                   && subscribers.Contains(connection.connectionId);
        }

        public void SendToSubscribed<T>(
            NetworkConnectionToClient connection,
            T message,
            int channelId = Channels.Reliable)
            where T : struct, NetworkMessage
        {
            if (connection == null)
            {
                Debug.LogWarning($"[SubscriptionService] Can not send {typeof(T).Name}: connection is null.");
                return;
            }

            if (!IsSubscribed<T>(connection))
            {
                Debug.LogWarning(
                    $"[SubscriptionService] Client {connection.connectionId} is not subscribed to {typeof(T).Name}. Message was not sent.");

                return;
            }

            connection.Send(message, channelId);

            Debug.Log(
                $"[SubscriptionService] Sent {typeof(T).Name} to subscribed client {connection.connectionId}");
        }

        public void SendToSubscribers<T>(
            T message,
            int channelId = Channels.Reliable)
            where T : struct, NetworkMessage
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning(
                    $"[SubscriptionService] Can not send {typeof(T).Name}: server is not active.");

                return;
            }

            int sentCount = 0;

            foreach (NetworkConnectionToClient connection in NetworkServer.connections.Values)
            {
                if (!IsSubscribed<T>(connection))
                {
                    continue;
                }

                connection.Send(message, channelId);
                sentCount++;
            }

            Debug.Log(
                $"[SubscriptionService] Sent {typeof(T).Name} to {sentCount} subscribed clients.");
        }

        private void OnSubscribeToMessageRequest(
            NetworkConnectionToClient connection,
            SubscribeToMessageRequest request)
        {
            if (connection == null)
            {
                Debug.LogWarning("[SubscriptionService] Subscription request received from null connection.");
                return;
            }

            if (string.IsNullOrWhiteSpace(request.MessageType))
            {
                Debug.LogWarning(
                    $"[SubscriptionService] Client {connection.connectionId} sent empty message type.");

                return;
            }

            if (request.Subscribe)
            {
                AddSubscription(connection, request.MessageType);
            }
            else
            {
                RemoveSubscription(connection, request.MessageType);
            }
        }

        private void AddSubscription(NetworkConnectionToClient connection, string messageType)
        {
            if (!_subscriptionsByMessageType.TryGetValue(messageType, out HashSet<int> subscribers))
            {
                subscribers = new HashSet<int>();
                _subscriptionsByMessageType.Add(messageType, subscribers);
            }

            bool added = subscribers.Add(connection.connectionId);

            if (!added)
            {
                Debug.Log(
                    $"[SubscriptionService] Client {connection.connectionId} was already subscribed to {messageType}");

                return;
            }

            Debug.Log(
                $"[SubscriptionService] Client {connection.connectionId} subscribed to {messageType}");

            ClientSubscribed?.Invoke(connection, messageType);
        }

        private void RemoveSubscription(NetworkConnectionToClient connection, string messageType)
        {
            if (!_subscriptionsByMessageType.TryGetValue(messageType, out HashSet<int> subscribers))
            {
                return;
            }

            subscribers.Remove(connection.connectionId);

            if (subscribers.Count == 0)
            {
                _subscriptionsByMessageType.Remove(messageType);
            }

            Debug.Log(
                $"[SubscriptionService] Client {connection.connectionId} unsubscribed from {messageType}");
        }

        private void OnServerClientDisconnected(NetworkConnectionToClient connection)
        {
            if (connection == null)
            {
                return;
            }

            foreach (HashSet<int> subscribers in _subscriptionsByMessageType.Values)
            {
                subscribers.Remove(connection.connectionId);
            }

            List<string> emptyKeys = new();

            foreach (KeyValuePair<string, HashSet<int>> pair in _subscriptionsByMessageType)
            {
                if (pair.Value.Count == 0)
                {
                    emptyKeys.Add(pair.Key);
                }
            }

            foreach (string key in emptyKeys)
            {
                _subscriptionsByMessageType.Remove(key);
            }

            Debug.Log(
                $"[SubscriptionService] Removed subscriptions for disconnected client {connection.connectionId}");
        }
    }
}