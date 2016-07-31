using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using  System.Collections.Concurrent;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XML_reader
{
    public class xmlImpoter<T> where T : ICSVFormater, new()
    {
        private Xml_Parser<T> mXmlParser;
        private ConcurrentBag<T> mItems;
        private bool mIsXMLReadCompelet = false;
        private int mBatchSize;
        private volatile int mItemCount;

        public int ItemsCountInInputXMLFile { get { return mItemCount; } }
        public xmlImpoter(Xml_Parser<T> xmlParser, int batchSize)
        {
            mXmlParser = xmlParser;
            mBatchSize = batchSize > 0 ? batchSize : 100; 
            mItems = new ConcurrentBag<T>();
        }

        public void Process()
        {
            int count = 0;
            List<T> items = new List<T>();
            foreach (var Item in mXmlParser.GetElement())
            {
                items.Add(Item);
                count += 1;

                if (items.Count == mBatchSize)
                {
                    Write2CVSFile(items);
                    items = new List<T>();
                }
            }
            //remaining..
            Write2CVSFile(items);
            mItemCount = count;
        }

        /// <summary>
        /// Processes the Xml reads and write in producer & consumer design..
        /// </summary>
        public void ProcessAsyc()
        {
            Parallel.Invoke(() => ReadElements(), () => WriteElements());
        }

        /// <summary>
        /// Reads the XML elements in parallel
        /// </summary>
        private void ReadElements()  // Producser 
        {
            Parallel.ForEach(mXmlParser.GetElement(), (b) =>
            {
                mItems.Add(b);

            });
            mIsXMLReadCompelet = true;
        }
        
        /// <summary>
        /// Writes the elements to csv file in batch approach
        /// </summary>
        private void WriteElements()  // consumer 
        {
            List<T> items = new List<T>();
            while (!mIsXMLReadCompelet) // loop untill xml reads are compelet's
            {
                Thread.Sleep(100);
                int count = mItems.Count;
                for (int i = 0; i < count; i++)
                {
                    T item = new T();
                    mItems.TryTake(out item);
                    items.Add(item);
                }
                if (items.Count >= mBatchSize)
                {
                    Write2CVSFile(items);
                    items = new List<T>(); // create new temp item collection
                }
            }
            // second time check to clear any remaing items
            int cc = mItems.Count;
            for (int i = 0; i < cc; i++)
            {
                T item = new T();
                mItems.TryTake(out item);
                items.Add(item);
            }
            if (items.Count > 0)
            {
                Write2CVSFile(items);
                items.RemoveAll(x => true);
            }
        }
        private void print(List<T> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var b = items[i] as catalogBook;
                System.Console.WriteLine(b.id);
            }
        }

        private void Write2CVSFile(List<T> items)
        {
            mItemCount += items.Count;
            var b = new T().GetBuilders();
            for (int i = 0; i < items.Count; i++) // can optimise if we have only one record per xml node..
            {
                items[i].ToCVSFormat_Reflection(ref b);
            }
            foreach (var k in b.Keys) // if multiple file required then we can do the parallel foreach
            {
                var filename = k;
                var lines = b[k].ToString();
                using (System.IO.StreamWriter file = new StreamWriter(filename, true))
                {
                    file.WriteLine(lines.TrimEnd());
                }
            }
        }

        private void GetCSV(T t)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var fieldValues = t.GetType().GetProperties(flags).Select(f => f.GetValue(t)).ToList();
            var line = string.Join(",", fieldValues);
            line = Regex.Replace(line, @"\r\n?|\n", "");

        }
    }
}
