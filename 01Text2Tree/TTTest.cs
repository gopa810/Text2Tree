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
            foreach (TTTreeNode cn in pr.Subnodes)
            {
                print(cn, level + 1);
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
            foreach(TTTreeNode res2 in tn.Subnodes)
            {
                print(res2);
            }
            stopFuncLog();
        }

        public void testParser2()
        {
            startFuncLog("Parser");

            TTInputTextFile ip = new TTInputTextFile();
            ip.setContentString("-10.289829F  parpar");

            TTCharset csAlpha = new TTCharset("alpha");
            csAlpha.addRange('a', 'z');
            csAlpha.addRange('A', 'Z');

            TTCharset csDigit = new TTCharset("digit");
            csDigit.addRange('0', '9');

            TTCharset csNewlineSpaces = new TTCharset("whitespaceNewline");
            csNewlineSpaces.addChars(" \t\n\r");

            TTCharset csWhitespace = new TTCharset("whitespace");
            csWhitespace.addChars(" \t");

            TTCharset csNewline = new TTCharset("newline");
            csNewline.addChars("\r\n");

            TTCharset csIdent1 = new TTCharset("ident1");
            csIdent1.addRange('a', 'z');
            csIdent1.addRange('A', 'Z');
            csIdent1.addChars('_');

            TTCharset csIdent2 = new TTCharset("ident2");
            csIdent2.addRange('a', 'z');
            csIdent2.addRange('A', 'Z');
            csIdent2.addRange('0', '9');
            csIdent2.addChars('_');

            /*TTParser par = new TTParser();
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
            }*/


            TTPattern pFloat = new TTPattern("float");
            pFloat.addChars(0, 1, "+-");
            pFloat.addCharset(0, 1000, csDigit);
            pFloat.addChar(1, 1, '.');
            pFloat.addCharset(0, 1000, csDigit);
            pFloat.addChars(0, 1, "fF");

            TTPattern pIdent = new TTPattern("identifier");
            pIdent.addCharset(1, 1, csIdent1);
            pIdent.addCharset(0, 1000, csIdent2);

            TTPattern pWs = new TTPattern("ws");
            pWs.addCharset(1, 1000000, csNewlineSpaces);


            TTParser par = new TTParser();
            par.Max(pFloat, pIdent, pWs);

            TTParser par2 = new TTParser();
            par2.List(par);

            TTTreeNode tn = new TTTreeNode();
            if (par2.Run(ip, tn))
            {
                print(tn);
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
