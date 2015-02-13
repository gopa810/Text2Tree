using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Text2Tree
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

        public bool Run(TTInputTextFile input, TTTreeNode tree)
        {
            TextPosition pos = input.current;

            foreach (TTParserEntry pe in Lines)
            {
                if (!ParseLine(pe, input, tree))
                {
                    input.current = pos;
                    return false;
                }
            }

            return true;
        }

        public bool ParseLine(TTParserEntry entry, TTInputTextFile input, TTTreeNode result)
        {
            TTTreeNode work = null;
            TextPosition last = input.current;
            //
            // MAXIMUM
            //
            if (entry.type == TTP_MAX)
            {
                if (entry.args != null)
                {
                    TextPosition max = new TextPosition();
                    // current position is maximum till now :)
                    max = input.current;
                    TTTreeNode maxRes = null;
                    bool succ = false;

                    // enumerate all parsers
                    // and try to parse
                    foreach (object c in entry.args)
                    {
                        work = ParseObject(c, input);
                        if (work != null && work.endPos.position >= max.position)
                        {
                            maxRes = work;
                            max = input.current;
                        }

                        // return to original position
                        // after each parsing
                        input.current = last;
                    }

                    if (maxRes != null)
                    {
                        succ = true;
                        result.Subnodes.Add(maxRes);
                        input.current = max;
                        input.moveOneChar();
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
                        if (work != null)
                        {
                            result.Subnodes.Add(work);
                            succ = true;
                            break;
                        }
                        input.current = last;
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
                        last = input.current;
                        work = ParseObject(entry.args[0], input);
                        if (work != null)
                        {
                            result.Subnodes.Add(work);
                        }
                        else
                        {
                            input.current = last;
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
                    last = input.current;
                    work = ParseObject(entry.args[0], input);
                    if (work != null)
                    {
                        input.current = last;
                        return false;
                    }
                    else
                    {
                        result.Subnodes.Add(work);
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

            if (c is string)
            {
                string s = c as string;
                int currPos = 0;
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
                    CharEntry c1 = input.getChar();
                    if (c1.eof == false && tcs.ContainsChar(c1.c))
                        result.AddCharEntry(c1);
                    else
                        return result.getTreeNode();
                }
            }
            else if (c is TTPattern)
            {
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
