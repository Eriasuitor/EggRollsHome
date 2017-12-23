using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft;
using Newtonsoft.Json;

namespace Newegg.MIS.API.Utilities.Helpers
{
    public static class SerializationHelper
    {
        private static readonly object Mutex = new object();
        private static readonly Dictionary<Type, XmlSerializer> XmlSerializerPool;
        private static readonly XmlSerializerNamespaces EmptyNamespaces =
            new XmlSerializerNamespaces();

        static SerializationHelper()
        {
            XmlSerializerPool = new Dictionary<Type, XmlSerializer>();
            EmptyNamespaces.Add(string.Empty, string.Empty);
        }

        public static string Serialize<T>(T serialObject) where T : class
        {
            var ser = GetSingleInstanceSerializer(typeof(T), null);


            using (var mem = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(mem,
                    new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    ser.Serialize(writer, serialObject, EmptyNamespaces);
                    writer.Close();
                }

                return Encoding.Unicode.GetString(mem.ToArray());
            }
        }

        public static string Serialize(object instance, Type[] extraTypes)
        {
            if (null == instance) return string.Empty;
            var xmlSerializer = GetSingleInstanceSerializer(instance.GetType(), extraTypes);

            string xmlContent;
            using (Stream xmlStream = new MemoryStream())
            using (var writer = XmlWriter.Create(
                xmlStream,
                new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                xmlSerializer.Serialize(writer, instance, EmptyNamespaces);
                writer.Flush();
                xmlStream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(xmlStream);
                xmlContent = reader.ReadToEnd();
            }

            return xmlContent;
        }

        private static XmlSerializer GetSingleInstanceSerializer(Type type, Type[] extraTypes)
        {
            if (XmlSerializerPool.ContainsKey(type)) return XmlSerializerPool[type];

            lock (Mutex)
            {
                if (!XmlSerializerPool.ContainsKey(type))
                {
                    var xmlSerializer = new XmlSerializer(type, extraTypes);
                    XmlSerializerPool.Add(type, xmlSerializer);
                }
            }

            return XmlSerializerPool[type];
        }

        public static string Serialize<T>(List<T> list)
        {
            if (null == list)
            {
                return string.Empty;
            }

            string xmlContent;
            var xmlSerializer = GetListSerializer<T>();
            using (Stream xmlStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(xmlStream))
            {
                xmlSerializer.Serialize(streamWriter, list, EmptyNamespaces);
                streamWriter.Flush();
                xmlStream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(xmlStream);
                xmlContent = reader.ReadToEnd();
            }
            return xmlContent;
        }

        public static string SerializeWithoutNamespace<T>(List<T> list)
        {
            if (null == list) return string.Empty;

            string xmlContent;
            var xmlSerializer = GetListSerializer<T>();
            using (Stream xmlStream = new MemoryStream())
            using (var writer = XmlWriter.Create(
                                                xmlStream,
                                                new XmlWriterSettings { OmitXmlDeclaration = true })
                                                )
            {
                xmlSerializer.Serialize(writer, list, EmptyNamespaces);
                writer.Flush();
                xmlStream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(xmlStream);
                xmlContent = reader.ReadToEnd();
            }
            return xmlContent;
        }

        private static XmlSerializer GetListSerializer<TElement>()
        {
            var elementType = typeof(TElement);
            var listType = typeof(List<TElement>);

            if (XmlSerializerPool.ContainsKey(listType)) return XmlSerializerPool[listType];

            lock (Mutex)
            {
                if (!XmlSerializerPool.ContainsKey(listType))
                {
                    var xmlSerializer = new XmlSerializer(listType, new[] { elementType });
                    XmlSerializerPool.Add(listType, xmlSerializer);
                }
            }

            return XmlSerializerPool[listType];
        }

        public static T DeserializeSingleInstance<T>(string stringContent, Type[] extraTypes)
        {
            if (string.IsNullOrEmpty(stringContent))
            {
                return default(T);
            }

            var xmlSerializer = GetSingleInstanceSerializer(typeof(T), extraTypes);

            T result;
            using (Stream xmlStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(xmlStream))
            {
                streamWriter.Write(stringContent);
                streamWriter.Flush();
                xmlStream.Seek(0, SeekOrigin.Begin);

                result = (T)xmlSerializer.Deserialize(xmlStream);
            }

            return result;
        }

        public static T Deserialize<T>(Stream contentStream)
        {
            var xmlSerializer = GetSingleInstanceSerializer(typeof(T), null);

            return (T)xmlSerializer.Deserialize(contentStream);
        }

        public static T Deserialize<T>(string stringContent)
        {
            if (string.IsNullOrEmpty(stringContent))
            {
                return default(T);
            }

            T result;

            var xmlSerializer = GetSingleInstanceSerializer(typeof(T), null);
            using (Stream xmlStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(xmlStream))
            {
                streamWriter.Write(stringContent);
                streamWriter.Flush();
                xmlStream.Seek(0, SeekOrigin.Begin);

                result = (T)xmlSerializer.Deserialize(xmlStream);
            }

            return result;
        }

        public static List<T> DeserializeList<T>(string stringContent)
        {
            if (string.IsNullOrEmpty(stringContent))
            {
                return new List<T>();
            }

            List<T> result;

            var xmlSerializer = GetListSerializer<T>();
            using (Stream xmlStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(xmlStream))
            {
                streamWriter.Write(stringContent);
                streamWriter.Flush();
                xmlStream.Seek(0, SeekOrigin.Begin);

                result = (List<T>)xmlSerializer.Deserialize(xmlStream);
            }

            return result;
        }

        /*
        public static string JsonSerializer<T>(T t)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, t);
                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ms, Encoding.UTF8, false))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        public static T JsonConvertToObject<T>(string jsonStr)
        {
            var der = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr)))
            {
                return (T)der.ReadObject(ms);
            }
        }
         */

        public static string JsonSerializerByNewton<T>(T t)
        {
            return JsonConvert.SerializeObject(t);
        }

        public static T JsonConvertToObjectByNewton<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }


        public static object XmlToObject<T>(string xmlStr)
        {
            using (var ms2 = new MemoryStream(Encoding.UTF8.GetBytes(xmlStr)))
            {
                var xml2 = GetSingleInstanceSerializer(typeof(T), null);
                var obj = xml2.Deserialize(ms2);
                ms2.Close();
                return obj;
            }
        }
    }
}
