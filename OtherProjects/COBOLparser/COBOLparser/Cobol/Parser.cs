using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using COBOLparser.Cobol.Parsers;

namespace COBOLparser.Cobol
{
    public delegate CPBase ParsingFunc(SafeList list, ref int idx, string data);

    public class IPE
    {
        public ParsingFunc func;
        public string data;
        public int min;
        public int max;

        public IPE(ParsingFunc pf)
        {
            func = pf;
        }
        public IPE(ParsingFunc pf, string d)
        {
            func = pf;
            data = d;
        }
        public IPE(ParsingFunc pf, string d, int mn, int mx)
        {
            func = pf;
            data = d;
            min = mn;
            max = mx;
        }
    }

    public class Parser
    {


        public Node Root = new Node();
        public List<PathElement> CurrentPathList = new List<PathElement>();
        public Node CurrentNode = null;
        public InputFile input = null;
        public Node LastDivision = null;
        public Node LastSection = null;
        public Node LastParagraph = null;
        public int nextReadMode = 0;
        public string programName = string.Empty;
        public List<List<string>> filtered = new List<List<string>>();
        public HashSet<string> catchCommands = new HashSet<string>();
        public CPBase trueStat = new CPBase();

        public Dictionary<string,List<string[]>> Symbols = new Dictionary<string,List<string[]>>();

        public Parser()
        {
            CurrentNode = Root;

            AddSymbolDef("%VAR", "% ( % )");
            AddSymbolDef("%VAR", "% OF % OF % OF % OF %");
            AddSymbolDef("%VAR", "% OF % OF % OF %");
            AddSymbolDef("%VAR", "% OF % OF %");
            AddSymbolDef("%VAR", "% OF %");
            AddSymbolDef("%VAR", "%");

            AddSymbolDef("%LISTVAR", "%VAR %[ , %VAR %]");
            AddSymbolDef("%LISTVAR", "%VAR , %VAR , %VAR , %VAR , %VAR , %VAR , %VAR");
            AddSymbolDef("%LISTVAR", "%VAR , %VAR , %VAR , %VAR , %VAR , %VAR");
            AddSymbolDef("%LISTVAR", "%VAR , %VAR , %VAR , %VAR , %VAR");
            AddSymbolDef("%LISTVAR", "%VAR , %VAR , %VAR , %VAR");
            AddSymbolDef("%LISTVAR", "%VAR , %VAR , %VAR");
            AddSymbolDef("%LISTVAR", "%VAR , %VAR");
            AddSymbolDef("%LISTVAR", "%VAR");

            AddSymbolDef("%CONDITION", "( %CONDITION ) %OPER %CONDITION");
            AddSymbolDef("%CONDITION", "( %CONDITION )");
            AddSymbolDef("%CONDITION", "NOT %CONDITION");
            AddSymbolDef("%CONDITION", "%VAR %OPER %CONDITION");
            AddSymbolDef("%CONDITION", "%VAR NUMERIC");
            AddSymbolDef("%CONDITION", "%VAR");

            AddSymbolDef("%OPER", "NOT EQUAL");
            AddSymbolDef("%OPER", "IS EQUAL TO");
            AddSymbolDef("%OPER", "NOT =");
            AddSymbolDef("%OPER", "EQUAL");
            AddSymbolDef("%OPER", "OR");
            AddSymbolDef("%OPER", "AND");
            AddSymbolDef("%OPER", ">");
            AddSymbolDef("%OPER", "<");
            AddSymbolDef("%OPER", "<=");
            AddSymbolDef("%OPER", ">=");
            AddSymbolDef("%OPER", "<>");
            AddSymbolDef("%OPER", "=");

            AddSymbolDef("%STRINGARGS", "%STRARG %[ , %STRARG %]");

            AddSymbolDef("%STRARG", "%VAR DELIMITED BY %");
            AddSymbolDef("%STRARG", "%VAR");

            AddSymbolDef("%EXTINTERPRETER", "TAL");
            AddSymbolDef("%EXTINTERPRETER", "C");

        }

        private void AddSymbolDef(string p, string p_2)
        {
            List<string[]> s;
            if (Symbols.ContainsKey(p))
            {
                s = Symbols[p];
            }
            else
            {
                s = new List<string[]>();
                Symbols.Add(p, s);
            }
            string[] added = p_2.Split(' ');
            s.Add(added);
        }

        /// <summary>
        /// Gets or sets current node on given path.
        /// </summary>
        public string CurrentPath
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (PathElement s in CurrentPathList)
                {
                    sb.AppendFormat("{0}{1}{2}{3}", PathElement.PathSeparatorChar, s.Type, 
                        PathElement.ElementSeparatorChar, s.Value);
                }
                return sb.ToString();
            }
            set
            {
                string[] pp = value.Split(PathElement.PathSeparatorChar);
                if (pp.Length > 0)
                {
                    if (pp[0].Length == 0)
                    {
                        CurrentPathList.Clear();
                        for (int i = 1; i < pp.Length; i++)
                        {
                            CurrentPathList.Add(new PathElement(pp[i]));
                        }
                    }
                    else
                    {
                        foreach (string s in pp)
                        {
                            CurrentPathList.Add(new PathElement(s));
                        }
                    }
                }

                FindOrCreatePath(CurrentPathList);
            }
        }

        /// <summary>
        /// Finds current node or creates it (with all parent elements)
        /// </summary>
        /// <param name="CurrentPathList"></param>
        private void FindOrCreatePath(List<PathElement> CurrentPathList)
        {
            CurrentNode = Root;
            foreach (PathElement pe in CurrentPathList)
            {
                CurrentNode = CurrentNode.FindOrCreateChild(pe);
            }
        }

        public void ParseInput(TextReader ifile)
        {
            int mode = 0;
            int lineNo = 1;
            int indent = 0;
            List<string> tokens = new List<string>();
            SafeList list = new SafeList();
            list.list = tokens;
            string line = ifile.ReadLine();
            string trimmed = line.TrimStart();
            while (line != null)
            {
                trimmed = line.TrimStart();
                indent = line.Length - trimmed.Length;
                line = line.Trim();
                if (line.StartsWith("*"))
                {
                    if (tokens.Count > 0)
                    {
                        ProcessStatement(list, indent);
                        tokens.Clear();
                        mode = 0;
                    }

                    Node n = new Node();
                    n.Type = "*";
                    n.Value = line;
                    CurrentNode.Children.Add(n);
                    //tokens.Clear();
                }
                else if (line.StartsWith("/"))
                {
                    if (tokens.Count > 0)
                    {
                        ProcessStatement(list, indent);
                        tokens.Clear();
                        mode = 0;
                    }

                    Node n = new Node();
                    n.Type = "/";
                    n.Value = line;
                    CurrentNode.Children.Add(n);
                    tokens.Clear();
                }
                else if (line.StartsWith("?"))
                {
                    if (tokens.Count > 0)
                    {
                        ProcessStatement(list, indent);
                        tokens.Clear();
                        mode = 0;
                    }

                    Node n = new Node();
                    n.Type = "?";
                    n.Value = line;
                    CurrentNode.Children.Add(n);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    mode = ParseLine(lineNo, indent, tokens, list, line, sb);
                    if (sb.Length > 0)
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        mode = 0;
                    }
                }
                line = ifile.ReadLine();
                lineNo++;
            }
        }

        private int ParseLine(int lineNo, int indent, List<string> tokens, SafeList list, string line, StringBuilder sb)
        {
            int pos = 0;
            int mode = 0;
            foreach (char c in line)
            {
            retry:
                if (mode == 0) // ---
                {
                    if (Char.IsLetter(c) || c == '#' || c == ':' || c == '$')
                    {
                        sb.Append(c);
                        mode = 1;
                    }
                    else if (c == '=')
                    {
                        sb.Append(c);
                        mode = 101;
                    }
                    else if (Char.IsNumber(c))
                    {
                        sb.Append(c);
                        mode = 3;
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
                    else if (c == ',' || c == '(' || c == ')' || c == ';')
                    {
                        tokens.Add(c.ToString());
                    }
                    else if (c == '.')
                    {
                        ProcessStatement(list, indent);
                        tokens.Clear();
                    }
                    else if (c == '=' || c == '<' || c == '>' || c == '+' || c == '-' || c == '*' || c == '/' || c == '|')
                    {
                        sb.Append(c);
                        mode = 4;
                    }
                    else if (Char.IsWhiteSpace(c))
                    {
                    }
                    else
                    {
                        throw new Exception("Unexpected char in mode " + mode + " at line " + lineNo + ":\n" + line + "\n" + "".PadLeft(pos) + "^\n\n");
                    }
                }
                else if (mode == 101)
                {
                    if (c == ' ' || c == ':' || c == '\"' || c == '\'')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        mode = 0;
                        goto retry;
                    }
                    else if (c == '=')
                    {
                        sb.Append(c);
                        mode = 150;
                    }
                    else if (Char.IsLetterOrDigit(c))
                    {
                        sb.Append(c);
                        mode = 1;
                    }
                    else
                    {
                        throw new Exception("Unexpected char in mode " + mode + " at line " + lineNo + ":\n" + line + "\n" + "".PadLeft(pos) + "^\n\n");
                    }
                }
                else if (mode == 150)
                {
                    if (c == '=')
                    {
                        sb.Append(c);
                        mode = 151;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else if (mode == 151)
                {
                    if (c == '=')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        mode = 0;
                    }
                    else
                    {
                        sb.Append(c);
                        mode = 150;
                    }
                }
                else if (mode == 1) // --- identifier ---
                {
                    if (Char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '?')
                    {
                        sb.Append(c);
                    }
                    else if (Char.IsWhiteSpace(c))
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        mode = 0;
                    }
                    else if (c == '(' || c == ')' || c == ',' || c == ':' || c == ';')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        tokens.Add(c.ToString());
                        mode = 0;
                    }
                    else if (c == '.')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        ProcessStatement(list, indent);
                        tokens.Clear();
                        mode = 0;
                    }
                    else if (c == '=' || c == '|' || c == '<' || c == '>')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        mode = 0;
                        goto retry;
                    }
                    else if (c == '\"')
                    {
                        if (sb.ToString().ToLower().Equals("x"))
                        {
                            sb.Append(c);
                            mode = 2;
                        }
                        else
                        {
                            tokens.Add(sb.ToString());
                            sb.Clear();
                            mode = 0;
                            goto retry;
                        }
                    }
                    else
                    {
                        throw new Exception("Unexpected char in mode " + mode + " at line " + lineNo + ":\n" + line + "\n" + "".PadLeft(pos) + "^\n\n");
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
                    if (c == '"')
                    {
                        sb.Append(c);
                        mode = 2;
                    }
                    else if (Char.IsWhiteSpace(c) || Char.IsLetterOrDigit(c))
                    {
                        sb.Append(c);
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        mode = 0;
                        goto retry;
                    }
                    else if (c == '.')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        ProcessStatement(list, indent);
                        tokens.Clear();
                        mode = 0;
                    }
                    else if (c == ',' || c == ')')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        tokens.Add(c.ToString());
                        mode = 0;
                    }
                    else
                    {
                        throw new Exception("Unexpected char in mode " + mode + " at line " + lineNo + ":\n" + line + "\n" + "".PadLeft(pos) + "^\n\n");
                    }
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
                    if (c == '\'')
                    {
                        sb.Append(c);
                        mode = 6;
                    }
                    else if (Char.IsWhiteSpace(c))
                    {
                        sb.Append(c);
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        mode = 0;
                    }
                    else if (c == '.')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        ProcessStatement(list, indent);
                        tokens.Clear();
                        mode = 0;
                    }
                    else if (c == ',' || c == ')')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        tokens.Add(c.ToString());
                        mode = 0;
                    }
                    else
                    {
                        throw new Exception("Unexpected char in mode " + mode + " at line " + lineNo + ":\n" + line + "\n" + "".PadLeft(pos) + "^\n\n");
                    }
                }
                else if (mode == 3) // --- number ---
                {
                    if (Char.IsNumber(c))
                    {
                        sb.Append(c);
                    }
                    else if (Char.IsWhiteSpace(c))
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        mode = 0;
                    }
                    else if (Char.IsLetter(c) || c == '-')
                    {
                        sb.Append(c);
                        mode = 1;
                    }
                    else if (c == ',' || c == '(' || c == ')' || c == ':' || c == ';')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        tokens.Add(c.ToString());
                        mode = 0;
                    }
                    else if (c == '.')
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        ProcessStatement(list, indent);
                        tokens.Clear();
                        mode = 0;
                    }
                    else
                    {
                        throw new Exception("Unexpected char in mode " + mode + " at line " + lineNo + ":\n" + line + "\n" + "".PadLeft(pos) + "^\n\n");
                    }
                }
                else if (mode == 4) // -- operators ---
                {
                    if (Char.IsWhiteSpace(c))
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        mode = 0;
                    }
                    else if (c == '>' || c == '=' || c == '|')
                    {
                        sb.Append(c);
                    }
                    else if (Char.IsNumber(c))
                    {
                        sb.Append(c);
                        mode = 3;
                    }
                    else if (c == '(' || c == ')' || c == ':' || c == '\"' || c == '\'' || Char.IsLetterOrDigit(c))
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        mode = 0;
                        goto retry;
                    }
                    else
                    {
                        throw new Exception("Unexpected char in mode " + mode + " at line " + lineNo + ":\n" + line + "\n" + "".PadLeft(pos) + "^\n\n");
                    }
                }

                pos++;
            }
            return mode;
        }

        public void ProcessStatement(SafeList stat, int indent)
        {
            if (stat.Count == 0)
                return;

            if (stat.Count >= 2 && stat[1].Equals("DIVISION"))
            {
                PathElement pe = new PathElement(stat[1], stat[0]);
                CurrentPath = pe.NodeString;
                LastDivision = CurrentNode;
                return;
            }

            if (stat.Count >= 2 && stat[1].Equals("SECTION"))
            {
                if (LastDivision != null)
                {
                    PathElement pe = new PathElement(stat[1], stat[0]);
                    CurrentPath = string.Format("{0}{1}", LastDivision.NodeString, pe.NodeString);
                    LastSection = CurrentNode;
                }
                return;
            }

            if (LastDivision == null)
                return;
            switch (LastDivision.Value)
            {
                case "IDENTIFICATION":
                    {
                        if (stat.Count == 1)
                        {
                            if (nextReadMode == 0)
                            {
                                if (stat[0].Equals("PROGRAM-ID"))
                                    nextReadMode = 1;
                            }
                            else if (nextReadMode == 1)
                            {
                                programName = stat[0];
                                nextReadMode = 0;
                            }
                        }
                        else if (nextReadMode == 1)
                        {
                            programName = String.Join(" ", stat);
                            nextReadMode = 0;
                        }
                    }
                    break;
                case "ENVIRONMENT":
                    {
                        Node n = new Node();
                        n.Type = "SENTENCE";
                        n.Value = "";
                        foreach (string s in stat.list)
                        {
                            Node p = new Node();
                            p.Type = "";
                            p.Value = s;
                            n.Children.Add(p);
                        }
                        CurrentNode.Children.Add(n);
                    }
                    break;
                case "DATA":
                    {
                        Node n = new Node();
                        n.Type = "SENTENCE";
                        n.Value = "";
                        foreach (string s in stat.list)
                        {
                            Node p = new Node();
                            p.Type = "";
                            p.Value = s;
                            n.Children.Add(p);
                        }
                        CurrentNode.Children.Add(n);
                    }
                    break;
                case "PROCEDURE":
                    {
                        if (stat.Count == 1 && indent <= 1 && LastSection != null)
                        {
                            PathElement pe = new PathElement("LABEL", stat[0]);
                            CurrentPath = string.Format("{0}{1}{2}", LastDivision.NodeString, LastSection.NodeString, pe.NodeString);
                            LastParagraph = CurrentNode;
                            return;
                        }
                        else
                        {
                            ParseStatementsInSentence(stat);
                        }

                    }
                    break;
            }
        }

        public void ParseStatementsInSentence(SafeList stat)
        {
            int l;
            int si = 0;

            while(stat.Count > si)
            {
                if (catchCommands.Count > 0)
                {
                    while (stat.Count > si)
                    {
                        if (catchCommands.Contains(stat[si]))
                            break;
                        si++;
                    }
                }
                if (stat.Count <= si)
                    break;
                l = GetStatementEnd(stat, si);
                if (l > 0)
                {
                    Node n = new Node();
                    n.Type = "STMT";
                    n.Value = "";
                    Debugger.Log(0, "", programName + " >>");
                    List<string> los = new List<string>();
                    filtered.Add(los);
                    for (int i = si; i < l && i < stat.Count; i++)
                    {
                        Node p = new Node();
                        p.Type = "";
                        p.Value = stat[i];
                        n.Children.Add(p);
                        Debugger.Log(0, "", " " + stat[i]);
                        los.Add(stat[i]);
                    }
                    Debugger.Log(0, "", "\n");
                    si = l;
                    CurrentNode.Children.Add(n);
                }
                else
                {
                    break;
                }
            }
             
        }

        public CPBase TryArithmeticExpression(SafeList stats, ref int startIndex, string data)
        {
        }

        public CPBase TryDataName(SafeList stats, ref int startIndex, string data)
        {
        }

        public CPBase TryIdentifier(SafeList stats, ref int startIndex, string data)
        {
            int i = startIndex;
            int pp;
            CPBase idd;
            List<CPBase> listdd;

            idd = TryQualifiedDataName(stats, ref i, null);
            if (idd == null)
            {
                RaiseException(stats, i, "Expected qualified-data-name");
            }

            do
            {
                listdd = TryZSerial(stats, ref i, new IPE(TryZ, "("), new IPE(TrySubscript), new IPE(TryZ, ")"));
            }
            while(listdd != null);

            listdd = TryZSerial(stats, ref i, new IPE(TryZ, "("), new IPE(TryLeftmostCharacterPosition),
                new IPE(TryZ, ":"), new IPE(TryLength, null, 0, 1), new IPE(TryZ, ")"));

            startIndex = i;
            return idd;
        }

        public CPBase TryLeftmostCharacterPosition(SafeList stats, ref int startIndex, string data)
        {
            return TryArithmeticExpression(stats, ref startIndex, data);
        }

        public CPBase TryLength(SafeList stats, ref int startIndex, string data)
        {
            return TryArithmeticExpression(stats, ref startIndex, data);
        }

        public CPBase TryLiteral(SafeList stats, ref int startIndex, string data)
        {
        }

        public CPBase TryMoveStatement(SafeList stats, string data)
        {
            int i = startIndex;
            bool corresponding = false;

            if (TryZ(stats, ref i, "MOVE") == null)
                return null;

            if (stats[i].Equals("CORR") || stats[i].Equals("CORRESPONDING"))
            {
                corresponding = true;
                i++;
            }

            CPBase idd;

            idd = TryIdentifier(stats, ref i, null);
            if (idd == null)
            {
                idd = TryLiteral(stats, ref i, null);
                if (idd == null)
                    RaiseException(stats, i, "Expected identifier");
            }

            if (TryZ(stats, ref i, "TO") == null)
            {
                RaiseException(stats, i, "Expected TO");
            }

            do
            {
                idd = TryIdentifier(stats, ref i, null);
                TryZ(stats, ref i, ",");
//                if (stats[i].Equals(",")) i++;
            }
            while (idd != null);

            startIndex = i;

            CPBase st = new CPBase();
            st.Value = "MOVE";

            return st;
        }

        public CPBase TrySubscript(SafeList stats, ref int startIndex, string data)
        {
        }

        /// <summary>
        /// Tries to check if element in stats at index startIndex equals string in data
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="startIndex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public CPBase TryZ(SafeList stats, ref int startIndex, string data)
        {
            if (stats[startIndex].Equals(data))
            {
                startIndex++;
                return trueStat;
            }
            return null;
        }

        public CPBase TryZX(SafeList stats, ref int startIndex, params string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (stats[startIndex].Equals(data[i]))
                {
                    trueStat.Value = stats[startIndex];
                    startIndex++;
                    return trueStat;
                }
            }
            return null;
        }

        public List<CPBase> TryZSerial(SafeList stats, ref int startIndex, params IPE[] funcs)
        {
            List<CPBase> rv = new List<CPBase>();
            int i = startIndex;

            for (int idx = 0; idx < funcs.Length; idx++)
            {
                CPBase ev = funcs[idx].func(stats, ref i, funcs[idx].data);
                if (ev == null)
                {
                    if (funcs[idx].min > 0)
                        return null;
                }
                else
                {
                    rv.Add(ev);
                }
            }

            return rv;
        }

        public int GetStatementEnd(SafeList stats, int startIndex)
        {
            int match = 0;

            match = FindTokenPair(stats, startIndex, "EXEC", "END-EXEC");
            if (match >= 0) return match;
            match = FindTokenPair(stats, startIndex, "COMPUTE", "END-COMPUTE");
            if (match >= 0) return match;
            match = TryStatementPattern(stats, startIndex, "MOVE", "%VAR", "TO", "%LISTVAR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "PERFORM", "UNTIL", "%CONDITION");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "PERFORM", "WHEN", "%CONDITION");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "PERFORM", "VARYING", "%", "FROM", "%", "BY", "%", "UNTIL", "%CONDITION");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "PERFORM", "ONE", "OF", "%", "%", "%", "%", "%", "DEPENDING", "ON", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "PERFORM", "%", "UNTIL", "%CONDITION");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "PERFORM", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "ACCEPT", "%", "FROM", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "GO", "TO", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "SET", "%", "DOWN", "BY", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "SET", "%", "TO", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "IF", "%CONDITION");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "END-PERFORM");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "END-IF");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "INITIALIZE", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "EXIT", "%");
            if (match >= 0) return match + startIndex;
            if (stats[startIndex].Equals("STRING"))
            {
                match = TryStatementPattern(stats, startIndex, "STRING", "%STRINGARGS", "INTO", "%VAR");
                if (match >= 0) return match + startIndex;
                match = FindTokenPair(stats, startIndex, "STRING", "INTO");
                if (match >= 0)
                {
                    startIndex = match - 1;
                    match = TryStatementPattern(stats, startIndex, "INTO", "%VAR");
                    return match + startIndex;
                }
            }
            match = TryStatementPattern(stats, startIndex, "OPEN", "%", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "WRITE", "%", "FROM", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "READ", "%", "INTO", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "CALL", "%", "USING", "%LISTVAR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "CALL", "%VAR", "ON", "ERROR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "SEARCH", "ALL", "%", "AT", "END");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "SEARCH", "%", "VARYING", "%", "AT", "END");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "SEARCH", "%", "AT", "END");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "ENTER", "%EXTINTERPRETER", "%", "USING", "%LISTVAR", "GIVING", "%LISTVAR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "ENTER", "%EXTINTERPRETER", "%", "USING", "%LISTVAR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "ENTER", "%EXTINTERPRETER", "%", "GIVING", "%LISTVAR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "ENTER", "%EXTINTERPRETER", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "ENTER", "%", "USING", "%LISTVAR", "GIVING", "%LISTVAR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "ENTER", "%", "USING", "%LISTVAR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "EXIT");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "ELSE");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "CONTINUE");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "END-EVALUATE");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "END-SEARCH");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "END-STRING");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "STOP", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "CLOSE", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "ADD", "%VAR", "TO", "%VAR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "EVALUATE", "%VAR", "ALSO", "%");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "EVALUATE", "%VAR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "DISPLAY", "BASE", "%VAR");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "DISPLAY", "%VAR");
            if (match >= 0) return match + startIndex;
            // ACCEPT CKR100S UNTIL F2 F3 F5 ESCAPE SF16 TIMEOUT 120 ( F1 THRU S-NEXT-PAGE ) 
            match = TryStatementPattern(stats, startIndex, "ACCEPT", "%VAR", "UNTIL", "%", "%", "%", "ESCAPE", "%", "TIMEOUT", "%", "(", "%", "THRU", "%", ")");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "INSPECT", "%VAR", "TALLYING", "%VAR", "FOR", "CHARACTERS");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "WHEN", "%CONDITION");
            if (match >= 0) return match + startIndex;
            match = TryStatementPattern(stats, startIndex, "WHEN", "OTHER");
            if (match >= 0) return match + startIndex;

            RaiseException(stats, startIndex, "Unknown statement");

            return 0;
        }

        public int FindTokenPair(SafeList stats, int startIndex, string token1, string token2)
        {
            if (stats.Count - startIndex < 2)
                return -1;
            if (!stats[startIndex].Equals(token1))
                return -1;
            for (int i = 1; i < stats.Count; i++)
            {
                if (stats[i + startIndex].Equals(token2))
                    return i + startIndex + 1;
            }

            return -1;
        }

        public bool TryParse(SafeList stats, ref int index, params string[] a)
        {
            int i = index;
            int match;
            match = TryStatementPattern(stats, i, a);
            if (match >= 0)
            {
                index += match - 1;
                return true;
            }
            else
                return false;

        }

        public int TryStatementPattern(SafeList stats, int startIndex, params string[] a)
        {
            int idx = 0;
            int match;
            int i = startIndex;

            //Debugger.Log(0, "", "TryStatement: " + a[0] + "\n");

            for (; idx < a.Length;idx++, i++)
            {
                if (stats.Count <= i)
                    return -1;
                if (a[idx].Equals("%"))
                {
                    continue;
                }
                else if (a[idx].Equals("%["))
                {
                    int c = idx + 1;
                    int begin = c;
                    int end = -1;
                    while (c < stats.Count)
                    {
                        if (a[c].Equals("%]"))
                        {
                            end = c;
                            break;
                        }
                        c++;
                    }
                    if (end > 0)
                    {
                        string [] parts = new string[end - begin];
                        Array.Copy(a, begin, parts, 0, end - begin);
                        while (TryParse(stats, ref i, parts))
                        {
                            i++;
                        }
                        i--;
                        idx = end;
                    }
                    else
                    {
                        throw new Exception("Incomplete definition containing %[, missing is %]");
                    }
                }
                else if (Symbols.ContainsKey(a[idx]))
                {
                    bool found = false;
                    List<string[]> s = Symbols[a[idx]];
                    foreach (string[] sl in s)
                    {
                        //Debugger.Log(0, "", " ->\n");
                        if (TryParse(stats, ref i, sl))
                        {
                            found = true;
                            //Debugger.Log(0, "", " <-\n");
                            break;
                        }
                        //Debugger.Log(0, "", " <-\n");
                    }
                    if (!found)
                        return -1;
                }
                else if (!a[idx].Equals(stats[i]))
                    return -1;

            }

            return i - startIndex;
        }

        public void RaiseException(SafeList stats, int problemIndex, string message)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Problem: {0}\n", message);
            int i = problemIndex;
            int length = 0;
            foreach(string s in stats.list)
            {
                if (i > 0)
                {
                    length += s.Length + 1;
                }
                else if (i == 0)
                {
                    sb.AppendFormat(" <*> ");
                }
                i--;
                sb.AppendFormat("{0} ", s);
            }
            sb.AppendLine();
            sb.Append("^".PadLeft(length));
            sb.AppendLine();
            throw new Exception(sb.ToString());
        }
    }
}
