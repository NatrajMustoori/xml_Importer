using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XML_reader
{
   public class Xml_Parser<T>
    {
       private string mXmlFilePath;
       private string mElementName;
       public Xml_Parser(string inputxmlFilePath, string elementName)
       {
           mXmlFilePath = inputxmlFilePath;
           mElementName = elementName;
       }

       public int PrintElements()
       {
           int count = 0;
           using (var reader = XmlReader.Create(mXmlFilePath))
           {
               reader.ReadToFollowing(mElementName);
               do
               {
                   var outer = reader.ReadOuterXml();
                   var serializer = new XmlSerializer(typeof(T));
                   catalogBook b;
                   using (TextReader rr = new StringReader(outer))
                   {
                       b = (catalogBook)serializer.Deserialize(rr);
                   }
                   count += 1;
               }
               while (reader.ReadToNextSibling(mElementName));
           }
           return count;
       }

       public IEnumerable<T> GetElement()
       {
           using (XmlReader reader = XmlReader.Create(mXmlFilePath))
           {
               reader.ReadToFollowing(mElementName);
               do
               {
                   var outerXml = reader.ReadOuterXml();
                   var serializer = new XmlSerializer(typeof(T));
                   T item;
                   using (TextReader rr = new StringReader(outerXml))
                   {
                       item = (T)serializer.Deserialize(rr);
                   }
                   yield return item;
               }
               while (reader.ReadToNextSibling(mElementName));
           }
       }
    }
}
