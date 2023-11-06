using System;
using System.Collections.Generic;
using System.Linq;
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


    }
}
