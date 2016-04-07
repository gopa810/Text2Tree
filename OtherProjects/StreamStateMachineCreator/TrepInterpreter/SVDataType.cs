using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVDataType : SValue
    {
        public string dataTypeName = string.Empty;
        public SValue dataType = null;

        public override SValue CreateInstance(List<SValue> args)
        {
            return dataType.CreateInstance(args);
        }

    }
}
