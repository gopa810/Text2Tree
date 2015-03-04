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
            patternComment.addPattern(0, 1000000, patternCommentEntry, "");
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
            patternDoublequotedStringLiteralCharacter.addPattern(1, 1, patternStringLiteralEscapeSequence, "");

            patternDoublequotedStringLiteral = new TTPattern("l_string_dq");
            patternDoublequotedStringLiteral.addChar(1, 1, '\"');
            patternDoublequotedStringLiteral.addPattern(0, 1000000, patternDoublequotedStringLiteralCharacter, "");
            patternDoublequotedStringLiteral.addChar(1, 1, '\"');

            patternQuotedStringLiteralCharacter = new TTPattern("_string_lit_char_a");
            patternQuotedStringLiteralCharacter.IsParallel = true;
            patternQuotedStringLiteralCharacter.addCharset(1, 1, csSLCA);
            patternQuotedStringLiteralCharacter.addPattern(1, 1, patternStringLiteralEscapeSequence, "");

            patternQuotedStringLiteral = new TTPattern("l_string_sq");
            patternQuotedStringLiteral.addChar(1, 1, '\'');
            patternQuotedStringLiteral.addPattern(0, 1000000, patternQuotedStringLiteralCharacter, "");
            patternQuotedStringLiteral.addChar(1, 1, '\'');

            patternFloatNumber = new TTPattern("l_float");
            patternFloatNumber.addChars(0, 1, "+-");
            patternFloatNumber.addCharset(0, 1000, charsetDigits);
            patternFloatNumber.addChar(1, 1, '.');
            patternFloatNumber.addCharset(0, 1000, charsetDigits);
            patternFloatNumber.addChars(0, 1, "fF");

            TTPattern patternIntegerNumber = new TTPattern("l_integer");
            patternIntegerNumber.addChars(0, 1, "+-");
            patternIntegerNumber.addCharset(1, 1000, charsetDigits);

            patternDoubleNumberExtension = new TTPattern("_double_exp");
            patternDoubleNumberExtension.addChars(1, 1, "Ee");
            patternDoubleNumberExtension.addChars(0, 1, "+-");
            patternDoubleNumberExtension.addCharset(1, 1000, charsetDigits);

            patternDoubleNumber = new TTPattern("l_double");
            patternDoubleNumber.addChars(0, 1, "+-");
            patternDoubleNumber.addCharset(0, 1000, charsetDigits);
            patternDoubleNumber.addChar(1, 1, '.');
            patternDoubleNumber.addCharset(0, 1000, charsetDigits);
            patternDoubleNumber.addPattern(0, 1, patternDoubleNumberExtension, "");

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
            paNumber.Max(patternDoubleNumber, patternFloatNumber, patternIntegerNumber);

            // main parser line
            // contains all possible syntax entities
            mainParserEntry = new TTParser();
            mainParserEntry.First(patternIdentifier, patternNewline, patternWhitespace,
                paNumber, patternDoublequotedStringLiteral, patternQuotedStringLiteral,
                patternInlineComment, patternComment, patternBrackets, patternOperators, patternPunctuation);

            // main parser definition
            mainParser = new TTParser();
            mainParser.List(mainParserEntry);

            TTPattern pat3;

            Dictionary<string, TTPattern> pats = new Dictionary<string, TTPattern>();

            pat3 = getPattern(pats, "binary_asoc_oper");
            pat3.MatchingMethod = TTPattern.METHOD_FIRST;
            pat3.addAtom(1, 1, "oper", "<<=", "");
            pat3.addAtom(1, 1, "oper", ">>=", "");
            pat3.addAtom(1, 1, "oper", "&&=", "");
            pat3.addAtom(1, 1, "oper", "||=", "");
            pat3.addAtom(1, 1, "oper", "<<", "");
            pat3.addAtom(1, 1, "oper", ">>", "");
            pat3.addAtom(1, 1, "oper", "+=", "");
            pat3.addAtom(1, 1, "oper", "-=", "");
            pat3.addAtom(1, 1, "oper", "*=", "");
            pat3.addAtom(1, 1, "oper", "/=", "");
            pat3.addAtom(1, 1, "oper", "~=", "");
            pat3.addAtom(1, 1, "oper", "%=", "");
            pat3.addAtom(1, 1, "oper", "&&", "");
            pat3.addAtom(1, 1, "oper", "||", "");
            pat3.addAtom(1, 1, "oper", "&=", "");
            pat3.addAtom(1, 1, "oper", "|=", "");
            pat3.addAtom(1, 1, "oper", "<=", "");
            pat3.addAtom(1, 1, "oper", ">=", "");
            pat3.addAtom(1, 1, "oper", "!=", "");
            pat3.addAtom(1, 1, "oper", "==", "");
            pat3.addAtom(1, 1, "oper", "=", "");
            pat3.addAtom(1, 1, "oper", "<", "");
            pat3.addAtom(1, 1, "oper", "&", "");
            pat3.addAtom(1, 1, "oper", "|", "");
            pat3.addAtom(1, 1, "oper", ">", "");
            pat3.addAtom(1, 1, "oper", "+", "");
            pat3.addAtom(1, 1, "oper", "-", "");
            pat3.addAtom(1, 1, "oper", "*", "");
            pat3.addAtom(1, 1, "oper", "/", "");
            pat3.addAtom(1, 1, "oper", "~", "");
            pat3.addAtom(1, 1, "oper", "%", "");

            pat3 = getPattern(pats, "unary_prefix_oper");
            pat3.MatchingMethod = TTPattern.METHOD_FIRST;
            pat3.addAtom(1, 1, "oper", "++", "");
            pat3.addAtom(1, 1, "oper", "--", "");
            pat3.addAtom(1, 1, "oper", "-", "");
            pat3.addAtom(1, 1, "oper", "!", "");
            pat3.addAtom(1, 1, "oper", "~", "");

            pat3 = getPattern(pats, "unary_postfix_oper");
            pat3.MatchingMethod = TTPattern.METHOD_FIRST;
            pat3.addAtom(1, 1, "oper", "++", "");
            pat3.addAtom(1, 1, "oper", "--", "");

            pat3 = getPattern(pats, "function_call");
            pat3.OutputIdentity = "func_call";
            pat3.addAtom(1, 1, "brack", "[", null);
            pat3.addAtom(1, 1, "id", null, "object");
            pat3.addAtom(1, 1, "id", null, "method");
            pat3.addPattern(0, 10000, getPattern(pats, "expression_single"), "arg");
            pat3.addAtom(1, 1, "brack", "]", null);

            pat3 = getPattern(pats, "variable_definition_var");
            pat3.addAtom(1, 1, "punc", ",", null);
            pat3.addAtom(1, 1, "id", null, "var_name");

            pat3 = getPattern(pats, "variable_definition");
            pat3.OutputIdentity = "var_def";
            pat3.addAtom(1, 1, "id", "VAR", null);
            pat3.addAtom(1, 1, "id", null, "datatype");
            pat3.addAtom(1, 1, "id", null, "var_name");
            pat3.addPattern(0, 1000, getPattern(pats, "variable_definition_var"), "");
            pat3.addAtom(1, 1, "punc", ";", null);

            pat3 = getPattern(pats, "expression_with_end");
            pat3.addPattern(1, 1, getPattern(pats, "expression"), "");
            pat3.addAtom(1, 1, "punc", ";", null);

            pat3 = getPattern(pats, "expression_enveloped");
            pat3.addAtom(1, 1, "brack", "(", null);
            pat3.addPattern(1, 1, getPattern(pats, "expression"), "");
            pat3.addAtom(1, 1, "brack", ")", null);

            pat3 = getPattern(pats, "expression_single");
            pat3.MatchingMethod = TTPattern.METHOD_MAX;
            pat3.addAtom(1, 1, "l_string_dq", null, "");
            pat3.addAtom(1, 1, "l_string_sq", null, "");
            pat3.addAtom(1, 1, "l_double", null, "");
            pat3.addAtom(1, 1, "l_float", null, "");
            pat3.addAtom(1, 1, "l_integer", null, "");
            pat3.addAtom(1, 1, "id", null, "");
            pat3.addPattern(1, 1, getPattern(pats, "function_call"), "func_call");
            pat3.addPattern(1, 1, getPattern(pats, "expression_enveloped"), "ENVELOPES");

            pat3 = getPattern(pats, "expression_binary");
            pat3.addPattern(1, 1, getPattern(pats, "binary_asoc_oper"), "");
            pat3.addPattern(1, 1, getPattern(pats, "expression_prepostfixed"), "");

            pat3 = getPattern(pats, "expression_unary_pref");
            pat3.addPattern(1, 1, getPattern(pats, "unary_prefix_oper"), "");
            pat3.addPattern(1, 1, getPattern(pats, "expression"), "");

            pat3 = getPattern(pats, "expression_unary_post");
            pat3.addPattern(1, 1, getPattern(pats, "expression"), "");
            pat3.addPattern(1, 1, getPattern(pats, "unary_postfix_oper"), "");

            pat3 = getPattern(pats, "expression_prepostfixed");
            pat3.addPattern(0, 1, getPattern(pats, "unary_prefix_oper"), "prefixoper");
            pat3.addPattern(1, 1, getPattern(pats, "expression_single"), "expression_single");
            pat3.addPattern(0, 1, getPattern(pats, "unary_postfix_oper"), "postfixoper");

            pat3 = getPattern(pats, "expression");
            pat3.OutputIdentity = "expression";
            pat3.addPattern(1, 1, getPattern(pats, "expression_prepostfixed"), "");
            pat3.addPattern(0, 1000, getPattern(pats, "expression_binary"), "");


            pat3 = getPattern(pats, "exec_block");
            pat3.addAtom(1, 1, "brack", "{", null);
            pat3.addPattern(0, 100000, getPattern(pats, "statement"), "");
            pat3.addAtom(1, 1, "brack", "}", null);

            pat3 = getPattern(pats, "func_definition");
            pat3.OutputIdentity = "func_def";
            pat3.addAtom(1, 1, "id", "func", null);
            pat3.addAtom(1, 1, "id", null, "func_name");
            pat3.addPattern(1, 1, getPattern(pats, "exec_block"), "block");

            pat3 = getPattern(pats, "statement");
            pat3.OutputIdentity = "statement";
            pat3.MatchingMethod = TTPattern.METHOD_MAX;
            pat3.addAtom(1, 1, "EOF", "EOF", null);
            pat3.addPattern(1, 1, getPattern(pats, "variable_definition"), "var_def");
            pat3.addPattern(1, 1, getPattern(pats, "expression_with_end"), "expression_w_end");
            pat3.addPattern(1, 1, getPattern(pats, "func_definition"), "func_def");

            TTParser syntaxItem = new TTParser();
            syntaxItem.First(new TTParserAtom("EOF", "EOF"), getPattern(pats, "statement"));

            syntaxParser = new TTParser();
            syntaxParser.List(syntaxItem);

        }

        public TTPattern getPattern(Dictionary<string, TTPattern> pats, string name)
        {
            if (pats.ContainsKey(name))
                return pats[name];
            TTPattern pt = new TTPattern(name);
            pats.Add(name, pt);
            return pt;
        }

        public bool Run(TTInputTextFile ip)
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
                if (atomList.last != null)
                {
                    TTAtom eof = new TTAtom("EOF", "EOF");
                    eof.startPos.position = atomList.last.endPos.position + 1;
                    eof.endPos.position = eof.startPos.position + 1;
                    atomList.addItem(eof);
                }

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
