using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVList : SValue
    {
        public bool Evaluable = true;
        public List<SValue> list = new List<SValue>();

        public SVList()
        {
        }

        public SVList(List<SValue> l)
        {
            list.AddRange(l);
        }

        public SValue getHead()
        {
            if (list != null && list.Count > 0)
                return list[0];
            return null;
        }

        public void AssertMinCount(int count)
        {
            if (count > list.Count)
                throw new Exception("Expected at least " + count + " items in list at " + nodeId);
        }

        public void AssertMaxCount(int count)
        {
            if (count < list.Count)
                throw new Exception("Expected at most " + count + " items in list at " + nodeId);
        }

        public void AssertCount(int count)
        {
            if (count != list.Count)
                throw new Exception("Expected at least " + count + " items in list at " + nodeId);
        }

        public void AssertIsNumber(int index)
        {
            if (!(list[index] is SVNumber))
                throw new Exception("Expected number at index " + index + " in the list at " + nodeId);
        }

        public void AssertIsType(int index, Type tt)
        {
            if (! list[index].GetType().Equals(tt))
                throw new Exception("Expected type " + tt.Name + " at index " + index + " in the list at " + nodeId);
        }

        public override int ReorderIds(int i)
        {
            int n = base.ReorderIds(i);
            if (list != null)
            {
                foreach (SValue sv in list)
                {
                    n = sv.ReorderIds(n);
                }
            }
            return n;
        }

        public SVList SublistFrom(int i)
        {
            SVList sl = new SVList();
            for (int j = i; j < list.Count; j++)
            {
                sl.list.Add(list[j]);
            }
            sl.nodeId = nodeId;
            return sl;
        }

        public override SValue CreateInstance(List<SValue> args)
        {
            if (args.Count > 0)
            {
                return new SVList(args);
            }
            else
            {
                return new SVList();
            }
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            switch (method)
            {
                case "count":
                    return new SVInt32(list.Count);
                case "first":
                    if (list.Count > 0)
                        return list[0];
                    else
                        return space.nullValue;
                case "rest":
                    return SublistFrom(1);
                default:
                    break;
            }
            return base.ExecuteMethod(parent, space, method, args);
        }

        public int getIntValue(int a)
        {
            return list[a].getIntValue();
        }

        public string getStringValue(int a)
        {
            if (list[a] is SVNull)
                return null;
            return list[a].getStringValue();
        }

        public char getCharValue(int a)
        {
            return list[a].getCharValue();
        }
    }

}
