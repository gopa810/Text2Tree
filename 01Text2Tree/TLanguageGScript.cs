using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextTreeParser;

namespace Text2Tree
{
    public class TLanguageGScript: TLanguageBase
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

        //TTPattern mainParserEntry;
        List<TTTreeAdjustmentRule> treeAdjustmentRules;


        public override void Initialize()
        {
            base.Initialize();


            charsetAlpha = new TTCharset("cs.alpha");
            charsetAlpha.addRange('a', 'z');
            charsetAlpha.addRange('A', 'Z');

            charsetDigits = new TTCharset("cs.digit");
            charsetDigits.addRange('0', '9');

            charsetWhitespaceNewline = new TTCharset("cs.ws.stnl");
            charsetWhitespaceNewline.addChars(" \t\n\r");

            charsetWhitespace = new TTCharset("cs.ws.st");
            charsetWhitespace.addChars(" \t");

            charsetNewline = new TTCharset("cs.ws.nl");
            charsetNewline.addChars("\r\n");

            csIdent1 = new TTCharset("cs.idpart.alpha");
            csIdent1.addRange('a', 'z');
            csIdent1.addRange('A', 'Z');
            csIdent1.addChars('_');

            csIdent2 = new TTCharset("cs.idpart.alphanum");
            csIdent2.addRange('a', 'z');
            csIdent2.addRange('A', 'Z');
            csIdent2.addRange('0', '9');
            csIdent2.addChars('_');

            csAnyChar = new TTCharset("cs.anychar");
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

            patternDoublequotedStringLiteral = new TTPattern("lit.str.d");
            patternDoublequotedStringLiteral.addChar(1, 1, '\"');
            patternDoublequotedStringLiteral.addPattern(0, 1000000, patternDoublequotedStringLiteralCharacter);
            patternDoublequotedStringLiteral.addChar(1, 1, '\"');

            patternQuotedStringLiteralCharacter = new TTPattern("_string_lit_char_a");
            patternQuotedStringLiteralCharacter.IsParallel = true;
            patternQuotedStringLiteralCharacter.addCharset(1, 1, csSLCA);
            patternQuotedStringLiteralCharacter.addPattern(1, 1, patternStringLiteralEscapeSequence);

            patternQuotedStringLiteral = new TTPattern("lit.str.s");
            patternQuotedStringLiteral.addChar(1, 1, '\'');
            patternQuotedStringLiteral.addPattern(0, 1000000, patternQuotedStringLiteralCharacter);
            patternQuotedStringLiteral.addChar(1, 1, '\'');

            patternFloatNumber = new TTPattern("lit.num.float");
            patternFloatNumber.addChars(0, 1, "+-");
            patternFloatNumber.addCharset(0, 1000, charsetDigits);
            patternFloatNumber.addChar(1, 1, '.');
            patternFloatNumber.addCharset(0, 1000, charsetDigits);
            patternFloatNumber.addChars(0, 1, "fF");

            TTPattern patternIntegerNumber = new TTPattern("lit.num.int");
            patternIntegerNumber.addChars(0, 1, "+-");
            patternIntegerNumber.addCharset(1, 1000, charsetDigits);

            patternDoubleNumberExtension = new TTPattern("_double_exp");
            patternDoubleNumberExtension.addChars(1, 1, "Ee");
            patternDoubleNumberExtension.addChars(0, 1, "+-");
            patternDoubleNumberExtension.addCharset(1, 1000, charsetDigits);

            patternDoubleNumber = new TTPattern("lit.num.double");
            patternDoubleNumber.addChars(0, 1, "+-");
            patternDoubleNumber.addCharset(0, 1000, charsetDigits);
            patternDoubleNumber.addChar(1, 1, '.');
            patternDoubleNumber.addCharset(0, 1000, charsetDigits);
            patternDoubleNumber.addPattern(0, 1, patternDoubleNumberExtension);

            patternIdentifier = new TTPattern("lit.id");
            patternIdentifier.addCharset(1, 1, csIdent1);
            patternIdentifier.addCharset(0, 1000, csIdent2);

            patternWhitespace = new TTPattern("lit.ws.st");
            patternWhitespace.addCharset(1, 1000000, charsetWhitespace);

            patternNewline = new TTPattern("lit.ws.nl");
            patternNewline.IsParallel = true;
            patternNewline.addString(1, 1, "\r\n");
            patternNewline.addString(1, 1, "\r");
            patternNewline.addString(1, 1, "\n");

            charsetBrackets = new TTCharset("cs.brack");
            charsetBrackets.addChars("()[]{}");

            patternBrackets = new TTPattern("sem.brack");
            patternBrackets.addCharset(1, 1, charsetBrackets);

            patternOperators = new TTPattern("sem.oper");
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
            patternOperators.addString(1, 1, ",");
            patternOperators.addString(1, 1, "?");
            patternOperators.addString(1, 1, ":");


            patternPunctuation = new TTPattern("sem.punc");
            patternPunctuation.IsParallel = true;
            patternPunctuation.addString(1, 1, "?");
            patternPunctuation.addString(1, 1, ":");
            patternPunctuation.addString(1, 1, ",");
            patternPunctuation.addString(1, 1, ";");
            patternPunctuation.addString(1, 1, "#");

            /*TTParser paNumber = new TTParser();
            paNumber.Max(patternDoubleNumber, patternFloatNumber, patternIntegerNumber);*/
            TTPattern paNumber = new TTPattern("lit.num");
            paNumber.matchingNameTake = true;
            paNumber.MatchingMethod = TTPattern.METHOD_MAX;
            paNumber.addPattern(1, 1, patternDoubleNumber);
            paNumber.addPattern(1, 1, patternFloatNumber);
            paNumber.addPattern(1, 1, patternIntegerNumber);

            // main parser line
            // contains all possible syntax entities
            /*mainParserEntry = new TTParser();
            mainParserEntry.First(patternIdentifier, patternNewline, patternWhitespace,
                paNumber, patternDoublequotedStringLiteral, patternQuotedStringLiteral,
                patternInlineComment, patternComment, patternBrackets, patternOperators, patternPunctuation);
            */

            mainParser = new TTPattern();
            mainParser.matchingNameTake = true;
            mainParser.MatchingMethod = TTPattern.METHOD_FIRST;
            mainParser.addPattern(1, 1, patternIdentifier);
            mainParser.addPattern(1, 1, patternNewline);
            mainParser.addPattern(1, 1, patternWhitespace);
            mainParser.addPattern(1, 1, paNumber);
            mainParser.addPattern(1, 1, patternDoublequotedStringLiteral);
            mainParser.addPattern(1, 1, patternQuotedStringLiteral);
            mainParser.addPattern(1, 1, patternInlineComment);
            mainParser.addPattern(1, 1, patternComment);
            mainParser.addPattern(1, 1, patternBrackets);
            mainParser.addPattern(1, 1, patternOperators);
            mainParser.addPattern(1, 1, patternPunctuation);

            //
            // lexical analysis
            //
            // main parser definition
            //mainParser = new TTPattern();
            //mainParser.MatchingMethod = TTPattern.METHOD_LIST;
            //mainParser.addPattern(1, 1, mainParserEntry);


            //
            // semantical analysis
            //
            TTPattern pat3;

            pat3 = getSyntaxPattern("sem.oper.bin");
            pat3.MatchingMethod = TTPattern.METHOD_FIRST;
            pat3.addAtom(1, 1, "sem.oper", "<<=", "");
            pat3.addAtom(1, 1, "sem.oper", ">>=", "");
            pat3.addAtom(1, 1, "sem.oper", "&&=", "");
            pat3.addAtom(1, 1, "sem.oper", "||=", "");
            pat3.addAtom(1, 1, "sem.oper", "<<", "");
            pat3.addAtom(1, 1, "sem.oper", ">>", "");
            pat3.addAtom(1, 1, "sem.oper", "+=", "");
            pat3.addAtom(1, 1, "sem.oper", "-=", "");
            pat3.addAtom(1, 1, "sem.oper", "*=", "");
            pat3.addAtom(1, 1, "sem.oper", "/=", "");
            pat3.addAtom(1, 1, "sem.oper", "~=", "");
            pat3.addAtom(1, 1, "sem.oper", "%=", "");
            pat3.addAtom(1, 1, "sem.oper", "&&", "");
            pat3.addAtom(1, 1, "sem.oper", "||", "");
            pat3.addAtom(1, 1, "sem.oper", "&=", "");
            pat3.addAtom(1, 1, "sem.oper", "|=", "");
            pat3.addAtom(1, 1, "sem.oper", "<=", "");
            pat3.addAtom(1, 1, "sem.oper", ">=", "");
            pat3.addAtom(1, 1, "sem.oper", "!=", "");
            pat3.addAtom(1, 1, "sem.oper", "==", "");
            pat3.addAtom(1, 1, "sem.oper", "=", "");
            pat3.addAtom(1, 1, "sem.oper", "<", "");
            pat3.addAtom(1, 1, "sem.oper", "&", "");
            pat3.addAtom(1, 1, "sem.oper", "|", "");
            pat3.addAtom(1, 1, "sem.oper", ">", "");
            pat3.addAtom(1, 1, "sem.oper", "+", "");
            pat3.addAtom(1, 1, "sem.oper", "-", "");
            pat3.addAtom(1, 1, "sem.oper", "*", "");
            pat3.addAtom(1, 1, "sem.oper", "/", "");
            pat3.addAtom(1, 1, "sem.oper", "~", "");
            pat3.addAtom(1, 1, "sem.oper", "%", "");
            pat3.addAtom(1, 1, "sem.oper", ",", "");
            pat3.addAtom(1, 1, "sem.oper", "?", "");
            pat3.addAtom(1, 1, "sem.oper", ":", "");

            pat3 = getSyntaxPattern("sem.oper.un.pre");
            pat3.MatchingMethod = TTPattern.METHOD_FIRST;
            pat3.addAtom(1, 1, "sem.oper", "++", "");
            pat3.addAtom(1, 1, "sem.oper", "--", "");
            pat3.addAtom(1, 1, "sem.oper", "-", "");
            pat3.addAtom(1, 1, "sem.oper", "!", "");
            pat3.addAtom(1, 1, "sem.oper", "~", "");

            pat3 = getSyntaxPattern("sem.oper.un.post");
            pat3.MatchingMethod = TTPattern.METHOD_FIRST;
            pat3.addAtom(1, 1, "sem.oper", "++", "");
            pat3.addAtom(1, 1, "sem.oper", "--", "");

            pat3 = getSyntaxPattern("sem.call.func");
            //pat3.OutputIdentity = "sem.call.func";
            pat3.addAtom(1, 1, "sem.brack", "[", null);
            pat3.addAtom(1, 1, "lit.id", null, "func.object");
            pat3.addAtom(1, 1, "lit.id", null, "func.method");
            pat3.addPattern(0, 10000, getSyntaxPattern("exp.single"));
            pat3.addAtom(1, 1, "sem.brack", "]", null);

            pat3 = getSyntaxPattern("stat.def.variable.var");
            pat3.addAtom(1, 1, "sem.punc", ",", null);
            pat3.addAtom(1, 1, "lit.id", null, "var.name");

            pat3 = getSyntaxPattern("stat.def.variable");
            //pat3.OutputIdentity = "var_def";
            pat3.addAtom(1, 1, "lit.id", "VAR", null);
            pat3.addAtom(1, 1, "lit.id", null, "var.datatype");
            pat3.addAtom(1, 1, "lit.id", null, "var.name");
            pat3.addPattern(0, 1000, getSyntaxPattern("stat.def.variable.var"));
            pat3.addAtom(1, 1, "sem.punc", ";", null);

            pat3 = getSyntaxPattern("stat.exp.end");
            pat3.addPattern(1, 1, getSyntaxPattern("expression"));
            pat3.addAtom(1, 1, "sem.punc", ";", null);

            pat3 = getSyntaxPattern("expression_enveloped");
            pat3.addAtom(1, 1, "sem.brack", "(", null);
            pat3.addPattern(1, 1, getSyntaxPattern("expression"));
            pat3.addAtom(1, 1, "sem.brack", ")", null);

            pat3 = getSyntaxPattern("exp.single");
            pat3.MatchingMethod = TTPattern.METHOD_MAX;
            pat3.addAtom(1, 1, "lit.str.d", null, "exp.single.literal.string");
            pat3.addAtom(1, 1, "lit.str.s", null, "exp.single.literal.string");
            pat3.addAtom(1, 1, "lit.num.double", null, "exp.single.literal.double");
            pat3.addAtom(1, 1, "lit.num.float", null, "exp.single.literal.float");
            pat3.addAtom(1, 1, "lit.num.int", null, "exp.single.literal.int");
            pat3.addAtom(1, 1, "lit.id", null, "exp.single.literal.id");
            pat3.addPattern(1, 1, getSyntaxPattern("sem.call.func"));
            pat3.addPattern(1, 1, getSyntaxPattern("expression_enveloped"));

            pat3 = getSyntaxPattern("expression_binary");
            pat3.addPattern(1, 1, getSyntaxPattern("sem.oper.bin"));
            pat3.addPattern(1, 1, getSyntaxPattern("expression_prepostfixed"));

            pat3 = getSyntaxPattern("expression_unary_pref");
            pat3.addPattern(1, 1, getSyntaxPattern("sem.oper.un.pre"));
            pat3.addPattern(1, 1, getSyntaxPattern("expression"));

            pat3 = getSyntaxPattern("expression_unary_post");
            pat3.addPattern(1, 1, getSyntaxPattern("expression"));
            pat3.addPattern(1, 1, getSyntaxPattern("sem.oper.un.post"));

            pat3 = getSyntaxPattern("expression_prepostfixed");
            pat3.addPattern(0, 1, getSyntaxPattern("sem.oper.un.pre"));
            pat3.addPattern(1, 1, getSyntaxPattern("exp.single"));
            pat3.addPattern(0, 1, getSyntaxPattern("sem.oper.un.post"));

            pat3 = getSyntaxPattern("expression");
            //pat3.OutputIdentity = "expression";
            pat3.addPattern(1, 1, getSyntaxPattern("expression_prepostfixed"));
            pat3.addPattern(0, 1000, getSyntaxPattern("expression_binary"));


            pat3 = getSyntaxPattern("exec.block");
            pat3.addAtom(1, 1, "sem.brack", "{", null);
            pat3.addPattern(0, 100000, getSyntaxPattern("statement"));
            pat3.addAtom(1, 1, "sem.brack", "}", null);

            pat3 = getSyntaxPattern("stat.def.func");
            //pat3.OutputIdentity = "func_def";
            pat3.addAtom(1, 1, "lit.id", "func", null);
            pat3.addAtom(1, 1, "lit.id", null, "func.name");
            pat3.addPattern(1, 1, getSyntaxPattern("exec.block"));

            pat3 = getSyntaxPattern("stat.cmd.if.else");
            pat3.addAtom(1, 1, "lit.id", "else", null);
            pat3.addPattern(1, 1, getSyntaxPattern("exec.block"));


            pat3 = getSyntaxPattern("stat.cmd.if");
            //pat3.OutputIdentity = "stat.if";
            pat3.addAtom(1, 1, "lit.id", "if", null);
            pat3.addPattern(1, 1, getSyntaxPattern("exp.single"));
            pat3.addPattern(1, 1, getSyntaxPattern("exec.block"));
            pat3.addPattern(1, 1, getSyntaxPattern("stat.cmd.if.else"));

            pat3 = getSyntaxPattern("statement");
            //pat3.OutputIdentity = "";
            pat3.MatchingMethod = TTPattern.METHOD_MAX;
            pat3.addAtom(1, 1, "EOF", "EOF", null);
            pat3.addPattern(1, 1, getSyntaxPattern("stat.def.variable"));
            pat3.addPattern(1, 1, getSyntaxPattern("stat.exp.end"));
            pat3.addPattern(1, 1, getSyntaxPattern("stat.def.func"));
            pat3.addPattern(1, 1, getSyntaxPattern("stat.cmd.if"));

            TTPattern syntaxItem = new TTPattern("GScriptSyntaxItem");
            syntaxItem.MatchingMethod = TTPattern.METHOD_FIRST;
            syntaxItem.matchingNameTake = true;
            syntaxItem.addAtom(1, 1, "EOF", "EOF", "EOF");
            syntaxItem.addPattern(1, 1, getSyntaxPattern("statement"));
            //TTParser syntaxItem = new TTParser();
            //syntaxItem.First(new TTParserAtom("EOF", "EOF"), getSyntaxPattern("statement"));

            syntaxParser = new TTPattern("GScript");
            syntaxParser.addPattern(0, 10000000, syntaxItem);
            //syntaxParser.List(syntaxItem);


            //
            // tree reducing rules
            //
            treeAdjustmentRules = new List<TTTreeAdjustmentRule>();
            TTTreeAdjustmentRule trule = new TTTreeAdjustmentRule();

            trule.setTypeType("expression", TTTreeAdjustmentRule.PART_WHOLE, "exp.", TTTreeAdjustmentRule.PART_START);
            treeAdjustmentRules.Add(trule);


        }

        public override bool Run(TTInputTextFile ip)
        {
            bool result = false;
            atomList = new TTAtomList();
            resultTree = new TTNode();
            resultTree.Name = "FILE";

            //
            // PHASE 1
            // parse characters into atoms
            //
            TTErrorLog.Shared.enterDir("syntax", new TextPosition());
            atomList = mainParser.ParseTextToAtomList(ip);

            //
            // if EOF was reached, then parsing to atoms was successful
            result = !ip.canRead();
            TTErrorLog.Shared.goUp();
            addLastAtom("EOF", "EOF");


            //
            // PHASE 2
            // parse atoms into tree
            //
            if (result)
            {
                atomList.removeAtomsWithType("lit.ws.st");
                atomList.removeAtomsWithType("lit.ws.nl");
                atomList.removeAtomsWithType("comment");
                atomList.removeAtomsWithType("comment_inline");
                atomList.current = atomList.first;

                TTErrorLog.Shared.enterDir("semantics", new TextPosition());
                resultTree = syntaxParser.ParseAtomListToTree(atomList);
                //syntaxParser.ParseTree(atomList, resultTree);
                TTErrorLog.Shared.goUp();
            }

            //
            // PHASE 3
            // tree adjustments
            //
            resultTree.AdjustTree(treeAdjustmentRules);

            return result;
        }


    }
}
