using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextTreeParser;

namespace Text2Tree
{
    public class TLanguageCPP: TLanguageBase
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        public override bool Run(TTInputTextFile ip)
        {
            bool result = false;
            atomList = new TTAtomList();
            resultTree = new TTTreeNode();
            resultTree.Name = "FILE";

            try
            {
                TTErrorLog.Shared.enterDir("syntax", new TextPosition());
                result = mainParser.ParseAtomList(ip, atomList);
                TTErrorLog.Shared.goUp();
                addLastAtom("EOF", "EOF");

                if (result)
                {
                    atomList.removeAtomsWithType("ws");
                    atomList.removeAtomsWithType("nl");
                    atomList.removeAtomsWithType("comment");
                    atomList.removeAtomsWithType("comment_inline");
                    atomList.current = atomList.first;

                    TTErrorLog.Shared.enterDir("semantics", new TextPosition());
                    syntaxParser.ParseTree(atomList, resultTree);
                    TTErrorLog.Shared.goUp();
                }
            }
            catch (Exception x)
            {
                errorLog.FinalMessage = x.Message + "\r\n" + x.StackTrace + "\r\n\r\n";
            }

            return result;
        }
    }
}
