using Overlay;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cache_Server
{
    class Serializer
    {
        public Serializer(EventHandler<CustomEventArgs> handler)
        {
            RaiseEvent += handler;
        }


        #region event handler
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
                OnRaiseEvent(new CustomEventArgs("Cannot serialize on server"));
                throw;
            }
        }

        public DataObject DeSerialize(MemoryStream stream)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                DataObject data = (DataObject)formatter.Deserialize(stream);
                return data;
            }
            catch (SerializationException)
            {
                OnRaiseEvent(new CustomEventArgs("Cannot deserialize on server"));

                throw;
            }
        }

    }
}
