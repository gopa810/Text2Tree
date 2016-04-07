using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SValue
    {
        public bool shouldExit = false;
        public bool shouldBreak = false;
        public int nodeId = 0;
        
        // type conversions
        public virtual string getStringValue() { return string.Empty; }
        public virtual bool getBoolValue() { return false; }
        public virtual char getCharValue() { return '\0'; }
        public virtual Int32 getIntValue() { return 0; }
        public virtual Int64 getLongValue() { return 0L; }
        public virtual double getDoubleValue() { return 0.0; }


        public virtual SValue CreateInstance(List<SValue> args) 
        {
            return new SValue(); 
        }

        public virtual SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("nodeId"))
            {
                return new SVInt32(nodeId);
            }
            else if (method.Equals("intValue"))
            {
                return new SVInt32(getIntValue());
            }
            else if (method.Equals("stringValue"))
            {
                return new SVString(getStringValue());
            }
            else if (method.Equals("boolValue"))
            {
                return new SVBoolean(getBoolValue());
            }
            else if (method.Equals("longValue"))
            {
                return new SVInt64(getLongValue());
            }
            else if (method.Equals("doubleValue"))
            {
                return new SVDouble(getDoubleValue());
            }
            else if (method.Equals("charValue"))
            {
                return new SVChar(getCharValue());
            }
            // returning null means no handling
            return null;
        }

        public virtual int ReorderIds(int i)
        {
            nodeId = i;
            return i++;
        }
    }

    /// <summary>
    /// Data type dedicated to representing of null value
    /// </summary>
    public class SVNull : SValue
    {
    }

}
