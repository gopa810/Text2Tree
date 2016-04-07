using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVStringBuilder: SValue
    {
        public StringBuilder Builder = new StringBuilder();
        public SVStringBuilder() { }

        public override bool getBoolValue()
        {
            String s = Builder.ToString();
            int i;
            bool b;

            if (int.TryParse(s, out i))
                return Convert.ToBoolean(i);
            if (bool.TryParse(s, out b))
                return b;
            return false;
        }

        public override char getCharValue()
        {
            if (Builder.Length > 0)
                return Builder[0];
            return '\0';
        }

        public override double getDoubleValue()
        {
            double d = 0.0;
            if (double.TryParse(Builder.ToString(), out d))
                return d;
            return d;
        }

        public override int getIntValue()
        {
            String s = Builder.ToString();
            int i;

            if (int.TryParse(s, out i))
                return i;
            return i;
        }

        public override long getLongValue()
        {
            String s = Builder.ToString();
            long i;

            if (long.TryParse(s, out i))
                return i;

            return i;
        }

        public override string getStringValue()
        {
            return Builder.ToString();
        }

        public override SValue CreateInstance(List<SValue> args)
        {
            return new SVStringBuilder();
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("append"))
            {
                foreach (SValue sv in args.list)
                {
                    Builder.Append(sv.getStringValue());
                }
            }
            else if (method.Equals("clear"))
            {
                Builder.Clear();
            }
            return base.ExecuteMethod(parent, space, method, args);
        }
    }
}
