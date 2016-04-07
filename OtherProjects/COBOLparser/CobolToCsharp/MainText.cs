using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CobolToCsharp
{
    public class MainText
    {
        public class LineRef
        {
            public int Index;
            public string Text;
            public override string ToString()
            {
                return Text;
            }
        }

        public List<string> lines = new List<string>();
        public List<LineRef> takecareIndexes = new List<LineRef>();

        public void OpenFile(string file)
        {
            string [] lin = File.ReadAllLines(file);
            lines.Clear();
            takecareIndexes.Clear();
            StringBuilder buf = new StringBuilder();

            foreach (string s in lin)
            {
                string h = s.Trim();
                if (StartsWithJoin(ref h))
                {
                    buf.Append(" ");
                    buf.Append(h);
                }
                else
                {
                    if (buf.Length > 0)
                    {
                        lines.Add(buf.ToString());
                    }
                    buf.Clear();
                    buf.Append(s);
                    if (StartWithSpecial(ref h))
                    {
                        LineRef lr = new LineRef();
                        lr.Index = lines.Count;
                        lr.Text = buf.ToString();
                        takecareIndexes.Add(lr);
                    }
                }

            }

            if (buf.Length > 0)
            {
                lines.Add(buf.ToString());
            }
        }

        public string[] joinStarters = new string[] {
            "TO ", "AND ", "OR ", "USING ",
            "FROM ", "GIVING "
        };

        public bool StartsWithJoin(ref string h)
        {
            foreach (string s in joinStarters)
            {
                if (h.StartsWith(s))
                    return true;
            }
            return false;
        }

        public string[] specialStarters = new string[] {
            "ENTER ", "STRING ", "PERFORM "
        };

        public bool StartWithSpecial(ref string h)
        {
            foreach (string s in specialStarters)
            {
                if (h.StartsWith(s))
                    return true;
            }
            return false;
        }

        public List<string> Converted = new List<string>();
        private List<string> currParts = new List<string>();

        public void Convert()
        {
            int indent = 0;
            string nl = string.Empty;
            Converted.Clear();
            foreach (string s in lines)
            {
                indent = CalcIndent(s);
                nl = ConvertLine(indent, s);
                if (nl.IndexOf('\n') >= 0)
                {
                    string[] line2 = nl.Split('\n');
                    foreach (string h in line2)
                    {
                        Converted.Add(h);
                    }
                }
                else
                {
                    Converted.Add(nl);
                }
            }

            if (openMethod)
                Converted.Add("}");
            if (openClass)
                Converted.Add("}");
            if (openNamespace)
                Converted.Add("}");
        }

        private int CalcIndent(string s)
        {
            int c = 0;
            foreach (char ch in s)
            {
                if (Char.IsWhiteSpace(ch))
                    c++;
                else
                    return c;
            }

            return c;
        }

        public string ConvertLine(int indent, string line)
        {
            if (line.Length < 1)
                return string.Empty;

            if (line[0] == ' ')
            {
                SplitIntoParts(line);
                switch (divisionNo)
                {
                    case 0:
                        if (PartsMatches(0, "IDENTIFICATION", "DIVISION"))
                        {
                            line = "namespace AVT\n{";
                            openNamespace = true;
                            divisionNo = 1;
                        }
                        break;
                    case 1:
                        if (PartsMatches(0, "PROGRAM-ID"))
                        {
                            line = "public class " + TokenAt(1) + "\n{";
                            openClass = true;
                        }
                        else if (PartsMatches(0, "ENVIRONMENT", "DIVISION"))
                        {
                            line = "// " + line;
                            divisionNo = 2;
                        }
                        else
                        {
                            line = "// " + line;
                        }
                        break;
                    case 2:
                        if (PartsMatches(0, "DATA", "DIVISION"))
                        {
                            line = "// " + line;
                            divisionNo = 3;
                        }
                        else
                        {
                            line = "// " + line;
                        }
                        break;
                    case 3:
                        if (PartsMatches(0, "PROCEDURE", "DIVISION"))
                        {
                            line = "// " + line;
                            divisionNo = 4;
                        }
                        else
                        {
                            line = "// " + line;
                        }
                        break;
                    case 4:
                        if (indent == 1)
                        {
                            if (PartsMatches(0, "WHENEVER-SECTION"))
                            {
                                line = "public void WheneverSection()\n{";
                                openMethod = true;
                            }
                            else if (currParts.Count > 1 && currParts[1].Equals("SECTION"))
                            {
                                if (openMethod)
                                    line = "}\n\n";
                                else
                                    line = "";
                                line += "public void " + Camelise(currParts[0]) + "()\n{";
                                openMethod = true;
                            }
                            else if (currParts.Count == 1)
                            {
                                line = Camelise(currParts[0]) + ":";
                            }
                            else
                            {
                                line = "// " + line;
                            }
                        }
                        else
                        {
                            if (sqlExecStarted)
                            {
                                if (Equals(0, "END-EXEC"))
                                {
                                    line = ");";
                                    sqlExecString = false;
                                    sqlExecStarted = false;
                                }
                                else
                                {
                                    if (sqlExecString)
                                    {
                                        line = string.Format("+ \"{0}\"", line.Trim().Replace("\"", "\\\""));
                                    }
                                    else
                                    {
                                        line = string.Format("\"{0}\"", line.Trim().Replace("\"", "\\\""));
                                    }
                                    sqlExecString = true;
                                }
                            }
                            else if (Equals(0, "EXIT"))
                                line = "return;";
                            else if (Equals(0, "SET"))
                            {
                                ReduceOF();
                                if (Equals(2, "TO") && CurrCount == 4)
                                    line = string.Format("{0} = {1};", Camelise(TokenAt(1)), Camelise(TokenAt(3)));
                                else
                                    line = "// " + line;
                            }
                            else if (Equals(0, "MOVE"))
                            {
                                ReduceOF();
                                if (Equals(2, "TO") && CurrCount == 4)
                                    line = string.Format("{0} = {1};", Camelise(TokenAt(3)), Camelise(TokenAt(1)));
                                else
                                    line = "// " + line;
                            }
                            else if (Equals(0, "STRING"))
                            {
                                ReduceOF();
                                ReduceSeq(0, null, "DELIMITED", "BY", "SIZE");
                                ReduceSeq(0, null, "DELIMITED", "BY", "SIZE,");
                                StringBuilder sb = new StringBuilder();
                                StringBuilder sba = new StringBuilder();
                                int ari = 0;
                                line = "// " + line;
                                for (int i = 1; i < currParts.Count; i++)
                                {
                                    if (currParts[i].Equals("INTO"))
                                    {
                                        line = string.Format("{0} = string.Format(\"{1}\", {2});", Camelise(currParts[i+1]), sb.ToString(), sba.ToString().Replace(",,", ",").TrimEnd(','));
                                        break;
                                    }
                                    else if (currParts[i].StartsWith("\""))
                                    {
                                        sb.Append(currParts[i].Trim('\"'));
                                    }
                                    else
                                    {
                                        sb.Append('{');
                                        sb.Append(ari.ToString());
                                        sb.Append('}');
                                        sba.AppendFormat("{0},", Camelise(currParts[i]));
                                    }
                                }
                            }
                            else if (Equals(0, "ENTER"))
                            {
                                ReduceOF();
                                line = ProcessProcCall(line, "ENTER", "Enter");
                            }
                            else if (Equals(0, "CALL"))
                            {
                                ReduceOF();
                                line = ProcessProcCall(line, "CALL", "Call");
                            }
                            else if (Equals(0, "IF"))
                            {
                                ReduceOF();
                                ReduceConds(1);
                                StringBuilder sb = new StringBuilder();
                                sb.Clear();
                                sb.Append("if (");
                                for (int j = 1; j < currParts.Count; j++)
                                {
                                    sb.AppendFormat(" {0}", Camelise(currParts[j]));
                                }
                                sb.Append(")\n{");
                                line = sb.ToString();
                            }
                            else if (Equals(0, "ELSE"))
                            {
                                line = "} else {";
                            }
                            else if (Equals(0, "END-IF"))
                            {
                                line = "}";
                            }
                            else if (Equals(0, "EXEC") && Equals(1, "SQL"))
                            {
                                sqlExecStarted = true;
                                sqlExecString = false;
                                if (CurrCount > 2)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append("Process.ExecSql(");
                                    for (int k = 2; k < currParts.Count; k++)
                                    {
                                        sb.AppendFormat(" {0}", currParts[k]);
                                    }
                                    sb.Append("\"");
                                }
                                else
                                {
                                    line = "Process.ExecSql(";
                                }
                            }
                            else if (Equals(0, "PERFORM") && CurrCount == 2)
                                line = string.Format("{0}();", Camelise(TokenAt(1)));
                            else if (PartsMatches(0, "STOP", "RUN"))
                                line = "return;";
                            else
                            {
                                if (line.Trim().Equals("."))
                                {
                                    line = "";
                                }
                                else
                                {
                                    line = "//= " + line;
                                }
                            }
                        }
                        break;
                }
                return line;
            }
            else
            {
                return string.Format("// {0}", line.Substring(1));
            }

        }

        private bool sqlExecStarted = false;
        private bool sqlExecString = false;

        private string ProcessProcCall(string line, string ppn, string iMethod)
        {
            int mode = 0;
            string method = iMethod;
            string procname = null;
            List<string> args = new List<string>();
            string resName = null;
            foreach (string s in currParts)
            {
                if (mode == 0)
                {
                    if (s.Equals(ppn))
                    {
                        if (Equals(1, "TAL"))
                        {
                            method += "Tal";
                            mode = 10;
                        }
                        else
                            mode = 1;
                    }
                    if (s.Equals("USING"))
                        mode = 2;
                    if (s.Equals("GIVING"))
                        mode = 3;
                }
                else if (mode == 10)
                {
                    mode = 1;
                }
                else if (mode == 1)
                {
                    procname = s;
                    mode = 0;
                }
                else if (mode == 2)
                {
                    if (s.Equals("GIVING"))
                        mode = 3;
                    else
                    {
                        string sh = Camelise(s);
                        if (sh.EndsWith(","))
                            sh = sh.Substring(0, sh.Length - 1);
                        args.Add(sh);
                    }
                }
                else if (mode == 3)
                    resName = Camelise(s);
            }

            StringBuilder sb = new StringBuilder();
            if (resName != null)
                sb.AppendFormat("{0} = ", resName);
            sb.AppendFormat("Process.{0}({1}", method, procname);
            foreach (string a in args)
            {
                sb.AppendFormat(", {0}", a);
            }
            sb.Append(");");
            line = sb.ToString();
            return line;
        }

        private int divisionNo = 0;
        private StringBuilder currpart = new StringBuilder();
        private bool openNamespace = false;
        private bool openClass = false;
        private bool openMethod = false;

        private StringBuilder cameliseBuilder = new StringBuilder();

        private int CurrCount
        {
            get { return currParts.Count; }
        }
        private string Camelise(string s)
        {
            cameliseBuilder.Clear();
            if (s.IndexOf('-') < 0)
                return s;
            if (s.StartsWith("\""))
                return s;
            if (s.IndexOf(':') > 0)
            {
                int a = s.IndexOf('(');
                int b = s.IndexOf(':');
                int c = s.IndexOf(')');
                if (a > 0 && b > a && c > b && c == s.Length - 1)
                {
                    s = string.Format("{0}.Substring({1},{2})", s.Substring(0, a), s.Substring(a + 1, b - a - 1), s.Substring(b + 1, c - b - 1));
                }
            }
            return s.Replace('-', '_');
        }

        private void ReduceConds(int i)
        {
            ReduceSeq(i, "!=", "NOT", "=");
            ReduceSeq(i, "!=", "NOT", "EQUAL");
            ReduceSeq(i, "==", "EQUAL");
            ReduceSeq(i, "&&", "AND");
            ReduceSeq(i, "||", "OR");
            ReduceSeq(i, "==", "=");
        }

        private void ReduceSeq(int startIndex, string newValue, params string[] args)
        {
            for (int i = startIndex; i < currParts.Count - args.Length; i++)
            {
                if (PartsMatches(i, args))
                {
                    currParts.RemoveRange(i, args.Length);
                    if (newValue != null)
                        currParts.Insert(i, newValue);
                }
            }
        }

        private void ReduceOF()
        {
            bool hit = false;
            do
            {
                hit = false;
                for (int i = currParts.Count - 2; i > 0; i--)
                {
                    if (currParts[i].Equals("OF"))
                    {
                        string a = Camelise(currParts[i - 1]);
                        string b = Camelise(currParts[i + 1]);
                        string pref = "";
                        string post = "";
                        if (b.EndsWith(","))
                        {
                            post = "," + post;
                            b = b.Substring(0, b.Length - 1);
                        }
                        string s = string.Format("{0}.{1}", b, a);
                        currParts.RemoveRange(i - 1, 3);
                        currParts.Insert(i - 1, s);
                        hit = true;
                    }
                }
            } while (hit);

        }

        private string TokenAt(int i)
        {
            if (i >= currParts.Count)
                return string.Empty;
            return currParts[i];
        }

        private bool Equals(int i, string value)
        {
            if (i >= currParts.Count)
                return false;
            return currParts[i].Equals(value);
        }

        private bool PartsMatches(int startIndex, params string[] args)
        {
            if (args.Length > currParts.Count)
                return false;
            for (int i = startIndex; i < startIndex + args.Length; i++)
            {
                if (!currParts[i].Equals(args[i-startIndex]))
                    return false;
            }
            return true;
        }

        private int SplitIntoParts(string line)
        {
            StringBuilder sb = currpart;
            List<string> tokens = currParts;
            currpart.Clear();
            currParts.Clear();

            int mode = 0;
            bool retry = false;
            foreach (char c in line)
            {
                do
                {
                    retry = false;
                    if (mode == 0) // ---
                    {
                        if (c == '(' || c == ')' || c == ',')
                        {
                            if (sb.Length > 0)
                                AddToken(sb);
                            sb.Clear();
                            AddToken(c);
                        }
                        else if (c == '\'')
                        {
                            sb.Append(c);
                            mode = 6;
                        }
                        else if (c == '"')
                        {
                            sb.Append(c);
                            mode = 2;
                        }
                        else if (Char.IsWhiteSpace(c))
                        {
                        }
                        else
                        {
                            sb.Append(c);
                            mode = 1;
                        }
                    }
                    else if (mode == 1) // --- identifier ---
                    {
                        if (Char.IsWhiteSpace(c))
                        {
                            AddToken(sb);
                            sb.Clear();
                            mode = 0;
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    else if (mode == 2) // --- string ----
                    {
                        sb.Append(c);
                        if (c == '"')
                        {
                            mode = 201;
                        }
                    }
                    else if (mode == 201)
                    {
                        mode = ProcessCharInStringLiteral('\"', 2, sb, tokens, c);
                        retry = (mode == 0);
                    }
                    else if (mode == 6) // --- string ----
                    {
                        sb.Append(c);
                        if (c == '\'')
                        {
                            mode = 601;
                        }
                    }
                    else if (mode == 601)
                    {
                        mode = ProcessCharInStringLiteral('\'', 6, sb, tokens, c);
                        retry = (mode == 0);
                    }
                } while (retry);

            }

            if (sb.Length > 0)
                AddToken(sb);

            return mode;
        }

        private void AddToken(object a)
        {
            string s = a.ToString().TrimEnd('.');
            currParts.Add(s);
        }

        private int ProcessCharInStringLiteral(char ss, int remode, StringBuilder sb, List<string> tokens, char c)
        {
            if (c == ss)
            {
                sb.Append(c);
                return remode;
            }
            else if (Char.IsWhiteSpace(c))
            {
                AddToken(sb);
                sb.Clear();
            }
            return 0;
        }

    }
}
