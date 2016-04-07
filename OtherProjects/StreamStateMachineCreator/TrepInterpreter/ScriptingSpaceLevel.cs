using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class ScriptingSpaceLevel
    {
        public Dictionary<string, SValue> vmap = new Dictionary<string, SValue>();
        public Dictionary<string, ScriptingDefun> fmap = new Dictionary<string, ScriptingDefun>();

        public bool ContainsObject(string s)
        {
            return vmap.ContainsKey(s);
        }

        public SValue GetValue(string s)
        {
            return vmap[s];
        }

        public bool ContainsFunction(string s)
        {
            return fmap.ContainsKey(s);
        }

        public ScriptingDefun GetFunction(string s)
        {
            return fmap[s];
        }

        public void SetObjectValue(string s, SValue sv)
        {
            if (ContainsObject(s))
                vmap[s] = sv;
            else
                vmap.Add(s, sv);
        }
    }
}
