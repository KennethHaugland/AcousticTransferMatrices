using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AcousticTransferMatrices.Core.Serialization
{
    public static class SerializationHelpers
    {
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Creates an XElement from an implemented interface
        /// </summary>
        /// <param name="o">Object to be serialized</param>
        /// <returns>An XElement from the implemented interface</returns>
        /// <remarks>see https://stackoverflow.com/questions/1333864/
        /// xml-serialization-of-interface-property for original implementation</remarks>
        public static XElement Serialize(object ImplementedInterface)
        {
            Type ObjectType = ImplementedInterface.GetType();

            Type[] ImplementedTypes = ObjectType.GetProperties()
                   .Where(p => p.PropertyType.IsInterface)
                   .Select(p => p.GetValue(ImplementedInterface, null).GetType())
                   .ToArray();

            DataContractSerializer serializer = new DataContractSerializer(ObjectType, ImplementedTypes);
            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xw = new XmlTextWriter(sw))
                {
                    serializer.WriteObject(xw, ImplementedInterface);
                    return XElement.Parse(sw.ToString());
                }
            }
        }

        /// <summary>
        /// Creates an object from an interfaces saved in a XElement object
        /// </summary>
        /// <param name="File">XElement that should be turned into an object</param>
        /// <param name="ImplementedInterface">Object that implemented the streamed interface</param>
        /// <typeparam name="T">Represents the Interface you want the object casted to</typeparam>
        /// <returns></returns>
        public static T Deserialize<T>(XElement File, object implementedInterface)
        {
            Type ObjectType = implementedInterface.GetType();

            Type[] ImplementedTypes = ObjectType.GetProperties()
                .Where(p => p.PropertyType.IsInterface)
                .Select(p => p.GetValue(implementedInterface, null).GetType())
                .ToArray();

            DataContractSerializer serializer = new DataContractSerializer(ObjectType, ImplementedTypes);
            using (StringReader sr = new StringReader(File.ToString()))
            {
                using (XmlTextReader xr = new XmlTextReader(sr))
                {
                    return (T)serializer.ReadObject(xr);
                }
            }
        }

        public static void SerializeToFile<T>(string fileName , T ImplementedInterface)
        {                     
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.FileStream file = System.IO.File.Create(fileName);

            writer.Serialize(file, ImplementedInterface);
            file.Close();
        }

        public static T ReadSerializedFile<T>(string fileName)
        {
            // Now we can read the serialized book ...  
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            T overview = (T)reader.Deserialize(file);
            file.Close();

            return overview;
        }



    }
}
