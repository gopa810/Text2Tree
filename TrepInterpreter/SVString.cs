using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVString : SValue
    {
        public string sValue = null;
        public SVString() { }
        public SVString(string s) { sValue = s; }

        public override bool getBoolValue()
        {
            return ((sValue != null) && (sValue.Length > 0));
        }
        public override char getCharValue()
        {
            if (Length > 0)
            {
                return sValue[0];
            }
            else
            {
                return '\0';
            }
        }

        public int Length
        {
            get
            {
                if (sValue != null)
                    return sValue.Length;
                return 0;
            }
        }
        public override string getStringValue()
        {
            return sValue;
        }

        public override SValue CreateInstance(List<SValue> args)
        {
            if (args.Count > 0)
            {
                return new SVString(args[0].getStringValue());
            }
            else
            {
                return new SVString();
            }
        }

        /// <summary>
        /// Executing method
        /// </summary>
        /// <param name="parent">Current script</param>
        /// <param name="space">Current variable space</param>
        /// <param name="method">Method name</param>
        /// <param name="args">Additional arguments</param>
        /// <returns></returns>
        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            string i = (args.list.Count > 0) ? args.list[0].getStringValue() : string.Empty;
            switch (method)
            {
                case "!=":
                case "ne":
                    if (args.list.Count != 1)
                    {
                        throw new Exception("Invalid number of arguments for oepartor at " + args.nodeId);
                    }
                    if (args.list[0] is SVNull)
                    {
                        return new SVBoolean(sValue != null);
                    }
                    return new SVBoolean(!sValue.Equals(i));
                case "==":
                case "eq":
                    if (args.list.Count != 1)
                    {
                        throw new Exception("Invalid number of arguments for oepartor at " + args.nodeId);
                    }
                    if (args.list[0] is SVNull)
                    {
                        return new SVBoolean(sValue == null);
                    }
                    return new SVBoolean(sValue.Equals(i));
                case "<":
                    if (args.list.Count != 1)
                    {
                        throw new Exception("Invalid number of arguments for oepartor at " + args.nodeId);
                    }
                    return new SVInt32(sValue.CompareTo(i) < 0 ? 1 : 0);
                case "<=":
                    if (args.list.Count != 1)
                    {
                        throw new Exception("Invalid number of arguments for oepartor at " + args.nodeId);
                    }
                    return new SVInt32(sValue.CompareTo(i) <= 0 ? 1 : 0);
                case ">":
                    if (args.list.Count != 1)
                    {
                        throw new Exception("Invalid number of arguments for oepartor at " + args.nodeId);
                    }
                    return new SVInt32(sValue.CompareTo(i) > 0 ? 1 : 0);
                case ">=":
                    if (args.list.Count != 1)
                    {
                        throw new Exception("Invalid number of arguments for oepartor at " + args.nodeId);
                    }
                    return new SVInt32(sValue.CompareTo(i) >= 0 ? 1 : 0);
                case "=":
                case "set":
                    if (args.list.Count != 1)
                    {
                        throw new Exception("Invalid number of arguments for oepartor at " + args.nodeId);
                    }
                    if (args.list[0] is SVNull)
                    {
                        sValue = null;
                    }
                    else
                    {
                        sValue = i;
                    }
                    return this;
            }

            return base.ExecuteMethod(parent, space, method, args);
        }

    }
}
