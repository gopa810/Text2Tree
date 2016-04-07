using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TrepInterpreter;

namespace TextTreeParser
{
    public class TTPattern: TTNamedObject
    {
        protected class TTPatternEntry
        {
            public int Min = 0;
            public int Max = Int32.MaxValue;
            public int Current = 0;
            public bool Final = false;

            /// <summary>
            /// If Item name == null
            /// then items parsed by this line are not added to tree results
            /// 
            /// if item name == ""
            /// then we will use only subitems of returned tree node
            /// if node has no subitems, we will use returned node itself
            /// 
            /// if item name != ""
            /// then we will use returned node itself
            /// 
            /// </summary>
            public string OutputIdentity
            {
                get
                {
                    return EntryObject.Name;
                }
            }

            /// <summary>
            /// This may be char, TTCharset or TTPattern
            /// </summary>
            public TTNamedObject EntryObject = null;

            public override string ToString()
            {
                return string.Format("{0}:{1}:{2}", Min, Current, Max);
            }
        }

        //public string OutputIdentity = string.Empty;
        protected List<TTPatternEntry> Lines = new List<TTPatternEntry>();
        public int matchingMethod = METHOD_ALL_SERIAL;
        public bool matchingNameTake = false;

        public const int METHOD_ALL_SERIAL = 0;
        public const int METHOD_FIRST = 1;
        public const int METHOD_MAX = 2;
        public const int METHOD_LIST = 3;


        public TTPattern()
        {
        }

        public TTPattern(string str)
        {
            Name = str;
        }

        public override SValue CreateInstance(List<SValue> args)
        {
            if (args.Count >= 1)
            {
                return new TTPattern(args[0].getStringValue());
            }
            else
            {
                return new TTPattern();
            }
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("matchingMethod"))
            {
                return new SVInt32(matchingMethod);
            }
            else if (method.Equals("setMatchingMethod"))
            {
                args.AssertCount(1);
                matchingMethod = args.list[0].getIntValue();
                return space.nullValue;
            }
            else if (method.Equals("matchingMethodString"))
            {
                return new SVString(MethodString);
            }
            else if (method.Equals("name"))
            {
                return new SVString(Name);
            }
            else if (method.Equals("setName"))
            {
                args.AssertCount(1);
                Name = args.list[0].getStringValue();
                return space.nullValue;
            }
            else if (method.Equals("addString"))
            {
                args.AssertCount(3);
                addString(args.getIntValue(0), args.getIntValue(1), args.getStringValue(2));
                return space.nullValue;
            }
            else if (method.Equals("addChar"))
            {
                args.AssertCount(3);
                addChar(args.getIntValue(0), args.getIntValue(1), args.getCharValue(2));
                return space.nullValue;
            }
            else if (method.Equals("addChars"))
            {
                args.AssertCount(3);
                addChars(args.getIntValue(0), args.getIntValue(1), args.getStringValue(2));
                return space.nullValue;
            }
            else if (method.Equals("addCharset"))
            {
                args.AssertCount(3);
                args.AssertIsType(2, typeof(TTCharset));
                addCharset(args.getIntValue(0), args.getIntValue(1), args.list[2] as TTCharset);
                return space.nullValue;
            }
            else if (method.Equals("addPattern"))
            {
                args.AssertCount(3);
                args.AssertIsType(2, typeof(TTCharset));
                addPattern(args.getIntValue(0), args.getIntValue(1), args.list[2] as TTPattern);
                return space.nullValue;
            }
            else if (method.Equals("addAtom"))
            {
                args.AssertCount(5);
                addAtom(args.getIntValue(0), args.getIntValue(1), 
                    args.getStringValue(2), args.getStringValue(3), 
                    args.getStringValue(4));
                return space.nullValue;
            }
            else if (method.Equals("parseTextToAtomList"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTInputTextFile));
                return ParseTextToAtomList(args.list[0] as TTInputTextFile);
            }
            else if (method.Equals("parseTextToAtom"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTInputTextFile));
                return ParseTextToAtom(args.list[0] as TTInputTextFile);
            }
            else if (method.Equals("parseAtomListToTree"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTAtomList));
                return ParseAtomListToTree(args.list[0] as TTAtomList);
            }

            return base.ExecuteMethod(parent, space, method, args);
        }

        public bool IsParallel
        {
            get { return matchingMethod == METHOD_MAX; }
            set { matchingMethod = (value ? METHOD_MAX : METHOD_ALL_SERIAL); }
        }

        public string MethodString
        {
            get
            {
                switch (matchingMethod)
                {
                    case METHOD_ALL_SERIAL:
                        return "SERIAL";
                    case METHOD_FIRST:
                        return "FIRST";
                    case METHOD_LIST:
                        return "LIST";
                    case METHOD_MAX:
                        return "MAX";
                }

                return "UNKNOWN";
            }
        }

        public int MatchingMethod
        {
            get { return matchingMethod; }
            set { matchingMethod = value; }
        }

        public void addString(int min, int max, string str)
        {
            TTNamedString ns = new TTNamedString();
            ns.Value = str;

            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            pe.EntryObject = ns;

            Lines.Add(pe);
        }

        public void addChar(int min, int max, char c)
        {
            TTNamedChar nc = new TTNamedChar();
            nc.Value = c;

            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            pe.EntryObject = nc;

            Lines.Add(pe);
        }

        public void addChars(int min, int max, string str)
        {
            TTCharset cs = new TTCharset("_charset_" + str);
            cs.addChars(str);

            addCharset(min, max, cs);
        }

        public void addCharset(int min, int max, TTCharset cs)
        {
            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            pe.EntryObject = cs;
            Lines.Add(pe);
        }

        public void addPattern(int min, int max, TTPattern pat)
        {
            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            pe.EntryObject = pat;
            //pe.OutputIdentity = pat.Name;
            Lines.Add(pe);
        }

        public void addAtom(int min, int max, string t, string v, string itemName)
        {
            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            TTParserAtom pa = new TTParserAtom();
            pa.TestType = t;
            pa.TestValue = v;
            pa.Name = itemName;
            pe.EntryObject = pa;
            Lines.Add(pe);
        }

        public TTAtomList ParseTextToAtomList(TTInputTextFile input)
        {
            TTAtomList atomList = new TTAtomList();

            TTAtom ta = null;
            do
            {
                TTErrorLog.Shared.enterDir("try", input.next);
                ta = ParseTextToAtom(input);
                if (ta != null)
                {
                    atomList.addItem(ta);
                }
                TTErrorLog.Shared.goUp();
            }
            while (ta != null);

            return atomList;
        }

        public TTAtom ParseTextToAtom(TTInputTextFile input)
        {
            TextPosition orig = input.next;
            bool b;

            switch (matchingMethod)
            {
                case METHOD_ALL_SERIAL:
                    b = ParseTextAllSerial(input, orig);
                    break;
                case METHOD_FIRST:
                    b = ParseTextFirst(input, orig);
                    break;
                case METHOD_MAX:
                    b = ParseTextMax(input, orig);
                    break;
                default:
                    b = false;
                    break;
            }

            if (b)
            {
                TTAtom tn = new TTAtom();
                tn.Type = Name;
                tn.startPos = orig;
                tn.endPos = input.next;
                tn.Value = input.stringFromRange(orig.position, input.next.position - 1); /*  value from input file, needs to be extracted according parsed range */
                return tn;
            }

            return null;
        }

        /// <summary>
        /// Main function for parsing atom list into tree
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public TTNode ParseAtomListToTree(TTAtomList input)
        {
            TTAtom orig = input.current;
            TTNode b = null;

            switch (matchingMethod)
            {
                case METHOD_ALL_SERIAL:
//                    TTErrorLog.Shared.enterDir("#serial", orig.startPos);
                    b = ParseAtomsAllSerial(input, orig);
//                    TTErrorLog.Shared.goUp();
                    break;
                case METHOD_FIRST:
//                    TTErrorLog.Shared.enterDir("#first", orig.startPos);
                    b = ParseAtomsFirst(input, orig);
//                    TTErrorLog.Shared.goUp();
                    break;
                case METHOD_MAX:
//                    TTErrorLog.Shared.enterDir("#paralell", orig.startPos);
                    b = ParseAtomsMax(input, orig);
//                    TTErrorLog.Shared.goUp();
                    break;
                case METHOD_LIST:
//                    TTErrorLog.Shared.enterDir("#list", orig.startPos);
                    b = ParseAtomsList(input, orig);
//                    TTErrorLog.Shared.goUp();
                    break;
                default:
                    break;
            }

            if (b != null)
            {
                RationalizeWhenExpression(b);
            }

            return b;
        }

        public bool RationalizeWhenExpression(TTNode node)
        {
            bool b = false;

            if (node.Name.Equals("expression"))
            {
                // level 15
                if (!b) b = TryBinaryDivide(node, "oper", ",", "oper.comma");
                // level 14
                if (!b) b = TryBinaryDivide(node, "oper", "=", "oper.assign");
                if (!b) b = TryBinaryDivide(node, "oper", "&=", "oper.bandassign");
                if (!b) b = TryBinaryDivide(node, "oper", "|=", "oper.borassign");
                if (!b) b = TryBinaryDivide(node, "oper", "+=", "oper.plusassign");
                if (!b) b = TryBinaryDivide(node, "oper", "-=", "oper.minusassign");
                if (!b) b = TryBinaryDivide(node, "oper", "*=", "oper.multassign");
                if (!b) b = TryBinaryDivide(node, "oper", "/=", "oper.divassign");
                if (!b) b = TryBinaryDivide(node, "oper", "~=", "oper.xorassign");
                if (!b) b = TryBinaryDivide(node, "oper", "%=", "oper.modassign");
                if (!b) b = TryBinaryDivide(node, "oper", "<<=", "oper.lshiftassign");
                if (!b) b = TryBinaryDivide(node, "oper", ">>=", "oper.rshiftassign");
                if (!b) b = TryBinaryDivide(node, "oper", "&&=", "oper.landassign");
                if (!b) b = TryBinaryDivide(node, "oper", "||=", "oper.lorassign");
                // level 13
                if (!b) b = TryBinaryDivide(node, "oper", "<", "oper.lt");
                if (!b) b = TryBinaryDivide(node, "oper", "&", "oper.band");
                if (!b) b = TryBinaryDivide(node, "oper", "|", "oper.bor");
                if (!b) b = TryBinaryDivide(node, "oper", ">", "oper.gt");
                if (!b) b = TryBinaryDivide(node, "oper", "+", "oper.plus");
                if (!b) b = TryBinaryDivide(node, "oper", "-", "oper.minus");
                if (!b) b = TryBinaryDivide(node, "oper", "*", "oper.mult");
                if (!b) b = TryBinaryDivide(node, "oper", "/", "oper.div");
                if (!b) b = TryBinaryDivide(node, "oper", "~", "oper.xor");
                if (!b) b = TryBinaryDivide(node, "oper", "%", "oper.mod");
                if (!b) b = TryBinaryDivide(node, "oper", "<<", "oper.lshift");
                if (!b) b = TryBinaryDivide(node, "oper", ">>", "oper.rshift");
                if (!b) b = TryBinaryDivide(node, "oper", "&&", "oper.land");
                if (!b) b = TryBinaryDivide(node, "oper", "||", "oper.lor");
                if (!b) b = TryBinaryDivide(node, "oper", "<=", "oper.le");
                if (!b) b = TryBinaryDivide(node, "oper", ">=", "oper.ge");
                if (!b) b = TryBinaryDivide(node, "oper", "!=", "oper.neq");
                if (!b) b = TryBinaryDivide(node, "oper", "==", "oper.eq");
                /*if (!b) b = TryUnaryPrefix(node, null, "oper", "++", "unary.inc.prefix", "expression");
                if (!b) b = TryUnaryPostfix(node, null, "oper", "++", "unary.inc.postfix", "expression");
                */

                if (b)
                {
                    foreach (TTNode iter in node.Children)
                    {
                        RationalizeWhenExpression(iter);
                    }
                }
            }

            return true;
        }

        public bool TryUnaryPrefix(TTNode node, string atomType, string atomValue, string newNodeName, string newSubnodeName)
        {
            TTNode tn;
            tn = node.getHead();
            if (tn != null)
            {
                if (tn.canMatchAtom(atomType, atomValue))
                {
                    TTNodeCollection subs = node.takeChildrenAfter(tn);
                    tn.addCollection(subs);
                    tn.Name = newSubnodeName;
                    node.Name = newNodeName;
                    return true;
                }
            }

            return false;
        }

        public bool TryUnaryPostfix(TTNode node, string atomType, string atomValue, string newNodeName, string newSubnodeName)
        {
            TTNode tn;
            tn = node.getTail();
            if (tn != null)
            {
                if (tn.canMatchAtom(atomType, atomValue))
                {
                    TTNodeCollection subs = node.takeChildrenBefore(tn);
                    tn.addCollection(subs);
                    tn.Name = newSubnodeName;
                    node.Name = newNodeName;
                    return true;
                }
            }

            return false;
        }

        public bool TryBinaryDivide(TTNode node, string atomType, string atomValue, string newNodeName)
        {
            TTNode dividerNode;
            dividerNode = node.findNodeForward(atomType, atomValue, null);
            if (dividerNode != null)
            {
                //node.atom = null;

                TTNodeCollection a1, a2;

                a1 = node.takeChildrenBefore(dividerNode);
                node.removeHead();
                a2 = node.takeAllChildren();

                node.addSubnode(a1.createEncapsulationNode("expression"));
                node.addSubnode(a2.createEncapsulationNode("expression"));
                node.Name = newNodeName;

                return true;
            }
            else
            {
                return false;
            }
        }

        private TTNode ParseAtomsMax(TTAtomList input, TTAtom orig)
        {
            TTNode main = new TTNode(Name);
            TTAtom maxPos = orig;
            TTNode maxTree = null;
            TTPatternEntry maxLine = null;

            foreach (TTPatternEntry pe in Lines)
            {
                input.current = orig;
                TTNode currTree = ParseLineAtoms(pe, input);
                if (currTree != null)
                {
                    //if (pe.OutputIdentity != null && pe.OutputIdentity.Length > 0 && (currTree.Name == null || currTree.Name.Length == 0))
                        currTree.Name = pe.OutputIdentity;
                    // this is like negation
                    // if this line was hit, then stop using this parser
                    if (pe.Max < 0)
                    {
                        input.current = orig;
                        return null;
                    }
                    if (maxPos.startPos.position < input.current.startPos.position)
                    {
                        maxPos = input.current;
                        maxLine = pe;
                        maxTree = currTree;
                    }
                }
            }

            if (maxLine != null && maxPos.startPos.position > orig.startPos.position)
            {
                input.current = maxPos;
                main.addSubnode(maxTree);
                return main;
            }

            return null;
        }

        private TTNode ParseAtomsFirst(TTAtomList input, TTAtom orig)
        {
            TTNode main = new TTNode(Name);

            foreach (TTPatternEntry pe in Lines)
            {
                input.current = orig;
                TTNode tn = ParseLineAtoms(pe, input);
                if (tn != null)
                {
                    main.addSubnode(tn);
                    return main;
                }
            }

            input.current = orig;
            return null;
        }

        private TTNode ParseAtomsList(TTAtomList input, TTAtom orig)
        {
            if (Lines.Count == 0)
                return null;

            bool succ = true;
            TTPatternEntry pe = Lines[2];
            TTNode main = new TTNode(Name);

            while (succ)
            {
                TTNode tn = ParseLineAtoms(pe, input);
                if (tn == null)
                {
                    input.current = orig;
                    if (main.Children.count() < pe.Min || main.Children.count() > pe.Max)
                        return null;
                    return main;
                }
                main.addSubnode(tn);
                orig = input.current;
            }

            return null;
        }

        private bool ParseTextFirst(TTInputTextFile input, TextPosition orig)
        {
            foreach (TTPatternEntry pe in Lines)
            {
                input.next = orig;
                if (ParseLineText(pe, input))
                {
                    if (matchingNameTake)
                        Name = pe.OutputIdentity;
                    return true;
                }
            }

            input.next = orig;
            return false;
        }

        private bool ParseTextMax(TTInputTextFile input, TextPosition orig)
        {
            TextPosition maxPos = orig;
            TTPatternEntry maxLine = null;

            foreach (TTPatternEntry pe in Lines)
            {
                input.next = orig;
                if (ParseLineText(pe, input))
                {
                    // this is like negation
                    // if this line was hit, then stop using this parser
                    if (pe.Max < 0)
                    {
                        input.next = orig;
                        return false;
                    }
                    if (maxPos.position < input.next.position)
                    {
                        maxPos = input.next;
                        maxLine = pe;
                    }
                }
            }

            if (maxLine != null && maxPos.position > orig.position)
            {
                if (matchingNameTake)
                {
                    Name = maxLine.OutputIdentity;
                }
                input.next = maxPos;
                return true;
            }

            return false;
        }

        private bool ParseTextAllSerial(TTInputTextFile input, TextPosition orig)
        {
            TextPosition linePos;
            int index = 0;
            int maxIndex = Lines.Count - 1;

            foreach (TTPatternEntry pe in Lines)
            {
                pe.Current = 0;
                while (pe.Current < pe.Max)
                {
                    linePos = input.next;
                    bool b = ParseLineText(pe, input);
                    if (b)
                    {
                        pe.Current++;
                    }
                    else
                    {
                        input.next = linePos;
                        break;
                    }
                }
                if (pe.Current < pe.Min)
                {
                    input.next = orig;
                    return false;
                }
                index++;
            }

            return true;

        }

        private TTNode ParseAtomsAllSerial(TTAtomList input, TTAtom orig)
        {
            TTAtom linePos;
            int index = 0;
            int maxIndex = Lines.Count - 1;

            TTNode rv = new TTNode(Name);

            foreach (TTPatternEntry pe in Lines)
            {
                pe.Current = 0;
                while (pe.Current < pe.Max)
                {
                    linePos = input.current;
                    if (linePos == null)
                        break;

                    TTNode lv = ParseLineAtoms(pe, input);
                    if (lv != null)
                    {
                        rv.addSubnode(lv);
                        pe.Current++;
                        //Debugger.Log(0, "", "Current is " + pe.Current + "\n");
                    }
                    else
                    {
                        input.current = linePos;
                        break;
                    }
                }
                if (pe.Current < pe.Min)
                {
                    input.current = orig;
                    return null;
                }
                index++;
            }

            return rv;

        }


        private void clearFinalizedData(int index, int maxIndex)
        {
            for (int i = index + 1; i <= maxIndex; i++)
            {
                Lines[i].Current = 0;
                Lines[i].Final = false;
            }
        }

        private int findPreviousUnfinishedLine(int index)
        {
            int found = -1;
            for (int i = index - 1; i >= 0; i--)
            {
                if (Lines[i].Final == false)
                {
                    found = i;
                    break;
                }
            }
            return found;
        }

        private bool ParseLineText(TTPatternEntry pe, TTInputTextFile input)
        {
            if (pe.EntryObject is TTNamedChar)
            {
                CharEntry ce = input.getChar();
                if (ce.eof)
                    return false;
                TTNamedChar c = (TTNamedChar)pe.EntryObject;
                bool b = (c.Value == ce.c);
                TTErrorLog.Shared.addDir("char " + c, b, ce.pos);
                return b;
            }
            else if (pe.EntryObject is TTNamedString)
            {
                int i = 0;
                TTNamedString s = pe.EntryObject as TTNamedString;
                CharEntry ce;
                while (i < s.Value.Length)
                {
                    ce = input.getChar();
                    if (ce.eof)
                    {
                        TTErrorLog.Shared.addDir("string " + s, false, ce.pos);
                        return false;
                    }
                    if (ce.c != s.Value[i])
                    {
                        TTErrorLog.Shared.addDir("string " + s, false, ce.pos);
                        return false;
                    }
                    else
                    {
                        i++;
                    }
                }
                TTErrorLog.Shared.addDir("string " + s, true, input.next);
                return true;
            }
            else if (pe.EntryObject is TTCharset)
            {
                TTCharset cset = pe.EntryObject as TTCharset;
                CharEntry ce = input.getChar();
                if (ce.eof)
                {
                    TTErrorLog.Shared.addDir("charset " + cset.Name, false, ce.pos);
                    return false;
                }
                bool b = (cset.ContainsChar(ce.c));
                TTErrorLog.Shared.addDir("charset " + cset.Name, b, ce.pos);
                return b;
            }
            else if (pe.EntryObject is TTPattern)
            {
                TTPattern pat = pe.EntryObject as TTPattern;
                TTErrorLog.Shared.enterDir("pattern " + pat.Name + " (" + pat.MethodString + ")", input.next);
                TTAtom tn = pat.ParseTextToAtom(input);
                TTErrorLog.Shared.goUp();
                return (tn != null);
            }

            TTErrorLog.Shared.addDir("unknown line", false, input.next);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pe"></param>
        /// <param name="input"></param>
        /// <returns>Returns tree node which contains:
        /// If parser object is atom, then tree node refers directly to that atom
        /// If parser object is pattern, then item parsed by pattern are child items of returned tree node
        /// Returned tree node has name given by value of ItemName of pattern line entry.
        /// </returns>
        private TTNode ParseLineAtoms(TTPatternEntry pe, TTAtomList input)
        {
            if (pe.EntryObject is TTParserAtom)
            {
                TTAtom ce = input.getAtom();
                if (ce == null)
                    return null;
                TTParserAtom c = (TTParserAtom)pe.EntryObject;
                string message = string.Format("atom {0} <=> {1}", c, ce);
                if (c.Match(ce))
                {
                    TTErrorLog.Shared.addDir(message, true, ce.startPos);
                    TTNode rv = new TTNode();
                    rv.Name = c.Name;
                    if (pe.OutputIdentity == null)
                    {
                        rv.Name = null;
                    }
                    rv.Value = ce.Value;
                    return rv;
                }
                else
                {
                    TTErrorLog.Shared.addDir(message, false, ce.startPos);
                    return null;
                }
            }
            else if (pe.EntryObject is TTPattern)
            {
                TTPattern pat = pe.EntryObject as TTPattern;
                TTErrorLog.Shared.enterDir("pattern " + pat.Name + " (" + pat.MethodString + ")", (input.current != null ? input.current.startPos : new TextPosition()));
                TTNode rv = pat.ParseAtomListToTree(input);
                if (rv != null)
                    rv.Name = pat.Name;
                //if (rv != null && (rv.Name != null && rv.Name.Length == 0)
                //    && pat.OutputIdentity != null && pat.OutputIdentity.Length > 0)
                //{

                //    rv.Name = pat.OutputIdentity;
                //}
                TTErrorLog.Shared.goUp();
                return rv;
            }

            TTErrorLog.Shared.addDir("unknown line", false, input.current.startPos);
            return null;
        }

    }
}
