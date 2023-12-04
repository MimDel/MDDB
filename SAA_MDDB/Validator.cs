using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB
{
    internal class Validator
    {
        public static bool IsNameValid(string text)
        {
            foreach (var c in text)
            {
                if (!(c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '1' && c <= '9' || c == '_' || c == '-'))
                    return false;
            }
            return true;
        }

        //public static bool IsTypeValid(string text)
        //{
        //    if (!(text == "date" || text == "int" || text == "string"))
        //        return false;

        //    return true;
        //}

        public static MDDBType? StringToMDDBType(string type) => type switch
        {
            "int" => MDDBType.Int,
            "date" => MDDBType.Date,
            "string" => MDDBType.String,
            _ => null,
        };

        public static bool IsTypeValid(MDDBType type, string value) => type switch
        {
            MDDBType.Int => int.TryParse(value,out _),
            MDDBType.String => true,
            MDDBType.Date => DateTime.TryParse(value, out _),
            _ => false,
        };
    }
}
