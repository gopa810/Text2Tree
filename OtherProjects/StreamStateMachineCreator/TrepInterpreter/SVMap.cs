using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVMap: SValue
    {
        public Dictionary<string, SValue> map = new Dictionary<string, SValue>();

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
            SVMap mp = new SVMap();
            for (int i = 0; i < args.Count; i += 2)
            {

                if (i + 1 >= args.Count)
                {
                    if (!mp.map.ContainsKey(args[i].getStringValue()))
                        mp.map.Add(args[i].getStringValue(), new SVString());
                }
                else
                {
                    mp.map.Add(args[i].getStringValue(), args[i+1]);
                }
            }

            return base.CreateInstance(args);
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            string key;

            switch (method)
            {
                case "containsKey":
                    return new SVBoolean(map.ContainsKey(args.getStringValue(0)));
                case "valueForKey":
                    return map[args.getStringValue(0)];
                case "setValueForKey":
                    args.AssertMinCount(2);
                    args.AssertMaxCount(2);
                    key = args.getStringValue(0);
                    if (map.ContainsKey(key))
                        map[key] = args.list[1];
                    else
                        map.Add(key, args.list[1]);
                    return this;
                case "count":
                    return new SVInt32(map.Count);
            }

            return base.ExecuteMethod(parent, space, method, args);
        }
    }
}
