using System.Collections.Generic;
using System.Text;

namespace XML_reader
{
    public interface ICSVFormater
    {
        void ToCVSFormat(ref Dictionary<string, StringBuilder> builders);
        void ToCVSFormat_Reflection(ref Dictionary<string, StringBuilder> builders);
        Dictionary<string, StringBuilder> GetBuilders();
    }
}
