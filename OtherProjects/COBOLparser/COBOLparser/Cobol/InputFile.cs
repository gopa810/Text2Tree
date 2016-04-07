using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace COBOLparser.Cobol
{
    public class InputFile
    {
        private string text = "";
        private int cursor = 0;
        private StringReader sr = null;

        public void reset()
        {
            cursor = 0;
        }

        public void setText(string s)
        {
            text = s;
            cursor = 0;
            sr = new StringReader(text);
        }

        public string readLine()
        {
            return sr.ReadLine();
        }

        public TextReader getReader()
        {
            return sr;
        }
    }
}
