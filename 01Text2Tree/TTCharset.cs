using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Text2Tree
{
    /// <summary>
    /// Defines mutable charset with possibility to add characters into definition of charset.
    /// </summary>
    public class TTCharset
    {
        public static int PAGE_SIZE = 128;

        public string Name = string.Empty;
        private Dictionary<int, bool[]> pages = new Dictionary<int,bool[]>();
        private int lastPageId = -1;
        private bool[] lastPage = null;

        /// <summary>
        /// This Inverted property enables to use inverse charset definition.
        /// If Inverse = false, then we have to enumerate explicitly all characters
        ///  that are part of charet.
        /// If Inverse = true, then by default all characters are part of given
        ///  charset, and we have to explicitly state which characters are excluded.
        /// </summary>
        public bool Inverted = false;

        public TTCharset()
        {
        }

        public TTCharset(string sName)
        {
            Name = sName;
        }

        public TTCharset(bool bInverse)
        {
            Inverted = bInverse;
        }

        private bool[] getPage(int page)
        {
            if (pages.ContainsKey(page))
            {
                return pages[page];
            }
            return null;
        }

        private bool[] getSafePage(int page)
        {
            if (page == lastPageId)
                return lastPage;

            if (pages.ContainsKey(page))
            {
                lastPageId = page;
                lastPage = pages[page];
                return lastPage;
            }

            bool[] p = new bool[PAGE_SIZE];
            for (int i = 0; i < PAGE_SIZE; i++)
            {
                p[i] = false;
            }
            pages.Add(page, p);
            lastPageId = page;
            lastPage = p;

            return p;
        }

        public void addChars(int i)
        {
            bool[] b = getSafePage(i / PAGE_SIZE);
            b[i % PAGE_SIZE] = true;
        }

        public void addChars(char c)
        {
            addChars(Convert.ToInt32(c));
        }

        public void addChars(string str)
        {
            foreach(char c in str)
            {
                addChars(Convert.ToInt32(c));
            }
        }

        public void addChars(params string [] arg)
        {
            foreach (string s in arg)
            {
                foreach (char c in s)
                {
                    addChars(Convert.ToInt32(c));
                }
            }
        }

        public void addRange(char a, char b)
        {
            int A = Convert.ToInt32(a);
            int B = Convert.ToInt32(b);

            for(int i = A; i <= B; i++)
            {
                addChars(i);
            }
        }

        /// <summary>
        /// Checks if given character is member of this charset
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool ContainsChar(char c)
        {
            int i = Convert.ToInt32(c);
            bool[] b = getPage(i / PAGE_SIZE);
            if (b == null)
                return Inverted;
            return b[i % PAGE_SIZE] ? !Inverted : Inverted;
        }
    }
}
