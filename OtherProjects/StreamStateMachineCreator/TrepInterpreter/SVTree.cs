using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVTree: SValue
    {
        public string Name = string.Empty;
        public Dictionary<string, string> map = new Dictionary<string, string>();
        public SVList children;

        public override bool getBoolValue()
        {
            return false;
        }

        public override char getCharValue()
        {
            return '\0';
        }

        public override double getDoubleValue()
        {
            return 0.0;
        }

        public override int getIntValue()
        {
            return 0;
        }

        public override long getLongValue()
        {
            return 0;
        }

        public override string getStringValue()
        {
            return "";
        }

        public override SValue CreateInstance(List<SValue> args)
        {
            SVTree mp = new SVTree();
            if (args.Count > 0)
                Name = args[0].getStringValue();
            for (int i = 1; i < args.Count; i += 2)
            {

                if (i + 1 >= args.Count)
                {
                    if (!mp.map.ContainsKey(args[i].getStringValue()))
                        mp.map.Add(args[i].getStringValue(), string.Empty);
                }
                else
                {
                    mp.map.Add(args[i].getStringValue(), args[i + 1].getStringValue());
                }
            }

            return base.CreateInstance(args);
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            string key;

            switch (method)
            {
                case "getName":
                    return new SVString(Name);
                case "setName":
                    Name = args.getStringValue(0);
                    return this;
                case "hasAttribute":
                    return new SVBoolean(map.ContainsKey(args.getStringValue(0)));
                case "getAttribute":
                    return new SVString(map[args.getStringValue(0)]);
                case "setAttribute":
                    args.AssertMinCount(2);
                    args.AssertMaxCount(2);
                    key = args.getStringValue(0);
                    if (map.ContainsKey(key))
                        map[key] = args.list[1].getStringValue();
                    else
                        map.Add(key, args.list[1].getStringValue());
                    return this;
                case "attributesCount":
                    return new SVInt32(map.Count);
                case "childrenCount":
                    {
                        if (children == null)
                            return new SVInt32(0);
                        else
                            return new SVInt32(children.list.Count);
                    }
                default:
                    if (children != null)
                    {
                        children.ExecuteMethod(parent, space, method, args);
                    }
                    break;

            }

            return base.ExecuteMethod(parent, space, method, args);
        }
    }
}
