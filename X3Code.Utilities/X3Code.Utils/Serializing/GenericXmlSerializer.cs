using System.IO;
using System.Xml.Serialization;

namespace X3Code.Utils.Serializing;

/// <summary>
/// Generic encapsulation of .Net XML serializer
/// </summary>
public static class GenericXmlSerializer
{
    /// <summary>
    /// Generic XML-Loader. Loads and serialize any xml-file if the type is known.
    /// </summary>
    /// <typeparam name="T">Type of the serialized object (XML-File)</typeparam>
    /// <param name="file">The full filepath to the xml file</param>
    /// <returns></returns>
    public static T LoadXml<T>(string file) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));
        using (Stream stream = File.OpenRead(file))
        {
            return (T)serializer.Deserialize(stream);
        }
    }

    /// <summary>
    /// Serializes every type into a xml file. The file will be overwritten if existing.
    /// If the file extension is missing, it will not be added.
    /// </summary>
    /// <typeparam name="T">type of the object to save</typeparam>
    /// <param name="file">Full file path with extension(!) where the xml file should be created.</param>
    /// <param name="dataToSave">The object to serialize into a xml file</param>
    public static void SaveXml<T>(string file, T dataToSave)
    {
        var serializer = new XmlSerializer(typeof(T));
        using (Stream stream = File.Create(file))
        {
            serializer.Serialize(stream, dataToSave);
        }
    }

    /// <summary>
    /// Serializes every type into a xml string.
    /// </summary>
    /// <typeparam name="T">The object to serialize into a xml string.</typeparam>
    /// <param name="toSerialize">The serialized xml string.</param>
    /// <returns></returns>
    public static string SerializeObjectToXmlString<T>(T toSerialize)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));

        using (var textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
    }

    /// <summary>
    /// Deserializes a xml string back to an object
    /// </summary>
    /// <typeparam name="T">The object type, the string should be deserialized</typeparam>
    /// <param name="xmlString">The XML string to deserialize</param>
    /// <returns></returns>
    public static T DeserializeXmlStringTo<T>(string xmlString) where T : class
    {
        var xmlSerializer = new XmlSerializer(typeof(T));

        using (StringReader reader = new StringReader(xmlString))
        {
            return (T)xmlSerializer.Deserialize(reader);
        }
    }

    /// <summary>
    /// Serializes an object to InMemory-XML
    /// </summary>
    /// <typeparam name="T">The type to serialize</typeparam>
    /// <param name="toSerialize">The object to serialize</param>
    /// <returns>The serialized xml memory stream</returns>
    public static byte[] SerializeToXmlInMemory<T>(T toSerialize) where T : class
    {
        var xmlSerializer = new XmlSerializer(typeof(T));

        using (var stream = new MemoryStream())
        {
            xmlSerializer.Serialize(stream, toSerialize);
            return stream.ToArray();
        }
    }

    /// <summary>
    /// Deserializes an inMemory-XML-Stream back into the object
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <param name="toDeserialize">The inMemory-stream to deserialize as byte[]</param>
    /// <returns>The deserialized xml object</returns>
    public static T DeserializeXmlMemoryStreamTo<T>(byte[] toDeserialize) where T : class
    {
        var xmlSerializer = new XmlSerializer(typeof(T));

        using (MemoryStream stream = new MemoryStream(toDeserialize))
        {
            return (T)xmlSerializer.Deserialize(stream);
        }
    }
}