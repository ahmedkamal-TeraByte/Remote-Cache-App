using Overlay;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cache_Client
{
    class Serializer
    {

        public Serializer(EventHandler<CustomEventArgs> handler)
        {
            RaiseEvent += handler;
        }

        #region eventhandler
        public event EventHandler<CustomEventArgs> RaiseEvent;


        protected virtual void OnRaiseEvent(CustomEventArgs args)
        {
            if (RaiseEvent != null)
                RaiseEvent(this, args);
        }


        #endregion

        public byte[] Serialize(DataObject data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            try
            {
                formatter.Serialize(stream, data);
                byte[] bytes = stream.ToArray();
                stream.Dispose();
                return bytes;
            }
            catch (SerializationException)
            {
                stream.Dispose();
                OnRaiseEvent(new CustomEventArgs("Cannot Serialize Object"));
                return null;
            }

        }

        public DataObject DeSerialize(byte[] bytes)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            DataObject data = (DataObject)formatter.Deserialize(new MemoryStream(bytes));
            return data;
        }
    }
}
