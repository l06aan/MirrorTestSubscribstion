using Mirror;

namespace TestTask.Networking.Messages
{
    public static class MessageTypeKey
    {
        public static string Of<T>() where T : struct, NetworkMessage
        {
            return typeof(T).FullName;
        }
    }
}