﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TrepInterpreter;

namespace TextTreeParser
{
    /// <summary>
    /// Position in the text maintains absolute posiion from the
    /// beginning of file, as well as line position and line number
    /// </summary>
    public struct TextPosition
    {
        public int linePos;
        public int lineNo;
        public int position;
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", linePos, lineNo, position);
        }
    }

    public class TextPositionClass
    {
        public TextPosition pos;
    }

    public struct CharEntry
    {
        public bool eof;
        public char c;
        public TextPosition pos;

        public CharEntry(bool b)
        {
            eof = b;
            c = '\0';
            pos = new TextPosition();
        }

        public override string ToString()
        {
            return String.Format("Char:{0}  EOF:{1}", c, eof);
        }
    }

    public class TTInputTextFile: SValue
    {
        public class TextPositionObject: SValue
        {
            public TextPosition pos;

            public TextPositionObject() { }
            public TextPositionObject(TextPosition tp) { pos = tp; }
            public override SValue CreateInstance(List<SValue> args)
            {
                return new TextPositionObject();
            }

            public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
            {
                if (method.Equals("line"))
                {
                    return new SVInt32(pos.lineNo);
                }
                else if (method.Equals("lineOffset"))
                {
                    return new SVInt32(pos.linePos);
                }
                else if (method.Equals("fileOffset"))
                {
                    return new SVInt32(pos.position);
                }
                else if (method.Equals("setLine"))
                {
                    args.AssertCount(1);
                    args.AssertIsNumber(0);
                    pos.lineNo = args.list[0].getIntValue();
                }
                else if (method.Equals("setLineOffset"))
                {
                    args.AssertCount(1);
                    args.AssertIsNumber(0);
                    pos.linePos = args.list[0].getIntValue();
                }
                else if (method.Equals("setFileOffset"))
                {
                    args.AssertCount(1);
                    args.AssertIsNumber(0);
                    pos.position = args.list[0].getIntValue();
                }
                return space.nullValue;
            }
        }

        public class CharEntryObject: SValue
        {
            public CharEntry charEntry;

            public CharEntryObject() { }
            public CharEntryObject(CharEntry cc) { charEntry = cc; }

            public override SValue CreateInstance(List<SValue> args)
            {
                return new CharEntryObject();
            }

            public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
            {
                if (method.Equals("character"))
                {
                    return new SVChar(charEntry.c);
                }
                else if (method.Equals("eof"))
                {
                    return new SVBoolean(charEntry.eof);
                }
                else if (method.Equals("position"))
                {
                    return new TextPositionObject(charEntry.pos);
                }
                return base.ExecuteMethod(parent, space, method, args);
            }
        }

        private static readonly int CT_NULL = 0;
        private static readonly int CT_STRING = 1;

        private int contentType = CT_NULL;
        private string contentString = String.Empty;

        private CharEntry lastChar;
        private TextPosition nextPos;

        public override SValue CreateInstance(List<SValue> args)
        {
            return new TTInputTextFile();
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("reset"))
            {
                reset();
            }
            else if (method.Equals("canRead"))
            {
                return new SVBoolean(canRead());
            }
            else if (method.Equals("validateEof"))
            {
                validateEof();
            }
            else if (method.Equals("getChar"))
            {
                return new CharEntryObject(getChar());
            }
            else if (method.Equals("currentChar"))
            {
                return new CharEntryObject(lastChar);
            }
            else if (method.Equals("nextPosition"))
            {
                return new TextPositionObject(nextPos);
            }
            else if (method.Equals("setNextPosition"))
            {
                if (args.list.Count == 1 && args.list[0] is TextPositionObject)
                {
                    TextPositionObject tpo = args.list[0] as TextPositionObject;
                    this.next = tpo.pos;
                }
                else
                {
                    throw new Exception("Expected TextPosition as first argument at " + args.nodeId);
                }
            }
            else if (method.Equals("currentLine"))
            {
                return new SVInt32(nextPos.lineNo);
            }
            else if (method.Equals("currentLineOffset"))
            {
                return new SVInt32(nextPos.linePos);
            }
            else if (method.Equals("currentFileOffset"))
            {
                return new SVInt32(nextPos.position);
            }
            return space.nullValue;
        }

        public TextPosition next
        {
            get
            {
                return nextPos;
            }
            set
            {
                nextPos = value;
                validateEof();
            }
        }

        public void reset()
        {
            lastChar.pos.position = 0;
            lastChar.pos.lineNo = 0;
            lastChar.pos.linePos = 0;
            lastChar.eof = false;
        }

        public bool canRead()
        {
            return !lastChar.eof;
        }

        /// <summary>
        /// Sets string content and validates EOF property
        /// </summary>
        /// <param name="cs"></param>
        public void setContentString(string cs)
        {
            contentType = CT_STRING;
            contentString = cs;
            reset();
            validateEof();
        }

        public void validateEof()
        {
            if (contentType == CT_STRING)
            {
                lastChar.eof = (contentString.Length <= nextPos.position);
            }
        }

        /// <summary>
        /// Get character which is pointed by current.position
        /// Does not move current position.
        /// </summary>
        /// <returns></returns>
        public CharEntry getChar()
        {
            validateEof();
            if (lastChar.eof)
            {
                //Debugger.Log(0, "", "getChar EOF\n");
                return lastChar;
            }
            if (contentType == CT_STRING)
            {
                // if character is not valid, then read current position
                // if character is valid, then first move to next position
                // then read character (if possible)
                lastChar.c = contentString[nextPos.position];
                lastChar.pos = nextPos;
                nextPos.position++;
                nextPos.linePos++;
                if (lastChar.c == '\x0a' || lastChar.c == '\x0d')
                {
                    nextPos.lineNo++;
                    nextPos.linePos = 0;
                }
            }

            //Debugger.Log(0, "", "getChar [" + (lastChar.pos.position) + "] " + lastChar.c + "\n");
            return lastChar;
        }

        private List<TextPositionObject> stateStack = new List<TextPositionObject>(); 

        public void popState()
        {
            if (stateStack.Count > 0)
            {
                next = stateStack[stateStack.Count - 1].pos;
                stateStack.RemoveAt(stateStack.Count - 1);
            }
        }

        public void pushState()
        {
            TextPositionObject tpo = new TextPositionObject();
            tpo.pos = next;
            stateStack.Add(tpo);
        }

        public void peekState()
        {
            if (stateStack.Count > 0)
            {
                next = stateStack[stateStack.Count - 1].pos;
            }
        }

        public string stringFromRange(int pStart, int pEnd)
        {
            if (contentType == CT_STRING)
            {
                return contentString.Substring(pStart, pEnd - pStart + 1);
            }

            return String.Empty;
        }
    }
}
