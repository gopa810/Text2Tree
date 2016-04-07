using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TrepInterpreter
{
    public class SVInt32 : SVNumber
    {
        public Int32 nValue = 0;

        public SVInt32() { }
        public SVInt32(int a) { nValue = a; }
        public override bool getBoolValue()
        {
            return nValue != 0;
        }
        public override int getIntValue()
        {
            return nValue;
        }
        public override char getCharValue()
        {
            return Convert.ToChar(nValue);
        }
        public override long getLongValue()
        {
            return (long)nValue;
        }
        public override double getDoubleValue()
        {
            return (double)nValue;
        }
        public override string getStringValue()
        {
            return nValue.ToString();
        }
        public override SValue CreateInstance(List<SValue> args)
        {
            if (args.Count > 0)
            {
                return new SVInt32(Convert.ToInt32(args[0].getIntValue()));
            }
            else
            {
                return new SVInt32();
            }
        }
        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            int i = (args.list.Count > 0) ? args.list[0].getIntValue() : 0;
            switch (method)
            {
                case "!=":
                case "ne":
                    args.AssertCount(1);
                    return new SVBoolean(nValue != i);
                case "==":
                case "eq":
                    args.AssertCount(1);
                    return new SVBoolean(nValue == i);
                case "<":
                    args.AssertCount(1);
                    return new SVBoolean(nValue < i);
                case "<=":
                    args.AssertCount(1);
                    return new SVBoolean(nValue <= i);
                case ">":
                    args.AssertCount(1);
                    return new SVBoolean(nValue > i);
                case ">=":
                    args.AssertCount(1);
                    return new SVBoolean(nValue >= i);
                case "&&":
                case "and":
                    args.AssertCount(1);
                    return new SVBoolean((nValue != 0) && (i != 0));
                case "||":
                case "or":
                    args.AssertCount(1);
                    return new SVBoolean((nValue != 0) || (i != 0));
                case "^^":
                case "xor":
                    args.AssertCount(1);
                    return new SVBoolean(((nValue != 0) && (i == 0)) || ((nValue == 0) && (i != 0)));
                case "!":
                case "not":
                    return new SVBoolean(nValue == 0);
                case "|":
                    args.AssertCount(1);
                    return new SVInt32(nValue | i);
                case "&":
                    args.AssertCount(1);
                    return new SVInt32(nValue & i);
                case "^":
                    args.AssertCount(1);
                    return new SVInt32(nValue ^ i);
                case "=":
                case "set":
                    args.AssertCount(1);
                    nValue = i;
                    return this;
                case "++":
                case "inc":
                    nValue++;
                    return this;
                case "--":
                case "dec":
                    nValue--;
                    return this;
            }

            return base.ExecuteMethod(parent, space, method, args);
        }
    }

}
