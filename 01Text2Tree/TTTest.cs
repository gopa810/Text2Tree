using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

using TextTreeParser;
using TrepInterpreter;

namespace Text2Tree
{
    public class TTTest
    {
        public string inputFile = "";

        public void print(params object [] args)
        {
            foreach(object s in args)
            {
                Debugger.Log(0,"",s.ToString());
            }
        }

        public void println(params object[] args)
        {
            foreach (object s in args)
            {
                Debugger.Log(0, "", s.ToString());
            }
            Debugger.Log(0, "", "\n");
        }

        /*public void print(ParserResult pr)
        {
            if (pr == null)
            {
                println("(parser is null)");
            }
            else
            {
                println("Parser:");
                println("   matchedString: ", pr.matchedString.ToString());
            }
        }*/

        public void print(TTAtomList pr)
        {
            TTAtom cn = pr.first;
            while (cn != null)
            {
                println("  ATOM[", cn.Type, "] = ", cn.Value);
                cn = cn.next;
            }
        }

        public void print(TTNode pr, int level)
        {
            if (pr == null)
            {
                for (int i = 0; i < level; i++)
                    print("  ");
                println("TreeNode NULL");
            }
            else
            {
                for (int i = 0; i < level; i++)
                    print("  ");
                println("TreeNode [", pr.Name, "] = ", pr.Value);
            }
        }

        public void print(TTNode pr)
        {
            print(pr, 0);
        }

        public void startFuncLog(string f)
        {
            println("******************************************");
            println("* ", f, "  *");
            println("******************************************");
        }

        public void stopFuncLog()
        {
            println("******************************************");
            println("");
        }

        public void main(string textToParse, TreeView treeView1, TreeView treeView2)
        {
            //testInputTextFile();
            testParser2(textToParse, treeView1, treeView2);
        }

        public void testParser()
        {
            /*
            startFuncLog("Parser");
            string file = "abcdef";
            TTInputTextFile ip = new TTInputTextFile();
            ip.setContentString(file);

            TTParser par = new TTParser();
            //ParserResult res = null;

            par.Max("ab", "abc", "abcd", "abs");
            par.First("ea", "er", "e", "ef");
            TTAtomList tn = new TTAtomList();
            par.ParseAtomList(ip, tn);
            //res = par.ParseObject("abcd", ip);
            //print(res);
            //res = par.ParseObject("ef", ip);
            //print(res);
            TTAtom res2 = tn.first;
            while (res2 != null)
            {
                print(res2);
                res2 = res2.next;
            }

            stopFuncLog();
            */
        }

        public void testParser2(string textToParse, TreeView tv, TreeView restv)
        {
            startFuncLog("Parser");
            /*
            VAR CHARSET newline;
            VAR PATTERN pat2;

            [pat2 SETMETHOD FIRST];
            [pat3 ADDSTRING 1 1 'func'];
            
            func main {
               [patMain exec];
            }

             */
            inputFile = textToParse;
//            inputFile = "VAR CHARSET newline;\r\n [newline addchars 'abcdef' [charset default]];\r\n-10.289829F + 20; /*parpar 1.67E67*/ \r\n-.278E+0718 \"string\\\" here\" // commnet\n here ";
            TTInputTextFile ip = new TTInputTextFile();
            ip.setContentString(inputFile);

            TLanguageGScript script = new TLanguageGScript();

            script.Initialize();

            script.resultTree = GetNodeB(Scripting.ParseText(textToParse));

            /*try
            {
                if (script.Run(ip))
                {
                    println(" * * * ATOM LIST * * *");
                    print(script.atomList);
                    println(" * * * TREE * * *");
                    print(script.resultTree);
                }
            }
            catch (Exception ex)
            {
                script.errorLog.FinalMessage = ex.Message + "\r\n" + ex.StackTrace + "\r\n\r\n";
            }

            println(" * * * LOG * * *");
            println(script.errorLog.ToString());
            */
            if (tv != null)
            {
                TTErrorLog.Shared.validateSuccess();

                TreeNode tn = GetNode(TTErrorLog.Shared.Root);
                tv.Nodes.Add(tn);
            }

            if (restv != null)
            {
                TreeNode tn = GetNodeA(script.resultTree);
                restv.Nodes.Add(tn);
            }

            stopFuncLog();
        }

        internal TreeNode GetNode(TTErrorLog.TreeItem node)
        {
            TreeNode tn = new TreeNode();
            tn.Text = node.Name;
            tn.ImageIndex = tn.SelectedImageIndex = (node.Success ? 1 : 0);
            tn.Tag = node;
            if (node.kids != null)
            {
                foreach (TTErrorLog.TreeItem ti in node.kids)
                {
                    TreeNode tn2 = GetNode(ti);
                    tn.Nodes.Add(tn2);
                }
            }

            return tn;
        }

        internal TreeNode GetNodeA(TTNode node)
        {
            TreeNode tn = new TreeNode();
            tn.Text= (node.Name == null ? "(NULL)" : node.Name) + " := " + (node.Value == null ? "(NULL)" : node.Value);
            tn.Tag = node;
            foreach(TTNode ti in node.Children)
            {
                TreeNode tn2 = GetNodeA(ti);
                tn.Nodes.Add(tn2);
            }

            return tn;
        }

        internal TTNode GetNodeB(SValue sv)
        {
            if (sv is SVList)
            {
                SVList sl = sv as SVList;
                TTNode tn = new TTNode("list #" + sl.nodeId);
                foreach (SValue s1 in sl.list)
                {
                    tn.addSubnode(GetNodeB(s1));
                }
                return tn;
            }
            else if (sv is SVInt32)
            {
                SVInt32 si = sv as SVInt32;
                TTNode tn = new TTNode("int #" + sv.nodeId);
                tn.Value = si.nValue.ToString();
                return tn;
            }
            else if (sv is SVToken)
            {
                SVToken st = sv as SVToken;
                TTNode tn = new TTNode("token #" + st.nodeId);
                tn.Value = st.sValue;
                return tn;
            }
            else if (sv is SVString)
            {
                SVString ss = sv as SVString;
                TTNode tn = new TTNode("string #" + ss.nodeId);
                tn.Value = ss.sValue;
                return tn;
            }
            else if (sv is SVInt64)
            {
                SVInt64 sl = sv as SVInt64;
                TTNode tn = new TTNode("long #" + sl.nodeId);
                tn.Value = sl.lValue.ToString();
                return tn;
            }
            else if (sv is SVDouble)
            {
                SVDouble sd = sv as SVDouble;
                TTNode tn = new TTNode("double #" + sd.nodeId);
                tn.Value = sd.dValue.ToString();
                return tn;
            }

            // should not happen this
            return new TTNode("N/A");
        }


        public void testInputTextFile()
        {
            startFuncLog("InputTextFile");
            string file = "\nabc\n this is a now\n\nnext line\n";
            TTInputTextFile ip = new TTInputTextFile();
            ip.setContentString(file);


            while (ip.canRead())
            {
                println("Char: ", ip.getChar().c, ", poss ", ip.next.position, " line: ", ip.next.lineNo, " linpos: ", ip.next.linePos);
                if (ip.next.position == 10)
                    ip.pushState();
            }

            stopFuncLog();
            ip.popState();
            while (ip.canRead())
            {
                println("Char: ", ip.getChar().c, ", poss ", ip.next.position, " line: ", ip.next.lineNo, " linpos: ", ip.next.linePos);
            }


            stopFuncLog();
        }
    }
}
