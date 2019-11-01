using System;

namespace Cache_Client
{
    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
    }
}
