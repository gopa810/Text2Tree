using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TextTreeParser;

namespace Text2Tree
{
    public class TTTest
    {
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

        public void print(ParserResult pr)
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
        }

        public void print(TTTreeNode pr, int level)
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
                println("TreeNode [", pr.Type, "] = ", pr.Value);
            }
            TTTreeNode cn = pr.firstChild;
            while (cn != null)
            {
                print(cn, level + 1);
                cn = cn.nextSibling;
            }
        }

        public void print(TTTreeNode pr)
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

        public void main()
        {
            //testInputTextFile();
            testParser2();
        }

        public void testParser()
        {
            startFuncLog("Parser");
            string file = "abcdef";
            TTInputTextFile ip = new TTInputTextFile();
            ip.setContentString(file);

            TTParser par = new TTParser();
            //ParserResult res = null;

            par.Max("ab", "abc", "abcd", "abs");
            par.First("ea", "er", "e", "ef");
            TTTreeNode tn = new TTTreeNode();
            bool prs = par.Run(ip, tn);
            //res = par.ParseObject("abcd", ip);
            //print(res);
            //res = par.ParseObject("ef", ip);
            //print(res);
            TTTreeNode res2 = tn.firstChild;
            while (res2 != null)
            {
                print(res2);
                res2 = res2.nextSibling;
            }

            stopFuncLog();
        }

        public void testParser2()
        {
            startFuncLog("Parser");

            TTInputTextFile ip = new TTInputTextFile();
            ip.setContentString("VAR CHARSET newline ; \r\n [newline addchars 'abcdef' [charset default]];\r\n-10.289829F  /*parpar 1.67E67*/ \r\n-.278E+0718 \"string\\\" here\" // commnet\n here ");

            TTScript script = new TTScript();

            script.Initialize();

            if (script.Run(ip))
            {
                print(script.resultTree);
            }

            println(script.errorLog.ToString());
            stopFuncLog();
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
