using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB
{
    internal class MyStringBuilder
    {
        private char[] _array;
        public int Length { get; private set; }

        public MyStringBuilder()
        {
            Length = 0;
            _array = new char[16];
        }

        public void Append(string value)
        {
            Resize();

            foreach (var c in value)
                _array[Length++] = c;
        }

        public void Append(char value)
        {
            Resize();

            _array[Length++] = value;
        }

        public void Clear()
        {
            _array = new char[16];
            Length = 0;
        }

        private void Resize()
        {
            if (Length != _array.Length)
                return;

            var newArray = new char[_array.Length * 2];

            for (int i = 0; i < Length; i++)
                newArray[i] = _array[i];

            _array = newArray;
        }

        public override string ToString() 
        {
            return new string(_array, 0, Length);
        }
    }
}
