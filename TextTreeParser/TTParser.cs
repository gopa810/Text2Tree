using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTreeParser
{
    public class ParserResult
    {
        public object parser = null;
        public StringBuilder matchedString = new StringBuilder();
        public TextPosition startPos;
        public TextPosition endPos;
        public bool posStarted = false;
        public List<ParserResult> subresults = null;

        public string getName()
        {
            if (parser == null)
                return "";
            if (parser is string)
                return "string";
            if (parser is TTCharset)
                return (parser as TTCharset).Name;
            if (parser is TTPattern)
                return (parser as TTPattern).Name;
            if (parser is TTParser)
                return (parser as TTParser).Name;
            return "";
        }

        public string getValue()
        {
            return matchedString.ToString();
        }

        public TTTreeNode getTreeNode()
        {
            TTTreeNode tn = new TTTreeNode();
            tn.Type = getName();
            tn.Value = getValue();
            tn.startPos = startPos;
            tn.endPos = endPos;

            return tn;
        }

        public void AddCharEntry(CharEntry ce)
        {
            matchedString.Append(ce.c);
            if (posStarted)
            {
                endPos = ce.pos;
            }
            else
            {
                startPos = ce.pos;
                endPos = ce.pos;
                posStarted = true;
            }
        }
    }

    public class TTParser
    {
        public static readonly int TTP_MAX = 1;
        public static readonly int TTP_LIST = 2;
        public static readonly int TTP_TRY = 3;
        public static readonly int TTP_FIRST = 4;
        public static readonly int TTP_OBJECT = 5;
        public TTErrorLog errors = null;

        public class TTParserEntry
        {
            public int type;
            public object[] args;

            public TTParserEntry(int t, params object[] a)
            {
                type = t;
                args = a;
            }
        }

        public TTParser()
        {
        }

        public TTParser(string name)
        {
            Name = name;
        }

        public TTParser(TTErrorLog err)
        {
            errors = err;
        }

        public TTParser(string name, TTErrorLog err)
        {
            Name = name;
            errors = err;
        }

        public string Name = string.Empty;
        public List<TTParserEntry> Lines = new List<TTParserEntry>();

        public void Max(params object[] arg)
        {
            Lines.Add(new TTParserEntry(TTP_MAX, arg));
        }

        public void List(params object[] arg)
        {
            Lines.Add(new TTParserEntry(TTP_LIST, arg));
        }

        public void Try(params object[] arg)
        {
            Lines.Add(new TTParserEntry(TTP_TRY, arg));
        }
        public void First(params object[] arg)
        {
            Lines.Add(new TTParserEntry(TTP_FIRST, arg));
        }
        public void ParseString(string str)
        {
            Lines.Add(new TTParserEntry(TTP_OBJECT, new string[] { str }));
        }
        public void ParsePattern(TTPattern pat)
        {
            Lines.Add(new TTParserEntry(TTP_OBJECT, new object[] { pat }));
        }

        /// <summary>
        /// Parses input text file and fills tree object
        /// </summary>
        /// <param name="input">Input text file</param>
        /// <param name="tree">Output tree object</param>
        /// <returns></returns>
        public bool Run(TTInputTextFile input, TTTreeNode tree)
        {
            TextPosition pos = input.next;

            // go through all lines
            // and expect success
            // if any line fails, return position in input text file
            // to original position before using this parser
            // and return false
            foreach (TTParserEntry pe in Lines)
            {
                if (!ParseLine(pe, input, tree))
                {
                    input.next = pos;
                    return false;
                }
            }

            // if nothing else (no failure during parsing)
            // return success
            return true;
        }

        /// <summary>
        /// Parsing line from parser list.
        /// When parsing of line is successful, then returns true
        /// if parsing failed, returns false
        /// </summary>
        /// <param name="entry">Line from parser's list</param>
        /// <param name="input">Input text file</param>
        /// <param name="result">Output tree node</param>
        /// <returns></returns>
        public bool ParseLine(TTParserEntry entry, TTInputTextFile input, TTTreeNode result)
        {
            TTTreeNode work = null;
            TextPosition last = input.next;
            //
            // MAXIMUM
            //
            if (entry.type == TTP_MAX)
            {
                if (entry.args != null)
                {
                    TextPosition max = new TextPosition();
                    // current position is maximum till now :)
                    max = input.next;
                    TTTreeNode maxRes = null;
                    bool succ = false;

                    // enumerate all parsers
                    // and try to parse
                    foreach (object c in entry.args)
                    {
                        work = ParseObject(c, input);
                        if (work != null && input.next.position > max.position)
                        {
                            maxRes = work;
                            max = input.next;
                        }

                        // return to original position
                        // after each parsing
                        input.next = last;
                    }

                    if (maxRes != null)
                    {
                        succ = true;
                        result.addSubnode(maxRes);
                        input.next = max;
                    }
                    else
                    {
                        input.next = last;
                        if (errors != null)
                        {
                            errors.addLog("Unable to use any of parsers at line {0}, position {1}", last.lineNo, last.linePos);
                        }
                    }

                    return succ;
                }
            }
            else if (entry.type == TTP_FIRST)
            {
                if (entry.args != null)
                {
                    bool succ = false;
                    foreach (object c in entry.args)
                    {
                        work = ParseObject(c, input);
                        if (work != null && input.next.position > last.position)
                        {
                            result.addSubnode(work);
                            succ = true;
                            break;
                        }
                        input.next = last;
                    }

                    return succ;
                }
            }
            else if (entry.type == TTP_LIST)
            {
                if (entry.args != null && entry.args.Length > 0)
                {
                    bool succ = true;
                    while (succ)
                    {
                        last = input.next;
                        work = ParseObject(entry.args[0], input);
                        if (work != null)
                        {
                            result.addSubnode(work);
                        }
                        else
                        {
                            input.next = last;
                            succ = false;
                        }
                    }

                    return true;
                }
            }
            else if (entry.type == TTP_OBJECT)
            {
                if (entry.args != null && entry.args.Length > 0)
                {
                    last = input.next;
                    work = ParseObject(entry.args[0], input);
                    if (work != null)
                    {
                        input.next = last;
                        return false;
                    }
                    else
                    {
                        result.addSubnode(work);
                        return true;
                    }
                }
            }

            return false;
        }

        public TTTreeNode ParseObject(object c, TTInputTextFile input)
        {
            ParserResult result = new ParserResult();
            result.parser = c;
            TextPosition last;
            if (c is string)
            {
                string s = c as string;
                int currPos = 0;
                last = input.next;
                while (currPos < s.Length)
                {
                    char c1 = s[currPos];
                    currPos++;
                    CharEntry c2 = input.getChar();
                    if (c2.eof == false && c1 == c2.c)
                    {
                        result.AddCharEntry(c2);
                    }
                    else
                    {
                        input.next = last;
                        return null;
                    }
                }
                return result.getTreeNode();
            }
            else if (c is TTCharset)
            {
                TTCharset tcs = c as TTCharset;
                while (true)
                {
                    last = input.next;
                    CharEntry c1 = input.getChar();
                    if (c1.eof == false && tcs.ContainsChar(c1.c))
                    {
                        result.AddCharEntry(c1);
                    }
                    else
                    {
                        input.next = last;
                        return result.getTreeNode();
                    }
                }
            }
            else if (c is TTPattern)
            {
                TTPattern pat = (c as TTPattern);
                TTTreeNode tn = pat.Parse(input);
                return tn;
            }
            else if (c is TTParser)
            {
                TTParser parser = c as TTParser;

                TTTreeNode tn = new TTTreeNode();
                tn.Type = parser.Name;
                tn.Value = "";
                if (parser.Run(input, tn))
                    return tn;
                return null;
            }

            return null;
        }
    }
}
