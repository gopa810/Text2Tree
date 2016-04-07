using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace TextTreeParser
{
    public class TTAtom: SValue
    {
        public TTAtom next = null;

        public string Type;
        public string Value;
        public TextPosition startPos;
        public TextPosition endPos;



        public TTAtom()
        {
        }

        public TTAtom(string t, string v)
        {
            Type = t;
            Value = v;
        }


        public override SValue CreateInstance(List<SValue> args)
        {
            return new TTAtom();
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("last"))
            {
                return last;
            }
            else if (method.Equals("type"))
            {
                return new SVString(Type);
            }
            else if (method.Equals("value"))
            {
                return new SVString(Value);
            }
            else if (method.Equals("setType"))
            {
                args.AssertCount(1);
                this.Type = args.list[0].getStringValue();
                return args.list[0];
            }
            else if (method.Equals("setValue"))
            {
                args.AssertCount(1);
                this.Value = args.list[0].getStringValue();
                return args.list[0];
            }
            else if (method.Equals("startPos"))
            {
                return new TTInputTextFile.TextPositionObject(startPos);
            }
            else if (method.Equals("endPos"))
            {
                return new TTInputTextFile.TextPositionObject(endPos);
            }
            else if (method.Equals("next"))
            {
                if (next == null)
                    return space.nullValue;
                return next;
            }
            else if (method.Equals("setNext"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTAtom));
                next = args.list[0] as TTAtom;
                return next;
            }

            return base.ExecuteMethod(parent, space, method, args);
        }

        public TTAtom last
        {
            get
            {
                TTAtom t = this;
                while (t.next != null)
                {
                    t = t.next;
                }
                return t;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", (Type != null ? Type : ""), (Value != null ? Value : ""));
        }
    }
}
