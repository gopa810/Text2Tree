using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTreeParser
{
    public class TTErrorLog
    {
        public StringBuilder sb = new StringBuilder();

        public override string ToString()
        {
            return sb.ToString();
        }

        public void addLog(string format, params object[] args)
        {
            sb.AppendFormat(format, args);
        }
    }
}
