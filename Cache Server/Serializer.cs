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
                System.Console.WriteLine("Cannot serialize on server");
                throw;
            }
            //finally
            //{
            //    stream.Dispose();
            //}

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
                //OnRaiseEvent(new CustomEventArgs("Cannot desearlize stream at server " + e.Message));
                throw;
            }
        }

    }
}
