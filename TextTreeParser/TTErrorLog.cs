using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace TextTreeParser
{
    public class TTErrorLog: SValue
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

            internal TreeItem getLastNode(string p)
            {
                TreeItem curr = null;
                TreeItem ti = null;

                if (kids != null)
                {
                    foreach (TreeItem k in kids)
                    {
                        curr = k.getLastNode(p);
                        if (curr != null)
                            ti = curr;
                    }
                }

                if (ti != null)
                    return ti;

                if (Name.Equals(p))
                    return this;

                return null;
            }
        }

        public TreeItem Root;
        public TreeItem Current;
        public string FinalMessage = "";
        public static TTErrorLog Shared = new TTErrorLog();

        public TTErrorLog()
        {
            Root = new TreeItem();
            Root.Name = "Parser Analysis";
            Current = Root;
        }


        public override SValue CreateInstance(List<SValue> args)
        {
            return new TTErrorLog();
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("finalMessage"))
            {
                return new SVString(FinalMessage);
            }
            else if (method.Equals("setFinalMessage"))
            {
                if (args.list.Count == 0)
                {
                    FinalMessage = string.Empty;
                }
                else
                {
                    FinalMessage = args.list[0].getStringValue();
                }
            }
            else if (method.Equals("addLog"))
            {
                if (args.list.Count == 0)
                {
                    addLog("\n", null);
                }
                else if (args.list.Count == 1)
                {
                    addLog(args.list[0].getStringValue(), null);
                }
                else
                {
                    string[] argsa = new string[args.list.Count - 1];
                    for (int i = 1; i < args.list.Count; i++)
                    {
                        argsa[i - 1] = args.list[i].getStringValue();
                    }
                    addLog(args.list[0].getStringValue(), argsa);
                }
            }
            else if (method.Equals("enterDir"))
            {
                if (args.list.Count == 1)
                {
                    enterDir(args.list[0].getStringValue(), new TextPosition());
                }
                else if (args.list.Count == 2)
                {
                    args.AssertIsType(1, typeof(TTInputTextFile.TextPositionObject));
                    enterDir(args.list[0].getStringValue(),
                        (args.list[1] as TTInputTextFile.TextPositionObject).pos);
                }
                return space.nullValue;
            }
            else if (method.Equals("addDir"))
            {
                if (args.list.Count == 1)
                {
                    addDir(args.list[0].getStringValue(), true, new TextPosition());
                }
                else if (args.list.Count == 2)
                {
                    addDir(args.list[0].getStringValue(), args.list[1].getBoolValue(), new TextPosition());
                }
                else if (args.list.Count == 3)
                {
                    addDir(args.list[0].getStringValue(), 
                        args.list[1].getBoolValue(), 
                        (args.list[2] as TTInputTextFile.TextPositionObject).pos);
                }
                return space.nullValue;
            }
            else if (method.Equals("goUp"))
            {
                goUp();
                return space.nullValue;
            }
            else if (method.Equals("validateSuccess"))
            {
                validateSuccess();
                return space.nullValue;
            }
            else if (method.Equals("resolveLastError"))
            {
                resolveLastError();
                return space.nullValue;
            }

            return base.ExecuteMethod(parent, space, method, args);
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

        public void resolveLastError()
        {
            TreeItem lastSerial = null;
            TreeItem ti = this.Root;

            lastSerial = this.Root.getLastNode("#serial");

            if (lastSerial != null && lastSerial.kids != null && lastSerial.kids.Count > 0)
            {
                ti = lastSerial.kids[lastSerial.kids.Count - 1];
            }

            if (ti.Success == false)
            {
                this.FinalMessage += string.Format("Expected {0} / {1} at line {2}, position {3}", ti.Name, ti.Value, ti.pos.lineNo, ti.pos.linePos);
            }
        }

    }
}
