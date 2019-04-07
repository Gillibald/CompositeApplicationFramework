#region Usings

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace CompositeApplicationFramework.Utility
{
    public class Serializer
    {
        public static readonly Serializer Instance = new Serializer();

        private Serializer()
        {
        }

        #region ToBinary

        public void SerializeObject<T>(string filename, T objectToSerialize)
        {
            var path = Path.GetDirectoryName(filename);

            if (path != null && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Stream stream = File.Open(filename, FileMode.Create);
            var bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        public byte[] SerializeObject<T>(T objectToSerialize)
        {
            var stream = new MemoryStream();
            var bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            var memoryStream = new MemoryStream();
            stream.Position = 0;
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public T DeSerializeObject<T>(byte[] data)
        {
            var stream = new MemoryStream(data);
            var bFormatter = new BinaryFormatter();
            var objectToSerialize = (T) bFormatter.Deserialize(stream);
            stream.Close();
            return objectToSerialize;
        }

        public T DeSerializeObject<T>(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Open);
            var bFormatter = new BinaryFormatter();
            var objectToSerialize = (T) bFormatter.Deserialize(stream);
            stream.Close();
            return objectToSerialize;
        }

        #endregion
    }
}