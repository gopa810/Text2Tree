using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class ScriptingDefun
    {
        public string Name = string.Empty;
        public List<string> Args = new List<string>();
        public List<string> ArgTypes = new List<string>();
        public SValue Body = null;
    }
}
