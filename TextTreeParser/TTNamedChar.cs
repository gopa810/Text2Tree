using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace TextTreeParser
{
    public class TTNamedChar: TTNamedObject
    {
        public char Value = '\0';

        public override string ToString()
        {
            return Value.ToString();
        }

        public override SValue CreateInstance(List<SValue> args)
        {
            return new TTNamedChar();
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("value"))
            {
                return new SVChar(Value);
            }
            else if (method.Equals("setValue"))
            {
                args.AssertCount(1);
                string s = args.list[0].getStringValue();
                if (s.Length > 0)
                    Value = s[0];
                else
                    Value = '\0';
                return this;
            }
            return base.ExecuteMethod(parent, space, method, args);
        }
    }
}
