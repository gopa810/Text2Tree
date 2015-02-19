using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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

    public class TTInputTextFile
    {
        private class TextPositionObject
        {
            public TextPosition pos;
        }

        private static readonly int CT_NULL = 0;
        private static readonly int CT_STRING = 1;

        private int contentType = CT_NULL;
        private string contentString = String.Empty;

        private CharEntry lastChar;
        private TextPosition nextPos;

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
                Debugger.Log(0, "", "getChar EOF\n");
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

            Debugger.Log(0, "", "getChar [" + (lastChar.pos.position) + "] " + lastChar.c + "\n");
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
