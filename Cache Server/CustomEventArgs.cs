using System;

namespace Cache_Server
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
