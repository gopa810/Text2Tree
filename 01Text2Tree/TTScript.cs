using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextTreeParser;

namespace Text2Tree
{
    public class TTScript
    {
        TTCharset charsetAlpha;
        TTCharset charsetDigits;
        TTCharset charsetWhitespaceNewline;
        TTCharset charsetWhitespace;
        TTCharset charsetNewline;
        TTCharset csIdent1;
        TTCharset csIdent2;
        TTCharset csAnyChar;
        TTCharset charsetNotNewline;
        TTCharset csSLC;
        TTCharset csSLCA;
        TTCharset charsetBrackets;

        TTPattern patternInlineComment;
        TTPattern patternCommentEntry;
        TTPattern patternComment;
        TTPattern patternPunctuation;
        TTPattern patternStringLiteralEscapeSequence;
        TTPattern patternDoublequotedStringLiteralCharacter;
        TTPattern patternDoublequotedStringLiteral;
        TTPattern patternQuotedStringLiteralCharacter;
        TTPattern patternQuotedStringLiteral;
        TTPattern patternFloatNumber;
        TTPattern patternDoubleNumberExtension;
        TTPattern patternDoubleNumber;
        TTPattern patternIdentifier;
        TTPattern patternWhitespace;
        TTPattern patternNewline;
        TTPattern patternOperators;
        TTPattern patternBrackets;

        TTParser mainParserEntry;
        TTParser mainParser;

        TTParser syntaxParser;

        public TTAtomList atomList;
        public TTTreeNode resultTree;
        public TTErrorLog errorLog;

        public void Initialize()
        {
            errorLog = new TTErrorLog();

            charsetAlpha = new TTCharset("alpha");
            charsetAlpha.addRange('a', 'z');
            charsetAlpha.addRange('A', 'Z');

            charsetDigits = new TTCharset("digit");
            charsetDigits.addRange('0', '9');

            charsetWhitespaceNewline = new TTCharset("whitespaceNewline");
            charsetWhitespaceNewline.addChars(" \t\n\r");

            charsetWhitespace = new TTCharset("whitespace");
            charsetWhitespace.addChars(" \t");

            charsetNewline = new TTCharset("newline");
            charsetNewline.addChars("\r\n");

            csIdent1 = new TTCharset("ident1");
            csIdent1.addRange('a', 'z');
            csIdent1.addRange('A', 'Z');
            csIdent1.addChars('_');

            csIdent2 = new TTCharset("ident2");
            csIdent2.addRange('a', 'z');
            csIdent2.addRange('A', 'Z');
            csIdent2.addRange('0', '9');
            csIdent2.addChars('_');

            csAnyChar = new TTCharset("anychar");
            csAnyChar.Inverted = true;

            patternCommentEntry = new TTPattern("_comment_entry");
            patternCommentEntry.IsParallel = true;
            patternCommentEntry.addString(1, -1, "*/");
            patternCommentEntry.addCharset(1, 1, csAnyChar);

            patternComment = new TTPattern("comment");
            patternComment.addString(1, 1, "/*");
            patternComment.addPattern(0, 1000000, patternCommentEntry);
            patternComment.addString(1, 1, "*/");


            charsetNotNewline = new TTCharset("not_newline");
            charsetNotNewline.Inverted = true;
            charsetNotNewline.addChars("\r\n");

            patternInlineComment = new TTPattern("comment_inline");
            patternInlineComment.addString(1, 1, "//");
            patternInlineComment.addCharset(0, 1000000, charsetNotNewline);

            csSLC = new TTCharset("_cs_string_lit_char");
            csSLC.addChars("\\\"");
            csSLC.Inverted = true;

            csSLCA = new TTCharset("_cs_string_lit_char_a");
            csSLCA.addChars("\\\'");
            csSLCA.Inverted = true;

            patternStringLiteralEscapeSequence = new TTPattern("_str_lit_esc_seq");
            patternStringLiteralEscapeSequence.addChar(1, 1, '\\');
            patternStringLiteralEscapeSequence.addCharset(1, 1, csAnyChar);

            patternDoublequotedStringLiteralCharacter = new TTPattern("_string_lit_char");
            patternDoublequotedStringLiteralCharacter.IsParallel = true;
            patternDoublequotedStringLiteralCharacter.addCharset(1, 1, csSLC);
            patternDoublequotedStringLiteralCharacter.addPattern(1, 1, patternStringLiteralEscapeSequence);

            patternDoublequotedStringLiteral = new TTPattern("string_lit");
            patternDoublequotedStringLiteral.addChar(1, 1, '\"');
            patternDoublequotedStringLiteral.addPattern(0, 1000000, patternDoublequotedStringLiteralCharacter);
            patternDoublequotedStringLiteral.addChar(1, 1, '\"');

            patternQuotedStringLiteralCharacter = new TTPattern("_string_lit_char_a");
            patternQuotedStringLiteralCharacter.IsParallel = true;
            patternQuotedStringLiteralCharacter.addCharset(1, 1, csSLCA);
            patternQuotedStringLiteralCharacter.addPattern(1, 1, patternStringLiteralEscapeSequence);

            patternQuotedStringLiteral = new TTPattern("string_lit_a");
            patternQuotedStringLiteral.addChar(1, 1, '\'');
            patternQuotedStringLiteral.addPattern(0, 1000000, patternQuotedStringLiteralCharacter);
            patternQuotedStringLiteral.addChar(1, 1, '\'');

            patternFloatNumber = new TTPattern("float");
            patternFloatNumber.addChars(0, 1, "+-");
            patternFloatNumber.addCharset(0, 1000, charsetDigits);
            patternFloatNumber.addChar(1, 1, '.');
            patternFloatNumber.addCharset(0, 1000, charsetDigits);
            patternFloatNumber.addChars(0, 1, "fF");

            patternDoubleNumberExtension = new TTPattern("_double_exp");
            patternDoubleNumberExtension.addChars(1, 1, "Ee");
            patternDoubleNumberExtension.addChars(0, 1, "+-");
            patternDoubleNumberExtension.addCharset(1, 1000, charsetDigits);

            patternDoubleNumber = new TTPattern("double");
            patternDoubleNumber.addChars(0, 1, "+-");
            patternDoubleNumber.addCharset(0, 1000, charsetDigits);
            patternDoubleNumber.addChar(1, 1, '.');
            patternDoubleNumber.addCharset(0, 1000, charsetDigits);
            patternDoubleNumber.addPattern(0, 1, patternDoubleNumberExtension);

            patternIdentifier = new TTPattern("id");
            patternIdentifier.addCharset(1, 1, csIdent1);
            patternIdentifier.addCharset(0, 1000, csIdent2);

            patternWhitespace = new TTPattern("ws");
            patternWhitespace.addCharset(1, 1000000, charsetWhitespace);

            patternNewline = new TTPattern("nl");
            patternNewline.IsParallel = true;
            patternNewline.addString(1, 1, "\r\n");
            patternNewline.addString(1, 1, "\r");
            patternNewline.addString(1, 1, "\n");

            charsetBrackets = new TTCharset("brack");
            charsetBrackets.addChars("()[]{}");

            patternBrackets = new TTPattern("brack");
            patternBrackets.addCharset(1, 1, charsetBrackets);

            patternOperators = new TTPattern("oper");
            patternOperators.IsParallel = true;
            patternOperators.addString(1, 1, "+=");
            patternOperators.addString(1, 1, "-=");
            patternOperators.addString(1, 1, "/=");
            patternOperators.addString(1, 1, "*=");
            patternOperators.addString(1, 1, "^=");
            patternOperators.addString(1, 1, "~=");
            patternOperators.addString(1, 1, "&=");
            patternOperators.addString(1, 1, "|=");
            patternOperators.addString(1, 1, "&&=");
            patternOperators.addString(1, 1, "||=");
            patternOperators.addString(1, 1, "+");
            patternOperators.addString(1, 1, "-");
            patternOperators.addString(1, 1, "++");
            patternOperators.addString(1, 1, "--");
            patternOperators.addString(1, 1, "*");
            patternOperators.addString(1, 1, "/");
            patternOperators.addString(1, 1, "&");
            patternOperators.addString(1, 1, "|");
            patternOperators.addString(1, 1, "^");
            patternOperators.addString(1, 1, "&&");
            patternOperators.addString(1, 1, "||");
            patternOperators.addString(1, 1, "=");
            patternOperators.addString(1, 1, "==");
            patternOperators.addString(1, 1, "<<");
            patternOperators.addString(1, 1, ">>");
            patternOperators.addString(1, 1, "<<=");
            patternOperators.addString(1, 1, ">>=");
            patternOperators.addString(1, 1, "<=");
            patternOperators.addString(1, 1, ">=");
            patternOperators.addString(1, 1, "%");
            patternOperators.addString(1, 1, "%=");


            patternPunctuation = new TTPattern("punc");
            patternPunctuation.IsParallel = true;
            patternPunctuation.addString(1, 1, "?");
            patternPunctuation.addString(1, 1, ":");
            patternPunctuation.addString(1, 1, ",");
            patternPunctuation.addString(1, 1, ";");
            patternPunctuation.addString(1, 1, "#");

            TTParser paNumber = new TTParser();
            paNumber.Max(patternDoubleNumber, patternFloatNumber);

            // main parser line
            // contains all possible syntax entities
            mainParserEntry = new TTParser(errorLog);
            mainParserEntry.First(patternIdentifier, patternNewline, patternWhitespace,
                paNumber, patternDoublequotedStringLiteral, patternQuotedStringLiteral,
                patternInlineComment, patternComment, patternBrackets, patternOperators, patternPunctuation);

            // main parser definition
            mainParser = new TTParser(errorLog);
            mainParser.List(mainParserEntry);

            TTPattern pat3 = new TTPattern("func_arg");
            TTPattern pat2 = new TTPattern("function_call");

            pat3.IsParallel = true;
            pat3.addAtom(1, 1, "string_lit_a", null);
            pat3.addPattern(1, 1, pat2);

            pat2.addAtom(1, 1, "brack", "[");
            pat2.addAtom(1, 1, "id", null);
            pat2.addAtom(1, 1, "id", null);
            pat2.addPattern(0, 10000, pat3);
            pat2.addAtom(1, 1, "brack", "]");

            TTParser syntaxItem = new TTParser(errorLog);
            syntaxItem.First(pat2);

            syntaxParser = new TTParser(errorLog);
            syntaxParser.List(syntaxItem);

        }

        public bool Run(TTInputTextFile ip)
        {
            bool result = false;
            atomList = new TTAtomList();
            resultTree = new TTTreeNode();

            try
            {

                result = mainParser.ParseAtomList(ip, atomList);

                if (result)
                {
                    atomList.removeAtomsWithType("ws");
                    atomList.removeAtomsWithType("nl");

                    syntaxParser.ParseTree(atomList, resultTree);
                }
            }
            catch (Exception x)
            {
                errorLog.addLog("{0}", x.Message);
            }

            return result;
        }

    }
}
