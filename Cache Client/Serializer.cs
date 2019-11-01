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

        public MemoryStream Serialize(DataObject data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            try
            {
                formatter.Serialize(stream, data);
                return stream;
            }
            catch (SerializationException)
            {
                OnRaiseEvent(new CustomEventArgs("Cannot Serialize Object"));
                return null;
            }

        }

        public DataObject DeSerialize(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            DataObject data = (DataObject)formatter.Deserialize(stream);
            return data;
        }
    }
}
