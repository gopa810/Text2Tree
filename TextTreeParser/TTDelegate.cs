using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace TextTreeParser
{
    public class TTDelegate: ScriptingDelegate
    {
        public override void RegisterDataTypes(ScriptingSpace aSpace)
        {
            aSpace.AddDatatype("InputTextFile", new TTInputTextFile());
            aSpace.AddDatatype("TextPosition", new TTInputTextFile.TextPositionObject());
            aSpace.AddDatatype("CharEntry", new TTInputTextFile.CharEntryObject());
            aSpace.AddDatatype("Atom", new TTAtom());
            aSpace.AddDatatype("AtomList", new TTAtomList());
            aSpace.AddDatatype("Node", new TTNode());
            aSpace.AddDatatype("NodeCollection", new TTNodeCollection());
            aSpace.AddDatatype("ErrorLog", new TTErrorLog());
            aSpace.AddDatatype("Charset", new TTCharset());
            aSpace.AddDatatype("ParserAtom", new TTParserAtom());
            aSpace.AddDatatype("NamedChar", new TTNamedChar());
            aSpace.AddDatatype("NamedString", new TTNamedString());
            aSpace.AddDatatype("Pattern", new TTPattern());

            base.RegisterDataTypes(aSpace);
        }

        public override SValue createInstance(Scripting parent, string dataTypeName, List<SValue> optArgs)
        {
            return base.createInstance(parent, dataTypeName, optArgs);
        }

        public override SValue ExecuteTokenList(Scripting parent, ScriptingSpace space, SVList node)
        {
            return base.ExecuteTokenList(parent, space, node);
        }

        public override void RegisterGlobalVariables(ScriptingSpace aSpace)
        {
            base.RegisterGlobalVariables(aSpace);

            aSpace.SetValue("METHOD_ALL_SERIAL", new SVInt32(0));
            aSpace.SetValue("METHOD_FIRST", new SVInt32(1));
            aSpace.SetValue("METHOD_MAX", new SVInt32(2));
            aSpace.SetValue("METHOD_LIST", new SVInt32(3));

            aSpace.SetValue("null", space.nullValue);
            aSpace.SetValue("true", new SVBoolean(true));
            aSpace.SetValue("false", new SVBoolean(false));

            aSpace.SetValue("MAXINT", new SVInt32(Int32.MaxValue));
            aSpace.SetValue("MININT", new SVInt32(Int32.MinValue));
        }
    }
}
