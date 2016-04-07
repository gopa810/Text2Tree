using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace StreamStateMachineCreator
{
    public class SMScriptDelegate: ScriptingDelegate
    {
        public Scripting CurrentScript = null;
        public ScriptingSpace CurrentScriptSpace = null;

        public SVChar readedChar = new SVChar();
        public SVString readedString = new SVString();
        public SVInt32 currStateId = new SVInt32();

        public SMScriptDelegate()
        {
            CurrentScript = new Scripting();
            CurrentScriptSpace = new ScriptingSpace();

            // initialization of variables in script
            CurrentScript.PrepareExecution(this, CurrentScriptSpace);
        }

        public char ReadedChar
        {
            get { return readedChar.nValue; }
            set { readedChar.nValue = value; }
        }

        public string ReadedString
        {
            get { return readedString.sValue; }
            set { readedString.sValue = value; }
        }

        public int CurrentStateId
        {
            get { return currStateId.nValue; }
            set { currStateId.nValue = value; }
        }

        public SValue ExecuteNode(SValue node)
        {
            return CurrentScript.ExecuteNode(CurrentScriptSpace, node);
        }

        public void DefineVariable(SMVariable sv)
        {
            if (sv.DataType == "string")
            {
                if (sv.InitialValue == null)
                    CurrentScriptSpace.SetValue(sv.Variable, new SVString(""));
                else
                    CurrentScriptSpace.SetValue(sv.Variable, new SVString(sv.InitialValue));
            }
            else if (sv.DataType == "int")
            {
                int val = 0;
                if (sv.InitialValue != null)
                    int.TryParse(sv.InitialValue, out val);
                CurrentScriptSpace.SetValue(sv.Variable, new SVInt32(val));
            }
            else if (sv.DataType == "long")
            {
                long val = 0;
                if (sv.InitialValue != null)
                    long.TryParse(sv.InitialValue, out val);
                CurrentScriptSpace.SetValue(sv.Variable, new SVInt64(val));
            }
            else if (sv.DataType == "array")
            {
                CurrentScriptSpace.SetValue(sv.Variable, new SVList());
            }
            else if (sv.DataType == "dictionary")
            {
                CurrentScriptSpace.SetValue(sv.Variable, new SVMap());
            }
            else if (sv.DataType == "tree")
            {
                CurrentScriptSpace.SetValue(sv.Variable, new SVTree());
            }
            else if (sv.DataType == "StringBuilder")
            {
                CurrentScriptSpace.SetValue(sv.Variable, new SVStringBuilder());
            }
        }

        public override void RegisterDataTypes(ScriptingSpace aSpace)
        {
            aSpace.AddDatatype("array", new SVList());
            aSpace.AddDatatype("dictionary", new SVMap());
            aSpace.AddDatatype("tree", new SVTree());
            aSpace.AddDatatype("StringBuilder", new SVStringBuilder());

            base.RegisterDataTypes(aSpace);
        }

        public override void RegisterGlobalVariables(ScriptingSpace aSpace)
        {
            aSpace.SetValue("RC", readedChar);
            aSpace.SetValue("RS", readedString);
            aSpace.SetValue("STATE", currStateId);

            base.RegisterGlobalVariables(aSpace);
        }
    }
}
