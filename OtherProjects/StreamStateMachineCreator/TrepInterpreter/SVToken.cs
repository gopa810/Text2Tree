using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVToken : SValue
    {
        public string sValue = "";
        public SVToken() { }
        public SVToken(string s) { sValue = s; }

        public override SValue CreateInstance(List<SValue> args)
        {
            if (args.Count > 0)
            {
                return new SVToken(args[0].getStringValue());
            }
            else
            {
                return new SVToken();
            }
        }

        public override string getStringValue()
        {
            return sValue;
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("=") || method.Equals("set"))
            {
                SValue sv = parent.ExecuteNode(space, args.list[0]);
                space.SetValue(sValue, sv);
                return sv;
            }
            else
            {
                SValue sv = space.GetValue(sValue);
                if (sv != null)
                    return sv.ExecuteMethod(parent, space, method, args);
                throw new Exception("Undefined value for token " + sValue);
            }
        }
    }
}
