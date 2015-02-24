using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTreeParser
{
    public class TTErrorLog
    {
        public class TreeItem
        {
            public bool Success = false;
            public string Name;
            public StringBuilder Value = new StringBuilder();
            public TreeItem parent = null;
            public List<TreeItem> kids = null;
            public TextPosition pos;

            public bool validateSuccess()
            {
                bool temp = false;
                if (kids != null)
                {
                    Success = false;
                    foreach (TreeItem ti in kids)
                    {
                        temp = ti.validateSuccess();
                        if (temp)
                            Success = true;
                    }
                }
                return Success;
            }
        }

        public TreeItem Root;
        public TreeItem Current;

        public static TTErrorLog Shared = new TTErrorLog();

        public TTErrorLog()
        {
            Root = new TreeItem();
            Root.Name = "Parser Analysis";
            Current = Root;
        }

        public override string ToString()
        {
            return Current.Value.ToString();
        }

        public void addLog(string format, params object[] args)
        {
            Current.Value.AppendFormat(format, args);
        }

        public void enterDir(string dir, TextPosition pos)
        {
            TreeItem ti = new TreeItem();
            ti.Name = dir;
            ti.parent = Current;
            ti.pos = pos;
            if (Current.kids == null)
                Current.kids = new List<TreeItem>();
            Current.kids.Add(ti);
            Current = ti;
        }

        public TreeItem addDir(string dir, bool succ, TextPosition pos)
        {
            TreeItem ti = new TreeItem();
            ti.Name = dir;
            ti.parent = Current;
            ti.Success = succ;
            ti.pos = pos;
            if (Current.kids == null)
                Current.kids = new List<TreeItem>();
            Current.kids.Add(ti);
            return ti;
        }

        public void goUp()
        {
            if (Current.parent != null)
                Current = Current.parent;
        }

        public void validateSuccess()
        {
            Root.validateSuccess();
        }
    }
}
