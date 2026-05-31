using Mirror;

namespace TestTask.Networking.Messages
{
    public struct HelloMessage : NetworkMessage
    {
        public string Text;
    }
}