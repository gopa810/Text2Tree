using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVDouble : SVNumber
    {
        public double dValue = 0.0;
        public SVDouble() { }
        public SVDouble(double d) { dValue = d; }
        public override bool getBoolValue()
        {
            return dValue != 0.0;
        }
        public override int getIntValue()
        {
            return Convert.ToInt32(dValue);
        }
        public override long getLongValue()
        {
            return Convert.ToInt64(dValue);
        }
        public override double getDoubleValue()
        {
            return dValue;
        }
        public override string getStringValue()
        {
            return dValue.ToString();
        }
        public override SValue CreateInstance(List<SValue> args)
        {
            if (args.Count > 0)
            {
                if (args[0] is SVNumber)
                    return new SVDouble(args[0].getDoubleValue());
                double d = 0.0;
                double.TryParse(args[0].getStringValue(), out d);
                return new SVDouble(d);
            }
            else
            {
                return new SVDouble();
            }
        }
        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            args.AssertMinCount(1);
            args.AssertIsNumber(0);
            SVNumber sn = args.list[0] as SVNumber;
            double i = (args.list.Count > 0) ? sn.getDoubleValue() : 0.0;
            switch (method)
            {
                case "!=":
                case "ne":
                    args.AssertCount(1);
                    return new SVBoolean(dValue != i);
                case "==":
                case "eq":
                    args.AssertCount(1);
                    return new SVBoolean(dValue == i);
                case "<":
                    args.AssertCount(1);
                    return new SVBoolean(dValue < i);
                case "<=":
                    args.AssertCount(1);
                    return new SVBoolean(dValue <= i);
                case ">":
                    args.AssertCount(1);
                    return new SVBoolean(dValue > i);
                case ">=":
                    args.AssertCount(1);
                    return new SVBoolean(dValue >= i);
                case "=":
                case "set":
                    args.AssertCount(1);
                    dValue = i;
                    return this;
                case "++":
                case "inc":
                    dValue++;
                    return this;
                case "--":
                case "dec":
                    dValue--;
                    return this;
            }

            return base.ExecuteMethod(parent, space, method, args);
        }


    }
}
