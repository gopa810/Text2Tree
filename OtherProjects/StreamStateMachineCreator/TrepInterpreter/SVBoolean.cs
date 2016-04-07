using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVBoolean: SVNumber
    {
        public bool bValue = false;

        public SVBoolean() { }
        public SVBoolean(bool a) { bValue = a; }
        public override bool getBoolValue()
        {
            return bValue;
        }
        public override int getIntValue()
        {
            return bValue ? 1 : 0;
        }
        public override long getLongValue()
        {
            return bValue ? 1L : 0L;
        }
        public override double getDoubleValue()
        {
            return bValue ? 1.0 : 0.0;
        }
        public override string getStringValue()
        {
            return bValue.ToString();
        }

        public override SValue CreateInstance(List<SValue> args)
        {
            if (args.Count > 0)
            {
                return new SVBoolean(args[0].getBoolValue());
            }
            else
            {
                return new SVBoolean();
            }
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            bool i = (args.list.Count > 0) ? args.list[0].getBoolValue() : false;
            switch (method)
            {
                case "!=":
                case "ne":
                    args.AssertCount(1);
                    return new SVBoolean(bValue != i);
                case "==":
                case "eq":
                    args.AssertCount(1);
                    return new SVBoolean(bValue == i);
                case "=":
                case "set":
                    args.AssertCount(1);
                    bValue = i;
                    return this;
                case "&&":
                case "and":
                    args.AssertCount(1);
                    return new SVBoolean(bValue && i);
                case "||":
                case "or":
                    args.AssertCount(1);
                    return new SVBoolean(bValue || i);
                case "^^":
                case "xor":
                    args.AssertCount(1);
                    return new SVBoolean((bValue && !i) || (!bValue && i));
                case "!":
                case "not":
                    return new SVBoolean(!bValue);
            }

            return base.ExecuteMethod(parent, space, method, args);
        }
    }
}
