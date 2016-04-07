using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVInt16 : SVNumber
    {
        public Int16 nValue = 0;

        public SVInt16() { }
        public SVInt16(Int16 a) { nValue = a; }
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
                return new SVInt16((Int16)args[0].getIntValue());
            }
            else
            {
                return new SVInt16();
            }
        }
        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            Int16 i = Convert.ToInt16((args.list.Count > 0) ? args.list[0].getIntValue() : 0);
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
                    return new SVInt16(Convert.ToInt16(nValue | i));
                case "&":
                    args.AssertCount(1);
                    return new SVInt16(Convert.ToInt16(nValue & i));
                case "^":
                    args.AssertCount(1);
                    return new SVInt16(Convert.ToInt16(nValue ^ i));
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
