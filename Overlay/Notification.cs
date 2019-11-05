using System;

namespace Overlay
{
    [Serializable]
    public class Notification
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Message { get; set; }

        public Notification(string key, object value, string msg)
        {
            Key = key;
            Value = value;
            Message = msg;
        }

        public override string ToString()
        {
            return "This is a notification\n Key:" + Key + "\n Value:" + Value + "\n Message:" + Message + "\n";
        }
    }
}
