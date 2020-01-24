using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace X3Code.Utils.Serializing
{
    /// <summary>
    /// Generuic Capsulation of the .Net In Memory serializer.
    /// </summary>
    public static class GenericMemorySerializer
    {
        #region Serializer

        /// <summary>
        /// Serializes any Object[] into a binary MemoryStream
        /// </summary>
        /// <param name="toSerialize">The objects to serialize</param>
        /// <returns></returns>
        public static byte[] Serialize(object[] toSerialize)
        {
            var formatter = new BinaryFormatter();
            using (var memory = new MemoryStream())
            {
                formatter.Serialize(memory, toSerialize);
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Deserializes a binary MemoryStream into an object[]
        /// </summary>
        /// <param name="toDeserialize">The stream to deserialize</param>
        /// <returns></returns>
        public static object[] Deserialize(byte[] toDeserialize)
        {
            var formatter = new BinaryFormatter();
            using (var memory = new MemoryStream(toDeserialize))
            {
                return formatter.Deserialize(memory) as object[];
            }
        }

        #endregion

        #region Extension

        /// <summary>
        /// Serializes any object to an memory byte stream
        /// </summary>
        /// <param name="toConvert">The object to convert</param>
        /// <returns>Serialized object as binary memory stream</returns>
        public static byte[] ToBinaryMemoryStream(this object[] toConvert)
        {
            return Serialize(toConvert);
        }


        /// <summary>
        /// Deserializes a byte memory stream into an object[]
        /// </summary>
        /// <param name="toConvert">The stream to deserialize</param>
        /// <returns>Deserialized memory stream as object[]</returns>
        public static object[] ToObjectArray(this byte[] toConvert)
        {
            return Deserialize(toConvert);
        }

        #endregion
    }
}
