using Mirror;

namespace TestTask.Networking.Messages
{
    public struct SubscribeToMessageRequest : NetworkMessage
    {
        public string MessageType;
        public bool Subscribe;
    }
}