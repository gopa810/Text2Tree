using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTreeParser
{
    public class TTAtomList
    {
        private TTAtom firstItem = null;
        private TTAtom lastItem = null;

        public TTAtom current = null;

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
