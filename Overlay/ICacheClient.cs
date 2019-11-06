namespace Overlay
{
    public interface ICacheClient : ICache
    {

        void Subscribe(string eventName);
        void Unsubscribe(string eventName);
        void GetSubscriptions();
    }
}
