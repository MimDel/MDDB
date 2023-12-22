using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB
{
    class Cell
    {
        public string ColName;
        public MDDBType Type;
        public string Value;

        public Cell(string colName, MDDBType type, string value)
        {
            ColName = colName;
            Type = type;
            Value = value;
        }

    }
}
