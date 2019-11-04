﻿namespace Overlay
{
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
    }
}
