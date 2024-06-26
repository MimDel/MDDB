﻿using System;
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
            MyStringBuilder sb = new MyStringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != c)
                    sb.Append(text[i]);
                else
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
            }

            result.Add(sb.ToString());
            return result.ToArray();
        }

        public static string[] SplitAttributes(string text, char ignore, char delimiter)
        {
            MyList<string> result = new MyList<string>();
            MyStringBuilder sb = new MyStringBuilder();
            bool isInIgnore = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ignore)
                {
                    isInIgnore = !isInIgnore;
                    continue;
                }

                if (text[i] == delimiter && !isInIgnore)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }

                sb.Append(text[i]);

            }

            result.Add(sb.ToString());
            return result.ToArray();
        }

        public static string[] SplitAttributesIncludeIgnore(string text, char ignore, char delimiter)
        {
            MyList<string> result = new MyList<string>();
            MyStringBuilder sb = new MyStringBuilder();
            bool isInIgnore = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ignore)
                {
                    isInIgnore = !isInIgnore;
                    sb.Append(text[i]);
                    continue;
                }

                if (text[i] == delimiter && !isInIgnore)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }

                sb.Append(text[i]);

            }

            result.Add(sb.ToString());
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

            while (IndexOf(copyText, delimiter) != -1)
            {
                index = IndexOf(copyText, delimiter);
                if (index > 0)
                {
                    result.Add(Substring(text, i, index + i));
                }
                i = index + i + delimiter.Length;
                copyText = Substring(text, i, text.Length);

            }
            if (copyText != "")
                result.Add(copyText);
            return result.ToArray();

        }

        public static string AddPadding(int padding, string s)
        {
            if (s.Length > padding)
            {
                s = Substring(s, 0, padding - 3);
                s += "...";
            }

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
            MyStringBuilder sb = new MyStringBuilder();
            for (int i = start; i < end; i++)
            {
                sb.Append(text[i]);
            }
            return sb.ToString();
        }

        public static string RemoveSpaces(string text)
        {
            MyStringBuilder sb = new MyStringBuilder();
            foreach (var c in text)
            {
                if (c != ' ')
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static string RemoveExtraSpaces(string text)
        {
            MyStringBuilder sb = new MyStringBuilder();
            bool firstSpace = false;
            foreach (var c in text)
            {
                if (c == ' ')
                {
                    if (!firstSpace)
                        sb.Append(' ');

                    firstSpace = true;
                }
                else
                {
                    sb.Append(c);
                    firstSpace = false;
                }
            }
            return sb.ToString();
        }

        public static string Trim(string text)
        {
            int startIndex;
            int endIndex;
            for (startIndex = 0; startIndex < text.Length - 1; startIndex++)
            {
                if (text[startIndex] != ' ')
                    break;
            }
            for (endIndex = text.Length - 1; 0 < endIndex; endIndex--)
            {
                if (text[endIndex] != ' ')
                    break;
            }
            return Substring(text, startIndex, endIndex + 1);
        }
        public static string Trim(string text, char c)
        {
            int startIndex;
            int endIndex;
            for (startIndex = 0; startIndex < text.Length - 1; startIndex++)
            {
                if (text[startIndex] != c)
                    break;
            }
            for (endIndex = text.Length - 1; 0 < endIndex; endIndex--)
            {
                if (text[endIndex] != c)
                    break;
            }
            return Substring(text, startIndex, endIndex + 1);
        }

        public static string ToLower(string text)
        {
            bool inQ = false;
            var copyText = text.ToCharArray();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '"')
                    inQ = !inQ;

                if ((text[i] >= 'A' && text[i] <= 'Z') && !inQ)
                    copyText[i] = (char)(text[i] + 32);
                else
                    copyText[i] = text[i];
            }
            return new string(copyText);
        }

        public static int CompareStrings(string str1, string str2)
        {
            int minLength = Math.Min(str1.Length, str2.Length);

            for (int i = 0; i < minLength; i++)
            {
                int charComparison = str1[i].CompareTo(str2[i]);

                if (charComparison != 0)
                {
                    return charComparison;
                }
            }

            return str1.Length.CompareTo(str2.Length);
        }
    }
}
