using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace XML_reader
{
    class Program
    {
        static void Main(string[] args)
        {

            //CreateTestFile(100000);

            for (int i = 1; i < 10; i++)
            {
                Stopwatch t = new Stopwatch();
                t.Start();
                var wr = new xmlImpoter<catalogBook>(new XML_reader.Xml_Parser<catalogBook>("book1.xml", "book"), i*100);
                wr.ProcessAsyc();
                t.Stop();
                TimeSpan ts = t.Elapsed;

                string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                Console.WriteLine("Batch = {0},  Item Count ={1} time={2})", i*100, wr.ItemsCountInInputXMLFile, elapsedTime);
            }
            
            /*
           var  sw = new Stopwatch();
            sw.Start();
            var xread = new XML_reader.Xml_Parser<catalogBook>(@"D:\XML_reader\XML_reader\books.xml", "book");
            int count=xread.PrintElements();
            t.Stop();
            ts = t.Elapsed;

            elapsedTime = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Item Count ={0} time={1})", count, elapsedTime);
            */

            // sequentially ready xml file..
           /* var xmlparser = new XML_reader.Xml_Parser<catalogBook>(@"D:\XML_reader\XML_reader\books.xml", "book");
            foreach (var v in xmlparser.GetElement())
            {
                System.Console.WriteLine(v.id);
            }
            */
        }

        private static void CreateTestFile(int count)
        {
            catalog cb = new catalog();
            cb.book = new List<catalogBook>();
            for (int i = 1; i <= count; i++)
            {
                catalogBook b = new catalogBook();
                b.id = i.ToString();
                b.author = "a";
                b.description = "d";
                b.genre = "g";
                b.price = new decimal();
                b.title = "t";
                b.publish_date = new DateTime();
                cb.book.Add(b);
            }
            var serializer = new XmlSerializer(typeof(catalog));
            TextWriter writer = new StreamWriter("book1.xml");
            serializer.Serialize(writer, cb);
            writer.Close();
        }
     }
}
