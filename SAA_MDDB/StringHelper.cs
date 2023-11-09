using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB
{
    internal class StringHelper
    {
        public static string[] MySplit(string text, char c)
        {
            MyList<string> result = new MyList<string>();
            string word = "";

            for (int i = 0; i<text.Length;i++)
            {
                if (text[i] != c)
                    word += text[i];
                else 
                {
                    result.Add(word);
                    word = "";
                }
            }

            result.Add(word);
            return result.ToArray();
        }

        public static string AddPadding(int padding, string s)
        {
            return s + new string(' ', padding - s.Length) + '|';
        }

        public static int IndexOf(string text, char c)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == c)
                    return i;
            }
            return -1;
        }

        public static string Substring(string text, int start, int end)
        {
            string copy = "";
            for (int i = start; i < end; i++)
            {
                copy += text[i];
            }
            return copy;
        }
    }
}
