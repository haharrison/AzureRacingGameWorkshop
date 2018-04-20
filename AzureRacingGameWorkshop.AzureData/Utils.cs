using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace AzureRacingGameWorkshop.AzureData
{
    public static class Utils
    {

        public static string GetAscendingRowKey(DateTime date)
        {
            return string.Format("{0:D19}", date.Ticks);
        }

        public static string GetAscendingRowKey()
        {
            return GetAscendingRowKey(DateTime.UtcNow);
        }

        public static string GetDescendingRowKey(DateTime date)
        {
            return string.Format("{0:D19}", DateTime.MaxValue.Ticks - date.Ticks);
        }



        public static string GetAscendingRowKey(int integer)
        {
            return string.Format("{0:D19}", integer);
        }

        public static string DataContractSerializeObject<T>(T objectToSerialize)
        {
            using (MemoryStream memStm = new MemoryStream())
            {

                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                StringBuilder sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb))
                {
                    serializer.WriteObject(writer, objectToSerialize);
                    writer.Flush();
                    return sb.ToString();
                }
            }
        }

        public static T Deserialize<T>(string rawXml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(rawXml)))
            {
                DataContractSerializer formatter0 =
                    new DataContractSerializer(typeof(T));
                return (T)formatter0.ReadObject(reader);
            }
        }


    }
}
