using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVChar: SValue
    {
        public char nValue = '\0';

        public SVChar() { }
        public SVChar(char a) { nValue = a; }
        public override bool getBoolValue()
        {
            return nValue != 0;
        }
        public override int getIntValue()
        {
            return nValue;
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
                if (args[0] is SVString)
                {
                    SVString str = args[0] as SVString;
                    if (str.Length > 0)
                        return new SVChar((args[0] as SVString).sValue[0]);
                    else
                        return new SVChar();
                }
                return new SVChar(Convert.ToChar(args[0].getIntValue()));
            }
            else
            {
                return new SVChar();
            }
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            char i = Convert.ToChar((args.list.Count > 0) ? args.list[0].getIntValue() : 0);
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
                    return new SVChar(Convert.ToChar(nValue | i));
                case "&":
                    args.AssertCount(1);
                    return new SVChar(Convert.ToChar(nValue & i));
                case "^":
                    args.AssertCount(1);
                    return new SVChar(Convert.ToChar(nValue ^ i));
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
