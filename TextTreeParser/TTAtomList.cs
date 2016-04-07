using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace TextTreeParser
{
    public class TTAtomList: SValue
    {
        private TTAtom firstItem = null;
        private TTAtom lastItem = null;

        public TTAtom current = null;


        public override SValue CreateInstance(List<SValue> args)
        {
            return new TTAtomList();
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("first"))
            {
                return first;
            }
            else if (method.Equals("last"))
            {
                return last;
            }
            else if (method.Equals("current"))
            {
                return current;
            }
            else if (method.Equals("addItem"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTAtom));
                addItem(args.list[0] as TTAtom);
                return args.list[0];
            }
            else if (method.Equals("getAtom"))
            {
                return getAtom();
            }
            else if (method.Equals("removeAtomsWithType"))
            {
                args.AssertMinCount(1);
                foreach (SValue sv in args.list)
                {
                    removeAtomsWithType(sv.getStringValue());
                }
                return space.nullValue;
            }

            return base.ExecuteMethod(parent, space, method, args);
        }

        public TTAtom first
        {
            get
            {
                return firstItem;
            }
        }

        public TTAtom last
        {
            get
            {
                return lastItem;
            }
        }

        public void addItem(TTAtom atom)
        {
            if (firstItem == null)
            {
                firstItem = atom;
                lastItem = atom.last;
                current = atom;
            }
            else
            {
                lastItem.next = atom;
                lastItem = atom.last;
            }
        }

        public TTAtom getAtom()
        {
            TTAtom atom = this.current;
            current = current.next;
            return atom;
        }


        public void removeAtomsWithType(string p)
        {
            TTAtom iter = this.firstItem;

            while (iter != null && iter.Type.Equals(p))
            {
                iter = iter.next;
                this.firstItem.next = null;
                this.firstItem = iter;
            }

            while (iter != null)
            {
                if (iter.next != null && iter.next.Type.Equals(p))
                {
                    iter.next = iter.next.next;
                }
                else
                {
                    iter = iter.next;
                }
            }
        }
    }
}
