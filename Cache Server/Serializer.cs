using Overlay;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cache_Server
{
    class Serializer
    {

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
                //OnRaiseEvent(new CustomEventArgs("Cannot Serialize Object on server"));
                return null;
            }
            finally
            {
                stream.Dispose();
            }

        }

        public DataObject DeSerialize(Stream stream)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                DataObject data = (DataObject)formatter.Deserialize(stream);
                return data;
            }
            catch (SerializationException e)
            {
                //OnRaiseEvent(new CustomEventArgs("Cannot desearlize stream at server " + e.Message));
                throw;
            }
        }

    }
}
