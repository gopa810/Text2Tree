using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace TextTreeParser
{
    public class TTNamedString: TTNamedObject
    {
        public string Value = string.Empty;

        public override string ToString()
        {
            return Value;
        }

        public override TrepInterpreter.SValue CreateInstance(List<TrepInterpreter.SValue> args)
        {
            return new TTNamedString();
        }

        public override TrepInterpreter.SValue ExecuteMethod(TrepInterpreter.Scripting parent, TrepInterpreter.ScriptingSpace space, string method, TrepInterpreter.SVList args)
        {
            if (method.Equals("value"))
            {
                return new SVString(Value);
            }
            else if (method.Equals("setValue"))
            {
                args.AssertCount(1);
                Value = args.list[0].getStringValue();
                return this;
            }

            return base.ExecuteMethod(parent, space, method, args);
        }
    }
}
