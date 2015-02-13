using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Text2Tree
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
        public bool cValid;
        public char c;
        public TextPosition pos;

        public CharEntry(bool b)
        {
            eof = b;
            c = '\0';
            cValid = false;
            pos = new TextPosition();
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

        private CharEntry currentChar;

        public TextPosition current
        {
            get
            {
                return currentChar.pos;
            }
            set
            {
                currentChar.pos = value;
                validateEof();
                currentChar.cValid = false;
            }
        }

        public void reset()
        {
            currentChar.pos.position = 0;
            currentChar.pos.lineNo = 0;
            currentChar.pos.linePos = 0;
            currentChar.eof = false;
            currentChar.cValid = false;
        }

        public bool canRead()
        {
            return !currentChar.eof;
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
                currentChar.eof = (contentString.Length <= current.position);
            }
        }

        /// <summary>
        /// Get character which is pointed by current.position
        /// Does not move current position.
        /// </summary>
        /// <returns></returns>
        public CharEntry getChar()
        {
            if (currentChar.eof)
            {
                return currentChar;
            }
            if (contentType == CT_STRING)
            {
                // if character is not valid, then read current position
                // if character is valid, then first move to next position
                // then read character (if possible)
                if (currentChar.cValid == false)
                {
                    currentChar.c = contentString[current.position];
                    currentChar.cValid = true;
                }
                else
                {
                    // moving positions
                    moveOneChar();
                }
            }

            return currentChar;
        }

        public void moveOneChar()
        {
            currentChar.pos.position++;
            currentChar.pos.linePos++;
            if (currentChar.c == '\x0a' || currentChar.c == '\x0d')
            {
                currentChar.pos.lineNo++;
                currentChar.pos.linePos = 0;
            }
            validateEof();
            if (!currentChar.eof)
            {
                currentChar.c = contentString[current.position];
            }
        }

        private List<TextPositionObject> stateStack = new List<TextPositionObject>(); 

        public void popState()
        {
            if (stateStack.Count > 0)
            {
                current = stateStack[stateStack.Count - 1].pos;
                stateStack.RemoveAt(stateStack.Count - 1);
            }
        }

        public void pushState()
        {
            TextPositionObject tpo = new TextPositionObject();
            tpo.pos = current;
            stateStack.Add(tpo);
        }

        public void peekState()
        {
            if (stateStack.Count > 0)
            {
                current = stateStack[stateStack.Count - 1].pos;
            }
        }
    }
}
