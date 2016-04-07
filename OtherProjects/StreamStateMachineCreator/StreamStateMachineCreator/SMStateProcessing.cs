using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace StreamStateMachineCreator
{
    public class SMStateProcessing
    {
        public string Comparer { get; set; }
        public string Values { get; set; }
        public string Actions { get; set; }
        public string NextState { get; set; }
        public bool ReprocessInput { get; set; }

        public SValue CompiledActions { get; set; }
        public int CompiledNextState { get; set; }

        internal void Compile()
        {
            CompiledActions = Scripting.ParseText(Actions);
        }

        public bool IsCharAcceptable(char c)
        {
            return false;
        }

        public bool IsStringAcceptable(string s)
        {
            return false;
        }
    }
}
