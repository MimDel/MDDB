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

        public static bool IsTypeValid(string text)
        {
            if (!(text == "date" || text == "int" || text == "string"))
                return false;

            return true;
        }

        public static MDDBType? StringToMDDBType(string type)
        {
            switch (type)
            {
                case "int": return MDDBType.Int;
                case "date": return MDDBType.Date;
                case "string": return MDDBType.String;
            }
            return null;
        }
    }
}
