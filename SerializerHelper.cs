using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace CloudSync
{
    public class SerializeHelper
    {
        public static string Serialize<T>(object objectToSerialize)
        {
            string output = "";

            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces(); xmlSerializerNamespaces.Add("", "");
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    new XmlSerializer(typeof(T)).Serialize(writer, objectToSerialize, xmlSerializerNamespaces);

                    output = stringWriter.ToString();
                }
            }

            return output;
        }

        public static string Deserialize<T>(string objectToDeserialize, out T outputObject)
        {
            outputObject = default(T);

            try
            {
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces(); xmlSerializerNamespaces.Add("", "");
                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));

                    using (StringReader stringReader = new StringReader(objectToDeserialize))
                    {
                        outputObject = (T)serializer.Deserialize(stringReader);
                    }
                }
            }
            catch (InvalidOperationException exception)
            {
                // There was a problem deserializing the object.  Return the error message to the caller
                if (exception.InnerException != null)
                {
                    return exception.InnerException.Message;
                }
                else
                {
                    return exception.Message;
                }
            }

            return "";
        }
    }
}
