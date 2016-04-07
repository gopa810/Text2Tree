using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace TextTreeParser
{
    /// <summary>
    /// Class for storing entity used for matching input list of atoms.
    /// In first step of file processing we are reading text file and comparing it with patterns and strings
    /// So matching objects are in that case patterns and strings
    /// In second step we are processing list of atoms (created from original input file)
    /// So matching object needs to store two values (TestType and TestValue) so we
    /// can check actual values of atom in the list (Type and Value)
    /// For this reason we have class TTParserAtom
    /// </summary>
    public class TTParserAtom : TTNamedObject
    {
        /// <summary>
        /// Type of atom
        /// </summary>
        public string TestType;

        /// <summary>
        /// Value of atom
        /// </summary>
        public string TestValue;

        /// <summary>
        /// default are null values, that means given parameter 
        /// is not used for matching in function Match
        /// </summary>
        public TTParserAtom()
        {
            TestType = null;
            TestValue = null;
        }

        public TTParserAtom(string s)
        {
            TestType = s;
            TestValue = null;
        }

        public TTParserAtom(string s, string v)
        {
            TestType = s;
            TestValue = v;
        }

        public override SValue CreateInstance(List<SValue> args)
        {
            if (args.Count == 1)
            {
                return new TTParserAtom(args[0].getStringValue());
            }
            else if (args.Count > 1)
            {
                return new TTParserAtom(args[0].getStringValue(), args[1].getStringValue());
            }

            return new TTParserAtom();
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("testType"))
            {
                return TestType == null ? space.nullValue : new SVString(TestType);
            }
            else if (method.Equals("testValue"))
            {
                return TestValue == null ? space.nullValue : new SVString(TestValue);
            }
            else if (method.Equals("setTestType"))
            {
                if (args.list.Count == 0)
                {
                    TestType = null;
                }
                else
                {
                    if (args.list[0] is SVNull)
                    {
                        TestType = null;
                    }
                    else
                    {
                        TestType = args.list[0].getStringValue();
                    }
                }
                return space.nullValue;
            }
            else if (method.Equals("setTestValue"))
            {
                if (args.list.Count == 0)
                {
                    TestValue = null;
                }
                else
                {
                    if (args.list[0] is SVNull)
                    {
                        TestValue = null;
                    }
                    else
                    {
                        TestValue = args.list[0].getStringValue();
                    }
                }
                return space.nullValue;
            }
            else if (method.Equals("match"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTAtom));
                return new SVBoolean(Match(args.list[0] as TTAtom));
            }

            return base.ExecuteMethod(parent, space, method, args);
        }

        /// <summary>
        /// Try to match ATOM received from input file, with this object.
        /// </summary>
        /// <param name="ce">Atom entry from input file</param>
        /// <returns>Returns true if this object's type and/or value equals atom's values</returns>
        public bool Match(TTAtom ce)
        {
            if (TestType != null)
            {
                if (!TestType.Equals(ce.Type))
                    return false;
            }
            if (TestValue != null)
            {
                if (!TestValue.Equals(ce.Value))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// For debugging we need dump of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}/{1}", (TestType != null ? TestType : ""), (TestValue != null ? TestValue : ""));
        }
    }


}
