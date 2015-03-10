using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextTreeParser;

namespace Text2Tree
{
    public class TLanguageBase
    {
        public TTAtomList atomList;
        public TTTreeNode resultTree;
        public TTErrorLog errorLog;
        public Dictionary<string, TTCharset> charsets = new Dictionary<string, TTCharset>();
        public Dictionary<string, TTPattern> lexicalPatterns = new Dictionary<string, TTPattern>();
        public Dictionary<string, TTPattern> syntaxPatterns = new Dictionary<string, TTPattern>();
        public TTParser mainParser;
        public TTParser syntaxParser;


        public virtual void Initialize()
        {
            errorLog = new TTErrorLog();
            mainParser = new TTParser();
            syntaxParser = new TTParser();
            atomList = new TTAtomList();
            resultTree = new TTTreeNode();
        }

        public virtual bool Run(TTInputTextFile ip)
        {
            return false;
        }

        public TTCharset getCharset(string name)
        {
            if (charsets.ContainsKey(name))
                return charsets[name];
            TTCharset cs = new TTCharset(name);
            charsets.Add(name, cs);
            return cs;
        }

        public TTPattern getLexicalPattern(string name)
        {
            if (lexicalPatterns.ContainsKey(name))
                return lexicalPatterns[name];
            TTPattern pt = new TTPattern(name);
            lexicalPatterns.Add(name, pt);
            return pt;
        }
        public TTPattern getSyntaxPattern(string name)
        {
            if (syntaxPatterns.ContainsKey(name))
                return syntaxPatterns[name];
            TTPattern pt = new TTPattern(name);
            syntaxPatterns.Add(name, pt);
            return pt;
        }

        protected void addLastAtom(string tt, string ta)
        {
            if (atomList.last != null)
            {
                TTAtom eof = new TTAtom(tt, ta);
                eof.startPos.position = atomList.last.endPos.position + 1;
                eof.endPos.position = eof.startPos.position + 1;
                atomList.addItem(eof);
            }
        }

    }
}
