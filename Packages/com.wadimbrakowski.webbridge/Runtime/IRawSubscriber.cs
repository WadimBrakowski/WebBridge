namespace WebBridge
{
    public interface IRawSubscriber
    {
        void OnMessageReceived(string topic, string payloadJson);
    }
}