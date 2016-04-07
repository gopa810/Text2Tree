using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVInt64 : SVNumber
    {
        public Int64 lValue = 0L;
        public SVInt64() { }
        public SVInt64(long a) { lValue = a; }
        public override bool getBoolValue()
        {
            return lValue != 0;
        }
        public override int getIntValue()
        {
            return (int)lValue;
        }
        public override char getCharValue()
        {
            return Convert.ToChar(lValue);
        }
        public override Int64 getLongValue()
        {
            return lValue;
        }
        public override double getDoubleValue()
        {
            return (double)lValue;
        }
        public override string getStringValue()
        {
            return lValue.ToString();
        }
        public override SValue CreateInstance(List<SValue> args)
        {
            if (args.Count > 0)
            {
                return new SVInt64(args[0].getLongValue());
            }
            else
            {
                return new SVInt64();
            }
        }
        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            Int64 i = (args.list.Count > 0) ? args.list[0].getLongValue() : 0;
            switch (method)
            {
                case "!=":
                case "ne":
                    args.AssertCount(1);
                    return new SVInt32(lValue != i ? 1 : 0);
                case "==":
                case "eq":
                    args.AssertCount(1);
                    return new SVInt32(lValue == i ? 1 : 0);
                case "<":
                    args.AssertCount(1);
                    return new SVInt32(lValue < i ? 1 : 0);
                case "<=":
                    args.AssertCount(1);
                    return new SVInt32(lValue <= i ? 1 : 0);
                case ">":
                    args.AssertCount(1);
                    return new SVInt32(lValue > i ? 1 : 0);
                case ">=":
                    args.AssertCount(1);
                    return new SVInt32(lValue >= i ? 1 : 0);
                case "=":
                case "set":
                    args.AssertCount(1);
                    lValue = i;
                    return this;
                case "&":
                    args.AssertCount(1);
                    return new SVInt64(lValue & i);
                case "&&":
                case "and":
                    args.AssertCount(1);
                    return new SVInt32((lValue != 0) && (i != 0) ? 1 : 0);
                case "|":
                    args.AssertCount(1);
                    return new SVInt64(lValue | i);
                case "||":
                case "or":
                    args.AssertCount(1);
                    return new SVInt32((lValue != 0) || (i != 0) ? 1 : 0);
                case "^":
                    args.AssertCount(1);
                    return new SVInt64(lValue ^ i);
                case "^^":
                case "xor":
                    args.AssertCount(1);
                    return new SVInt32((((lValue != 0) && (i == 0)) || ((lValue == 0) && (i != 0))) ? 1 : 0);
                case "!":
                case "not":
                    return new SVInt64(lValue == 0 ? 1 : 0);
                case "++":
                case "inc":
                    lValue++;
                    return this;
                case "--":
                case "dec":
                    lValue--;
                    return this;
            }

            return base.ExecuteMethod(parent, space, method, args);
        }

    }
}
