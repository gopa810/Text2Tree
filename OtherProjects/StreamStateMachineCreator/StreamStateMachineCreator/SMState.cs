using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamStateMachineCreator
{
    public class SMState
    {
        public int StateId { get; set; }
        public string Name { get; set; }
        public List<SMStateProcessing> Process { get; set; }

        public SMState()
        {
            StateId = -1;
            Name = string.Empty;
            Process = new List<SMStateProcessing>();
        }
    }
}
