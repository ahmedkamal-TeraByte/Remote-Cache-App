using System;

namespace Overlay
{
    public class DataObjectEventArgs : EventArgs
    {
        public DataObject dataObject { get; set; }

        public DataObjectEventArgs(DataObject data)
        {
            dataObject = data;
        }
    }
}
