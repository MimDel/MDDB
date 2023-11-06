using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAA_MDDB
{
    class DataFileStreamArray
    {
        private readonly Stream _fs;
        private readonly Stream _mfs;
        private readonly MyList<Column> _metaData;
        private BinaryReader _mbr;
        private BinaryReader _br;
        private readonly BinaryWriter _bw;
        private int DATA_SIZE;
        private int _rowCount = 0;

        public string _tableName;
        public readonly int Offset = 4;

        public string this[int index]
        {
            get 
            {
                var output = "";
                _fs.Seek(Offset + index * DATA_SIZE, SeekOrigin.Begin);
                foreach (var c in _metaData) 
                {
                    output += CharsToString(_br.ReadChars(c.GetSize())) + " ";
                }
                return output;
            }
            set
            {
                _rowCount++;
                var val = StringHelper.MySplit(value, '\0');

                if (_metaData.Count != val.Length)
                { 
                    Console.WriteLine("Wrong number of columns.");
                    return;
                }

                _bw.Write(_rowCount);
                _fs.Seek(Offset + index * DATA_SIZE, SeekOrigin.Begin);
                
                for (int i = 0; i < val.Length; i++)
                {
                    _bw.Write(StringToChars(val[i], _metaData[i].GetSize())); 
                }
            }
        }

        public DataFileStreamArray(string tableName)
        {
            _fs = File.Open(tableName, FileMode.Open, FileAccess.ReadWrite);
            _mfs = File.Open($"Meta_{tableName}", FileMode.Open, FileAccess.Read);
            _mbr = new BinaryReader(_mfs, Encoding.ASCII, true);
            _br = new BinaryReader(_fs, Encoding.ASCII,true);
            _bw = new BinaryWriter(_fs,Encoding.ASCII,true);        
            _metaData = LoadData(tableName);
            foreach (var col in _metaData)
            {
                DATA_SIZE += col.GetSize();
            }
            _tableName = tableName;
            _rowCount = _br.ReadInt32();
        }

        public static string CharsToString(char[] chars)
        {
            var c = 0;
            for (; c < chars.Length && chars[c] != 0; c++) ;

            return new string(chars, 0, c);
        }

        public static char[] StringToChars(string text, int length)
        {
            var chars = new char[length];
            var textArr = text.ToCharArray();

            for (var c = 0; c < Math.Min(textArr.Length, length); c++)
                chars[c] = text[c];

            return chars;
        }

        public MyList<Column> LoadData(string name)
        {
            var cols = new MyList<Column>();
            var count = _mbr.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var col = new Column(_mbr.ReadString(), (MDDBType)_mbr.ReadByte());
                col.DefaultValue = _mbr.ReadString();
                col.IsAutoIncrement = _mbr.ReadBoolean();
                cols.Add(col);
            }

            return cols;
        }
    }
}
