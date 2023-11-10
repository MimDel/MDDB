using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Formatters;
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

            for (int i = 0; i < text.Length; i++)
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

        public static string[] MySplit(string text, string delimiter)
        {
            MyList<string> result = new MyList<string>();
            int i = 0;
            int index = 0;
            string copyText = text;

            if (IndexOf(text, delimiter) == -1)
            {
                result.Add(text);
                return result.ToArray();
            }

            while (IndexOf(copyText,delimiter) != -1)
            {
                index = IndexOf(copyText, delimiter);
                result.Add(Substring(text, i, index + i));
                i = index + i + delimiter.Length;
                copyText = Substring(text, i, text.Length);
            }
            result.Add(copyText);
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

        public static int IndexOf(string text, string val)
        {
            for (int i = 0; i < text.Length - (val.Length - 1); i++)
            {
                if (Substring(text, i, i + val.Length) == val)
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

        public static string RemoveSpaces(string text)
        {
            string copy = "";
            foreach (var c in text)
            {
                if (c != ' ')
                    copy += c;
            }
            return copy;
        }

        public static string RemoveExtraSpaces(string text)
        {
            string copy = "";
            bool firstSpace = false;
            foreach (var c in text)
            {
                if (c == ' ')
                {
                    if (!firstSpace)
                        copy += " ";
                    firstSpace = true;
                }
                else
                {
                    copy += c;
                    firstSpace = false;
                }
            }
            return copy;
        }
    }
}
