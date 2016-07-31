using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XML_reader
{
    public partial class catalogBook :ICSVFormater
    {
        private string mFileName="book.csv";
        public void ToCVSFormat(ref Dictionary<string, StringBuilder> builders)
        {
            // First file logic..
            var csv = builders[mFileName];
            csv.Append(id);
            csv.Append(",");
            csv.Append(author);
            csv.Append(",");
            csv.Append(price);
            csv.Append(",");
            csv.Append(title);
            csv.Append(",");
            csv.Append(genre);
            csv.AppendLine();

            // Add logic for second file...
        }

        public void ToCVSFormat_Reflection(ref Dictionary<string, StringBuilder> builders)
        {
            // First file logic..
            var csv = builders[mFileName];
            var line = GetCSV();
            csv.Append(line);
            csv.AppendLine();
        }

        private string GetCSV()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var fieldValues = this.GetType().GetProperties(flags).Select(f => f.GetValue(this)).ToList();
            var line = string.Join(",", fieldValues);
            //line = Regex.Replace(line, @"\r\n?|\n|,", "");
            return line;

        }
        public  Dictionary<string, StringBuilder> GetBuilders()
        {
            var ret = new Dictionary<string, StringBuilder>();
            ret.Add(mFileName, new StringBuilder(1000000));
            return ret;
        }
    }
}
