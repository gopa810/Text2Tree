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

        public TTAtom getTreeNode()
        {
            TTAtom tn = new TTAtom();
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

    /// <summary>
    /// Class for storing entity used for matching input list of atoms.
    /// In first step of file processing we are reading text file and comparing it with patterns and strings
    /// So matching objects are in that case patterns and strings
    /// In second step we are processing list of atoms (created from original input file)
    /// So matching object needs to store two values (TestType and TestValue) so we
    /// can check actual values of atom in the list (Type and Value)
    /// For this reason we have class TTParserAtom
    /// </summary>
    public class TTParserAtom
    {
        /// <summary>
        /// Type of atom
        /// </summary>
        public string TestType;

        /// <summary>
        /// Value of atom
        /// </summary>
        public string TestValue;

        /// <summary>
        /// default are null values, that means given parameter 
        /// is not used for matching in function Match
        /// </summary>
        public TTParserAtom()
        {
            TestType = null;
            TestValue = null;
        }

        public TTParserAtom(string s)
        {
            TestType = s;
            TestValue = null;
        }

        public TTParserAtom(string s, string v)
        {
            TestType = s;
            TestValue = v;
        }

        /// <summary>
        /// Try to match ATOM received from input file, with this object.
        /// </summary>
        /// <param name="ce">Atom entry from input file</param>
        /// <returns>Returns true if this object's type and/or value equals atom's values</returns>
        public bool Match(TTAtom ce)
        {
            if (TestType != null)
            {
                if (!TestType.Equals(ce.Type))
                    return false;
            }
            if (TestValue != null)
            {
                if (!TestValue.Equals(ce.Value))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// For debugging we need dump of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}/{1}", (TestType != null ? TestType : ""), (TestValue != null ? TestValue : ""));
        }
    }

    /// <summary>
    /// Parser is needed for parsing input stream in customized or optimized way.
    /// Functionaly does the same as TTPattern with the following differences:
    /// - parser creates new tree node for each parsed subitem
    ///   (pattern are just creating one)
    /// - parser can use more parsing methods (Max, First, List)
    ///   (pattern uses just Max and List
    /// </summary>
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

        public TTParser()
        {
        }

        public TTParser(string name)
        {
            Name = name;
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
        public bool ParseAtomList(TTInputTextFile input, TTAtomList tree)
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

        public bool ParseTree(TTAtomList input, TTTreeNode tree)
        {
            TTAtom pos = input.current;

            TTErrorLog.Shared.enterDir("Parser " + Name, pos.startPos);
            // go through all lines
            // and expect success
            // if any line fails, return position in input text file
            // to original position before using this parser
            // and return false
            foreach (TTParserEntry pe in Lines)
            {
                if (!ParseLine(pe, input, tree))
                {
                    input.current = pos;
                    TTErrorLog.Shared.goUp();
                    return false;
                }
            }

            // if nothing else (no failure during parsing)
            // return success
            TTErrorLog.Shared.goUp();
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
        public bool ParseLine(TTParserEntry entry, TTInputTextFile input, TTAtomList result)
        {
            TTAtom work = null;
            TextPosition last = input.next;
            //
            // MAXIMUM
            //
            if (entry.type == TTP_MAX)
            {
                if (entry.args != null)
                {
                    TTErrorLog.Shared.enterDir("Line MAX", last);
                    TextPosition max = new TextPosition();
                    // current position is maximum till now :)
                    max = input.next;
                    TTAtom maxRes = null;
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
                        result.addItem(maxRes);
                        input.next = max;
                    }
                    else
                    {
                        input.next = last;
                        //throw new Exception(string.Format("SYNTAX: Unable to use any of parsers at line {0}, position {1}\n", last.lineNo, last.linePos));
                    }

                    TTErrorLog.Shared.goUp();
                    return succ;
                }
            }
            else if (entry.type == TTP_FIRST)
            {
                if (entry.args != null)
                {
                    TTErrorLog.Shared.enterDir("Line FIRST", last);
                    bool succ = false;
                    foreach (object c in entry.args)
                    {
                        work = ParseObject(c, input);
                        if (work != null && input.next.position > last.position)
                        {
                            result.addItem(work);
                            succ = true;
                            break;
                        }
                        input.next = last;
                    }

                    TTErrorLog.Shared.goUp();
                    return succ;
                }
            }
            else if (entry.type == TTP_LIST)
            {
                if (entry.args != null && entry.args.Length > 0)
                {
                    TTErrorLog.Shared.enterDir("Line LIST", last);
                    bool succ = true;
                    while (succ)
                    {
                        last = input.next;
                        work = ParseObject(entry.args[0], input);
                        if (work != null)
                        {
                            result.addItem(work);
                        }
                        else
                        {
                            input.next = last;
                            succ = false;
                        }
                    }

                    TTErrorLog.Shared.goUp();
                    return true;
                }
            }
            else if (entry.type == TTP_OBJECT)
            {
                if (entry.args != null && entry.args.Length > 0)
                {
                    TTErrorLog.Shared.enterDir("Line OBJECT", last);
                    last = input.next;
                    work = ParseObject(entry.args[0], input);
                    TTErrorLog.Shared.goUp();
                    if (work != null)
                    {
                        input.next = last;
                        return false;
                    }
                    else
                    {
                        result.addItem(work);
                        return true;
                    }
                }
            }

            return false;
        }


        public bool ParseLine(TTParserEntry entry, TTAtomList input, TTTreeNode result)
        {
            TTTreeNode work = null;
            TTAtom last = input.current;
            //
            // MAXIMUM
            //
            if (entry.type == TTP_MAX)
            {
                if (entry.args != null)
                {
                    TTErrorLog.Shared.enterDir("Line MAX", last.startPos);
                    TTAtom max = input.current;
                    TTTreeNode maxRes = null;
                    bool succ = false;

                    // enumerate all parsers
                    // and try to parse
                    foreach (object c in entry.args)
                    {
                        try
                        {
                            work = ParseObject(c, input);
                        }
                        catch(Exception ex)
                        {
                            TTErrorLog.Shared.addLog("{0}", ex.Message);
                            work = null;
                        }
                        if (work != null && input.current.endPos.position > max.endPos.position)
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
                        result.addSubnode(maxRes);
                        input.current = max;
                    }
                    else
                    {
                        input.current = last;
                        //throw new Exception(string.Format("SEMANTICS: Unable to use any of parsers at line {0}, position {1}\n", last.startPos.lineNo, last.startPos.linePos));
                    }

                    TTErrorLog.Shared.goUp();
                    return succ;
                }
            }
            else if (entry.type == TTP_FIRST)
            {
                if (entry.args != null)
                {
                    TTErrorLog.Shared.enterDir("Line FIRST", last.startPos);
                    bool succ = false;
                    foreach (object c in entry.args)
                    {
                        try
                        {
                            work = ParseObject(c, input);
                        }
                        catch(Exception ex)
                        {
                            TTErrorLog.Shared.addLog("{0}", ex.Message);
                            work = null;
                        }
                        if (work != null && input.current.endPos.position > last.endPos.position)
                        {
                            result.addSubnode(work);
                            succ = true;
                            break;
                        }
                        input.current = last;
                    }

                    if (!succ)
                    {
                        //throw new Exception(string.Format("SEMANTICS: Cannot recognize token {2}:{3} at line {0}, position {1}", last.startPos.lineNo, last.startPos.linePos, last.Type, last.Value));
                    }

                    TTErrorLog.Shared.goUp();
                    return succ;
                }
            }
            else if (entry.type == TTP_LIST)
            {
                if (entry.args != null && entry.args.Length > 0)
                {
                    TTErrorLog.Shared.enterDir("Line LIST", last.startPos);
                    bool succ = true;
                    while (succ)
                    {
                        last = input.current;
                        work = ParseObject(entry.args[0], input);
                        if (work != null)
                        {
                            result.addSubnode(work);
                        }
                        else
                        {
                            input.current = last;
                            succ = false;
                        }
                    }

                    TTErrorLog.Shared.goUp();
                    return true;
                }
            }
            else if (entry.type == TTP_OBJECT)
            {
                if (entry.args != null && entry.args.Length > 0)
                {
                    TTErrorLog.Shared.enterDir("Line OBJECT", last.startPos);
                    last = input.current;
                    work = ParseObject(entry.args[0], input);
                    TTErrorLog.Shared.goUp();
                    if (work != null)
                    {
                        input.current = last;
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

        public TTAtom ParseObject(object c, TTInputTextFile input)
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
                        TTErrorLog.Shared.addDir("parse string: " + s, false, last);
                        return null;
                    }
                }
                TTErrorLog.Shared.addDir("parse string: " + s, true, last);
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
                        TTErrorLog.Shared.addDir("parse charset: " + tcs.Name, (result.matchedString.Length > 0), last);
                        return result.getTreeNode();
                    }
                }


            }
            else if (c is TTPattern)
            {
                TTPattern pat = (c as TTPattern);
                TTErrorLog.Shared.enterDir("pattern " + pat.Name, input.next);
                TTAtom tn = pat.ParseAtom(input);
                TTErrorLog.Shared.goUp();
                return tn;
            }
            else if (c is TTParser)
            {
                TTParser parser = c as TTParser;

                TTAtomList tn = new TTAtomList();
                TTErrorLog.Shared.enterDir("parser " + parser.Name, input.next);
                if (parser.ParseAtomList(input, tn))
                {
                    TTErrorLog.Shared.goUp();
                    return tn.first;
                }
                TTErrorLog.Shared.goUp();
                return null;
            }

            return null;
        }

        public TTTreeNode ParseObject(object c, TTAtomList input)
        {
            TTAtom last = input.current;
            if (c is TTParserAtom)
            {
                TTParserAtom s = c as TTParserAtom;
                last = input.current;
                TTAtom c2 = input.getAtom();
                if (c2 != null && s.Match(c2))
                {
                    TTTreeNode tn = new TTTreeNode();
                    tn.atom = c2;
                    TTErrorLog.Shared.addDir(string.Format("atom {0}", c2.ToString()), true, last.startPos);
                    return tn;
                }
                else
                {
                    input.current = last;
                    TTErrorLog.Shared.addDir(string.Format("atom {0}", c2.ToString()), false, last.startPos);
                    return null;
                }
            }
            else if (c is TTPattern)
            {
                TTPattern pat = (c as TTPattern);
                TTErrorLog.Shared.enterDir("pattern " + pat.Name, last.startPos);
                TTTreeNode tn = pat.ParseTree(input);
                TTErrorLog.Shared.goUp();
                return tn;
            }
            else if (c is TTParser)
            {
                TTParser parser = c as TTParser;

                TTTreeNode tn = new TTTreeNode();
                tn.Name = parser.Name;
                TTErrorLog.Shared.enterDir("parser " + parser.Name, last.startPos);
                if (parser.ParseTree(input, tn))
                {
                    TTErrorLog.Shared.goUp();
                    return tn;
                }
                TTErrorLog.Shared.goUp();
                return null;
            }

            return null;
        }

    
    }
}
