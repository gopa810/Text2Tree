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
            TTCharset c;
            TTPattern p;

            c = getCharset("whitespace");
            c.addChars(" \t");

            c = getCharset("h-char");
            c.Inverted = true;
            c.addChars(">\r\n");

            c = getCharset("q-char");
            c.Inverted = true;
            c.addChars("\"\r\n");

            c = getCharset("digit");
            c.addRange('0', '9');

            c = getCharset("nonzero-digit");
            c.addRange('1', '9');

            c = getCharset("octal-digit");
            c.addRange('0', '7');

            c = getCharset("hexadecimal-digit");
            c.addRange('0', '9');
            c.addRange('a', 'f');
            c.addRange('A', 'F');

            c = getCharset("nondigit");
            c.addRange('a', 'z');
            c.addRange('A', 'Z');
            c.addChars('_');

            c = getCharset("sign");
            c.addChars("+-");

            c = getCharset("c-char");
            c.Inverted = true;
            c.addChars("\\\'\r\n");

            c = getCharset("s-char");
            c.Inverted = true;
            c.addChars("\\\"\r\n");

            c = getCharset("r-char");
            c.Inverted = true;
            c.addChars(")");

            c = getCharset("d-char");
            c.Inverted = true;
            c.addChars("\\\r\n()\f");

            p = getLexicalPattern("universal-character-name-1");
            p.addChar(1, 1, '\\');
            p.addChar(1, 1, 'u');
            p.addCharset(4, 4, getCharset("hexadecimal-digit"));

            p = getLexicalPattern("universal-character-name-2");
            p.addChar(1, 1, '\\');
            p.addChar(1, 1, 'U');
            p.addCharset(8, 8, getCharset("hexadecimal-digit"));

            p = getLexicalPattern("universal-character-name");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addPattern(1, 1, getLexicalPattern("universal-character-name-1"));
            p.addPattern(1, 1, getLexicalPattern("universal-character-name-2"));

            p = getLexicalPattern("header-name");
            p.matchingMethod = TTPattern.METHOD_FIRST;
            p.addPattern(1, 1, getLexicalPattern("header-name-h"));
            p.addPattern(1, 1, getLexicalPattern("header-name-q"));

            p = getLexicalPattern("header-name-h");
            p.addChar(1, 1, '<');
            p.addCharset(0, 1000, getCharset("h-char"));
            p.addChar(1, 1, '>');

            p = getLexicalPattern("header-name-q");
            p.addChar(1, 1, '"');
            p.addCharset(0, 1000, getCharset("q-char"));
            p.addChar(1, 1, '"');

            p = getLexicalPattern("identifier-nondigit");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addCharset(1, 1, getCharset("nondigit"));
            p.addPattern(1, 1, getLexicalPattern("universal-character-name"));

            p = getLexicalPattern("identifier-all-digit");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addCharset(1, 1, getCharset("nondigit"));
            p.addCharset(1, 1, getCharset("digit"));
            p.addPattern(1, 1, getLexicalPattern("universal-character-name"));

            p = getLexicalPattern("identifier");
            p.addPattern(1, 1, getLexicalPattern("identifier-nondigit"));
            p.addPattern(0, 10000, getLexicalPattern("identifier-all-digit"));


            p = getLexicalPattern("floating-literal-a");
            p.addPattern(1, 1, getLexicalPattern("fractional-constant"));
            p.addPattern(0, 1, getLexicalPattern("exponent-part"));
            p.addPattern(0, 1, getLexicalPattern("floating-suffix"));

            p = getLexicalPattern("floating-literal-b");
            p.addCharset(1, 1000, getCharset("digit"));
            p.addPattern(1, 1, getLexicalPattern("exponent-part"));
            p.addPattern(0, 1, getLexicalPattern("floating-suffix"));

            p = getLexicalPattern("fractional-constant");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addPattern(1, 1, getLexicalPattern("fractional-constant-a"));
            p.addPattern(1, 1, getLexicalPattern("fractional-constant-b"));

            p = getLexicalPattern("fractional-constant-a");
            p.addCharset(0, 1000, getCharset("digit"));
            p.addChar(1, 1, '.');
            p.addCharset(1, 1000, getCharset("digit"));

            p = getLexicalPattern("fractional-constant-b");
            p.addCharset(1, 1000, getCharset("digit"));
            p.addChar(1, 1, '.');

            p = getLexicalPattern("exponent-part");
            p.addChars(1, 1, "eE");
            p.addCharset(0, 1, getCharset("sign"));
            p.addCharset(1, 1000, getCharset("digit"));

            p = getLexicalPattern("floating-suffix");
            p.addChars(1, 1, "fFlL");

            p = getLexicalPattern("floating-literal");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addPattern(1, 1, getLexicalPattern("floating-literal-a"));
            p.addPattern(1, 1, getLexicalPattern("floating-literal-b"));

            p = getLexicalPattern("simple-escape-sequence");
            p.addChar(1, 1, '\\');
            p.addChars(1, 1, "\'\"\\?abfnrtv");

            p = getLexicalPattern("octal-escape-sequence");
            p.addChar(1, 1, '\\');
            p.addCharset(1, 3, getCharset("octal-digit"));

            p = getLexicalPattern("hexadecimal-escape-sequence");
            p.addChar(1, 1, '\\');
            p.addChar(1, 1, 'x');
            p.addCharset(1, 300, getCharset("hexadecimal-digit"));

            p = getLexicalPattern("escape-sequence");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addPattern(1, 1, getLexicalPattern("simple-escape-sequence"));
            p.addPattern(1, 1, getLexicalPattern("octal-escape-sequence"));
            p.addPattern(1, 1, getLexicalPattern("hexadecimal-escape-sequence"));

            p = getLexicalPattern("c-char");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addCharset(1, 1, getCharset("c-char"));
            p.addPattern(1, 1, getLexicalPattern("universal-character-name"));
            p.addPattern(1, 1, getLexicalPattern("escape-sequence"));

            p = getLexicalPattern("character-literal");
            p.addChars(0, 1, "uUL");
            p.addChar(1, 1, '\'');
            p.addPattern(0, 1000000, getLexicalPattern("c-char"));
            p.addChar(1, 1, '\'');

            p = getLexicalPattern("user-defined-character-literal");
            p.addPattern(1, 1, getLexicalPattern("character-literal"));
            p.addPattern(1, 1, getLexicalPattern("ud-suffix"));

            p = getLexicalPattern("ud-suffix");
            p.addPattern(1, 1, getLexicalPattern("identifier"));

            p = getLexicalPattern("encoding-prefix-u8");
            p.addChar(1, 1, 'u');
            p.addChar(1, 1, '8');

            p = getLexicalPattern("encoding-prefix");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addPattern(1, 1, getLexicalPattern("encoding-prefix-u8"));
            p.addChars(1, 1, "uUL");

            p = getLexicalPattern("s-char");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addCharset(1, 1, getCharset("s-char"));
            p.addPattern(1, 1, getLexicalPattern("universal-character-name"));
            p.addPattern(1, 1, getLexicalPattern("escape-sequence"));

            p = getLexicalPattern("string-literal-s");
            p.addPattern(0, 1, getLexicalPattern("encoding-prefix"));
            p.addChar(1, 1, '\"');
            p.addPattern(0, 1000000, getLexicalPattern("s-char"));
            p.addChar(1, 1, '\"');

            p = getLexicalPattern("string-literal-r");
            p.addPattern(0, 1, getLexicalPattern("encoding-prefix"));
            p.addChar(1, 1, 'R');
            p.addPattern(1, 1, getLexicalPattern("raw-string"));

            p = getLexicalPattern("raw-string");
            p.addChar(1, 1, '\"');
            p.addCharset(0, 1000000, getCharset("d-char"));
            p.addChar(1, 1, '(');
            p.addCharset(0, 1000000, getCharset("r-char"));
            p.addChar(1, 1, ')');
            p.addCharset(0, 1000000, getCharset("d-char"));
            p.addChar(1, 1, '\"');

            p = getLexicalPattern("ws");
            p.addCharset(1, 1000000, getCharset("whitespace"));

            p = getLexicalPattern("string-literal");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addPattern(1, 1, getLexicalPattern("string-prefix-s"));
            p.addPattern(1, 1, getLexicalPattern("string-prefix-r"));

            p = getLexicalPattern("user-defined-string-literal");
            p.addPattern(1, 1, getLexicalPattern("string-literal"));
            p.addPattern(1, 1, getLexicalPattern("ud-suffix"));

            p = getLexicalPattern("preprocessing-op-or-punc");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addString(1, 1, "%:%:");
            p.addString(1, 1, "%>");
            p.addString(1, 1, "%:");
            p.addChars(1, 2, "#");
            p.addString(1, 1, "<:");
            p.addString(1, 1, "<%");
            p.addString(1, 1, ":>");
            p.addString(1, 1, "...");
            p.addString(1, 1, "<<=");
            p.addString(1, 1, ">>=");
            p.addString(1, 1, "::");
            p.addString(1, 1, ".*");
            p.addString(1, 1, "+=");
            p.addString(1, 1, "-=");
            p.addString(1, 1, "*=");
            p.addString(1, 1, "/=");
            p.addString(1, 1, "%=");
            p.addString(1, 1, "^=");
            p.addString(1, 1, "&=");
            p.addString(1, 1, "<<");
            p.addString(1, 1, ">>");
            p.addString(1, 1, "==");
            p.addString(1, 1, "!=");
            p.addString(1, 1, "<=");
            p.addString(1, 1, ">=");
            p.addString(1, 1, "&&");
            p.addString(1, 1, "||");
            p.addString(1, 1, "++");
            p.addString(1, 1, "--");
            p.addString(1, 1, "->*");
            p.addString(1, 1, "->");
            p.addChars(1, 1, "{}[]();:.+-*/%^&|~!=<>,");
            p.addString(1, 1, "and");
            p.addString(1, 1, "and_eq");
            p.addString(1, 1, "bitand");
            p.addString(1, 1, "bitor");
            p.addString(1, 1, "compl");
            p.addString(1, 1, "not_eq");
            p.addString(1, 1, "or_eq");
            p.addString(1, 1, "xor_eq");
            p.addString(1, 1, "xor");
            p.addString(1, 1, "or");
            p.addString(1, 1, "not");
            p.addString(1, 1, "new");
            p.addString(1, 1, "delete");


            c = getCharset("anychar");
            c.Inverted = true;

            c = getCharset("not_newline");
            c.Inverted = true;
            c.addChars("\r\n");

            p = getLexicalPattern("_comment_entry");
            p.IsParallel = true;
            p.addString(1, -1, "*/");
            p.addCharset(1, 1, getCharset("anychar"));

            p = getLexicalPattern("comment");
            p.addString(1, 1, "/*");
            p.addPattern(0, 1000000, getLexicalPattern("_comment_entry"));
            p.addString(1, 1, "*/");

            p = getLexicalPattern("comment_inline");
            p.addString(1, 1, "//");
            p.addCharset(0, 1000000, getCharset("not_newline"));

            p = getLexicalPattern("new-line");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addString(1, 1, "\r\n");
            p.addString(1, 1, "\r");
            p.addString(1, 1, "\n");

            p = getLexicalPattern("decimal-literal");
            p.addCharset(1, 1, getCharset("nonzero-digit"));
            p.addCharset(0, 10000, getCharset("digit"));

            p = getLexicalPattern("octal-literal");
            p.addChar(1, 1000, '0');
            p.addCharset(0, 10000, getCharset("octal-digit"));

            p = getLexicalPattern("hexadecimal-literal");
            p.addChar(1, 1000, '0');
            p.addChars(1, 1, "xX");
            p.addCharset(1, 200, getCharset("hexadecimal-digit"));

            p = getLexicalPattern("integer-literal-dec");
            p.addPattern(1, 1, getLexicalPattern("decimal-literal"));
            p.addPattern(0, 1, getLexicalPattern("integer-suffix"));

            p = getLexicalPattern("integer-literal-oct");
            p.addPattern(1, 1, getLexicalPattern("octal-literal"));
            p.addPattern(0, 1, getLexicalPattern("integer-suffix"));

            p = getLexicalPattern("integer-literal-hex");
            p.addPattern(1, 1, getLexicalPattern("hexadecimal-literal"));
            p.addPattern(0, 1, getLexicalPattern("integer-suffix"));

            p = getLexicalPattern("integer-suffix");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addString(1, 1, "ull");
            p.addString(1, 1, "uLL");
            p.addString(1, 1, "Ull");
            p.addString(1, 1, "ULL");
            p.addString(1, 1, "llu");
            p.addString(1, 1, "LLu");
            p.addString(1, 1, "llU");
            p.addString(1, 1, "LLU");
            p.addString(1, 1, "uL");
            p.addString(1, 1, "ul");
            p.addString(1, 1, "UL");
            p.addString(1, 1, "Ul");
            p.addString(1, 1, "LU");
            p.addString(1, 1, "lU");
            p.addString(1, 1, "Lu");
            p.addString(1, 1, "lu");
            p.addString(1, 1, "u");
            p.addString(1, 1, "U");
            p.addString(1, 1, "l");
            p.addString(1, 1, "L");
            p.addString(1, 1, "ll");
            p.addString(1, 1, "LL");

            p = getLexicalPattern("integer-literal");
            p.MatchingMethod = TTPattern.METHOD_MAX;
            p.addPattern(1, 1, getLexicalPattern("integer-literal-dec"));
            p.addPattern(1, 1, getLexicalPattern("integer-literal-oct"));
            p.addPattern(1, 1, getLexicalPattern("integer-literal-hex"));

            p = getLexicalPattern("boolean-literal");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addString(1, 1, "true");
            p.addString(1, 1, "false");

            p = getLexicalPattern("pointer-literal");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addString(1, 1, "nullptr");

            p = getLexicalPattern("literal");
            p.MatchingMethod = TTPattern.METHOD_MAX;
            p.addPattern(1, 1, getLexicalPattern("character-literal"));
            p.addPattern(1, 1, getLexicalPattern("string-literal"));
            p.addPattern(1, 1, getLexicalPattern("integer-literal"));
            p.addPattern(1, 1, getLexicalPattern("floating-literal"));
            p.addPattern(1, 1, getLexicalPattern("boolean-literal"));
            p.addPattern(1, 1, getLexicalPattern("pointer-literal"));
            p.addPattern(1, 1, getLexicalPattern("user-defined-literal"));

            p = getLexicalPattern("integer-literal-dec-ud");
            p.addPattern(1, 1, getLexicalPattern("decimal-literal"));
            p.addPattern(0, 1, getLexicalPattern("ud-suffix"));

            p = getLexicalPattern("integer-literal-oct-ud");
            p.addPattern(1, 1, getLexicalPattern("octal-literal"));
            p.addPattern(0, 1, getLexicalPattern("ud-suffix"));

            p = getLexicalPattern("integer-literal-hex-ud");
            p.addPattern(1, 1, getLexicalPattern("hexadecimal-literal"));
            p.addPattern(0, 1, getLexicalPattern("ud-suffix"));

            p = getLexicalPattern("user-defined-integer-literal");
            p.MatchingMethod = TTPattern.METHOD_MAX;
            p.addPattern(1, 1, getLexicalPattern("integer-literal-dec-ud"));
            p.addPattern(1, 1, getLexicalPattern("integer-literal-oct-ud"));
            p.addPattern(1, 1, getLexicalPattern("integer-literal-hex-ud"));

            p = getLexicalPattern("floating-literal-a-ud");
            p.addPattern(1, 1, getLexicalPattern("fractional-constant"));
            p.addPattern(0, 1, getLexicalPattern("exponent-part"));
            p.addPattern(0, 1, getLexicalPattern("ud-suffix"));

            p = getLexicalPattern("floating-literal-b-ud");
            p.addCharset(1, 1000, getCharset("digit"));
            p.addPattern(1, 1, getLexicalPattern("exponent-part"));
            p.addPattern(0, 1, getLexicalPattern("ud-suffix"));

            p = getLexicalPattern("user-defined-floating-literal");
            p.MatchingMethod = TTPattern.METHOD_FIRST;
            p.addPattern(1, 1, getLexicalPattern("floating-literal-a-ud"));
            p.addPattern(1, 1, getLexicalPattern("floating-literal-b-ud"));

            p = getLexicalPattern("user-defined-literal");
            p.MatchingMethod = TTPattern.METHOD_MAX;
            p.addPattern(1, 1, getLexicalPattern("user-defined-character-literal"));
            p.addPattern(1, 1, getLexicalPattern("user-defined-string-literal"));
            p.addPattern(1, 1, getLexicalPattern("user-defined-integer-literal"));
            p.addPattern(1, 1, getLexicalPattern("user-defined-floating-literal"));

            p = getLexicalPattern("preprocessing-token");
            p.MatchingMethod = TTPattern.METHOD_MAX;
            p.addPattern(1, 1, getLexicalPattern("header-name"));
            p.addPattern(1, 1, getLexicalPattern("identifier"));
            p.addPattern(1, 1, getLexicalPattern("preprocessing-op-or-punc"));
            p.addPattern(1, 1, getLexicalPattern("literal"));
            base.Initialize();
        }

        public override bool Run(TTInputTextFile ip)
        {
            bool result = false;
            atomList = new TTAtomList();
            resultTree = new TTNode();
            resultTree.Name = "FILE";

            try
            {
                TTErrorLog.Shared.enterDir("syntax", new TextPosition());
                atomList = mainParser.ParseTextToAtomList(ip);
                result = !ip.canRead();
                TTErrorLog.Shared.goUp();

                if (result)
                {
                    addLastAtom("EOF", "EOF");
                    atomList.removeAtomsWithType("ws");
                    atomList.removeAtomsWithType("nl");
                    atomList.removeAtomsWithType("comment");
                    atomList.removeAtomsWithType("comment_inline");
                    atomList.current = atomList.first;

                    TTErrorLog.Shared.enterDir("semantics", new TextPosition());
                    resultTree = syntaxParser.ParseAtomListToTree(atomList);
                    //syntaxParser.ParseTree(atomList, resultTree);
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
