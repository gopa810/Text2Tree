using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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

        public void print(TTTreeNode pr)
        {
            if (pr == null)
            {
                println("(parser is null)");
            }
            else
            {
                println("Parser:");
                println("   matchedString: ", pr.Type, " value ", pr.Value);
            }
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
            foreach(TTTreeNode res2 in tn.Subnodes)
            {
                print(res2);
            }
            stopFuncLog();
        }

        public void testParser2()
        {
            startFuncLog("Parser");
            string file = "abcdef";
            TTInputTextFile ip = new TTInputTextFile();
            ip.setContentString(file);

            TTCharset tcs = new TTCharset();
            tcs.addChars("abcxyz");

            TTCharset tcs2 = new TTCharset();
            tcs2.addChars("0123456789");

            TTCharset tcs3 = new TTCharset();
            tcs2.addChars("open");

            TTParser par = new TTParser();
            //ParserResult res = null;

            par.Max("ab", tcs, "abcud", "abs");
            par.First("ea", tcs2, tcs3, "ef");
            TTTreeNode tn = new TTTreeNode();
            if (par.Run(ip, tn))
            {
                //res = par.ParseObject("abcd", ip);
                //print(res);
                //res = par.ParseObject("ef", ip);
                //print(res);
                foreach (TTTreeNode res2 in tn.Subnodes)
                {
                    print(res2);
                }
            }
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
                println("Char: ", ip.getChar().c, ", poss ", ip.current.position, " line: ", ip.current.lineNo, " linpos: ", ip.current.linePos);
                if (ip.current.position == 10)
                    ip.pushState();
            }

            stopFuncLog();
            ip.popState();
            while (ip.canRead())
            {
                println("Char: ", ip.getChar().c, ", poss ", ip.current.position, " line: ", ip.current.lineNo, " linpos: ", ip.current.linePos);
            }


            stopFuncLog();
        }
    }
}
