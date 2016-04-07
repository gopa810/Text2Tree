using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TrepInterpreter
{
    public class Scripting
    {
        public static bool isWhitespace(char c)
        {
            return Char.IsWhiteSpace(c);
        }

        public readonly static int AT_INT = 1;
        public readonly static int AT_SYMBOL = 2;
        public readonly static int AT_IDENTIFIER = 3;
        public readonly static int AT_STRING = 4;

        public static SValue ParseText(string s)
        {
            SVList root = new SVList();
            SVList current = root;
            StringBuilder sb = new StringBuilder();
            int mode = 0;
            int charNo = -1;
            List<SVList> stack = new List<SVList>();
            stack.Add(root);

            root.list.Add(new SVToken("exec"));

            foreach (char c in s)
            {
                charNo++;
                if (mode == 0)
                {
                    if (Char.IsWhiteSpace(c))
                    {
                    }
                    else if (c == '(')
                    {
                        current = enterNewSublist(current, stack, true);
                    }
                    else if (c == ')')
                    {
                        current = returnToParent(current, stack);
                    }
                    else if (c == '\'')
                    {
                        mode = 8;
                    }
                    else if (c == '\"')
                    {
                        mode = 6;
                    }
                    else if (c == ';')
                    {
                        mode = 9;
                    }
                    else
                    {
                        mode = 1;
                        sb.Append(c);
                    }
                }
                else if (mode == 1)
                {
                    bool b1 = false;
                    bool b2 = false;
                    bool b3 = false;
                    if (c == '(')
                    {
                        b1 = true;
                        b2 = true;
                        mode = 0;
                    }
                    else if (c == ')')
                    {
                        b1 = true;
                        b3 = true;
                        mode = 0;
                    }
                    else if (Char.IsWhiteSpace(c))
                    {
                        b1 = true;
                        mode = 0;
                    }
                    else
                    {
                        sb.Append(c);
                    }

                    if (b1)
                    {
                        addBufferToCurrentNode(current, sb);
                    }
                    if (b2)
                    {
                        current = enterNewSublist(current, stack, true);
                    }
                    if (b3)
                    {
                        current = returnToParent(current, stack);
                    }
                }
                else if (mode == 4)
                {
                    if (c == '\'')
                    {
                        addBufferToCurrentNode(current, sb, AT_STRING);
                        mode = 8;
                    }
                    else if (Char.IsWhiteSpace(c))
                    {
                        addBufferToCurrentNode(current, sb, AT_STRING);
                        mode = 0;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else if (mode == 5)
                {
                    if (c == 'n') sb.Append('\n');
                    else if (c == 't') sb.Append('\t');
                    else sb.Append(c);
                    mode = 4;
                }
                else if (mode == 6)
                {
                    if (c == '\"')
                    {
                        addBufferToCurrentNode(current, sb, AT_STRING);
                        mode = 0;
                    }
                    else if (c == '\\')
                    {
                        mode = 7;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else if (mode == 7)
                {
                    if (c == 'n') sb.Append('\n');
                    else if (c == 't') sb.Append('\t');
                    else sb.Append(c);
                    mode = 6;
                }
                else if (mode == 8)
                {
                    if (c == '(')
                    {
                        current = enterNewSublist(current, stack, false);
                        mode = 0;
                    }
                    else if (Char.IsLetter(c))
                    {
                        sb.Append(c);
                        mode = 4;
                    }
                    else if (c == ')')
                    {
                        current = returnToParent(current, stack);
                        mode = 0;
                    }
                    else
                    {
                        mode = 0;
                    }
                }
                else if (mode == 9)
                {
                    if (c == '\n')
                    {
                        mode = 0;
                    }
                }
            }

            root.ReorderIds(1);

            return root;
        }

        private static SVList returnToParent(SVList current, List<SVList> stack)
        {
            if (stack.Count > 1)
                stack.RemoveAt(stack.Count - 1);
            current = stack[stack.Count - 1];
            return current;
        }

        private static SVList enterNewSublist(SVList current, List<SVList> stack, bool evaluable)
        {
            SVList nn = new SVList();
            nn.Evaluable = evaluable;
            stack.Add(nn);
            current.list.Add(nn);
            return nn;
        }

        private static int addBufferToCurrentNode(SVList current, StringBuilder sb)
        {
            double d;
            long l;
            int i;
            string val = sb.ToString();
            SValue sv = null;

            if (int.TryParse(val, out i))
            {
                sv = new SVInt32(i);
            }
            else if (long.TryParse(val, out l))
            {
                sv = new SVInt64(l);
            }
            else if (double.TryParse(val, out d))
            {
                sv = new SVDouble(d);
            }
            else
            {
                sv = new SVToken(val);
            }

            current.list.Add(sv);
            sb.Clear();
            return 0;
        }


        private static int addBufferToCurrentNode(SVList current, StringBuilder sb, int nt)
        {
            SValue sv = null;
            if (nt == AT_IDENTIFIER)
            {
                sv = new SVToken(sb.ToString());
            }
            else if (nt == AT_INT)
            {
                int v = 0;
                if (int.TryParse(sb.ToString(), out v))
                {
                    sv = new SVInt32(v);
                }
                else
                {
                    throw new Exception("Cannot parse string " + sb.ToString() + " into integer");
                }
            }
            else if (nt == AT_STRING)
            {
                sv = new SVString(sb.ToString());
            }

            current.list.Add(sv);
            sb.Clear();
            return 0;
        }


        /// <summary>
        /// External delegate allows to define user-sepcific operations
        /// </summary>
        public ScriptingDelegate externalDelegate = null;

        public StringBuilder logtext = new StringBuilder();

        public SValue resultTree = null;

        /// <summary>
        /// Main function for executing script
        /// </summary>
        /// <param name="script"></param>
        public void Execute(string script, ScriptingDelegate dlg)
        {
            SValue node = ParseText(script);

            Execute(node, dlg);
        }

        /// <summary>
        /// Main function for executing script
        /// </summary>
        /// <param name="script"></param>
        /// <param name="dlg"></param>
        public void Execute(SValue script, ScriptingDelegate dlg)
        {
            SValue node = script;
            ScriptingSpace space = new ScriptingSpace();

            resultTree = node;
            PrepareExecution(dlg, space);


            // execute
            ExecuteNode(space, node);
        }

        public void PrepareExecution(ScriptingDelegate dlg, ScriptingSpace space)
        {

            // add built-in datatypes
            AddDefaultDataTypes(space);

            // needs to be inserted space level at this point, before initialization of 
            // space by delegate, because delegate can define its own global variables
            space.pushLevel();

            // initialize with delegate
            externalDelegate = dlg;
            externalDelegate.InitWithSpace(space);
        }

        private void AddDefaultDataTypes(ScriptingSpace space)
        {
            space.AddDatatype("short", new SVInt16());
            space.AddDatatype("Int16", new SVInt16());
            space.AddDatatype("int", new SVInt32());
            space.AddDatatype("Int32", new SVInt32());
            space.AddDatatype("long", new SVInt64());
            space.AddDatatype("Int64", new SVInt64());
            space.AddDatatype("double", new SVDouble());
            space.AddDatatype("char", new SVChar());
            space.AddDatatype("bool", new SVBoolean());
            space.AddDatatype("boolean", new SVBoolean());
            space.AddDatatype("byte", new SVByte());
            space.AddDatatype("string", new SVString());
            space.AddDatatype("list", new SVList());
        }

        /// <summary>
        /// Main function for executing script
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SValue ExecuteNode(ScriptingSpace space, SValue node)
        {
            if (node is SVList)
            {
                SVList nl = node as SVList;
                if (nl.Evaluable == false)
                    return node;
                SValue fn = nl.getHead();
                if (fn is SVToken)
                {
                    return ExecuteTokenList(space, nl);
                }

                // first child is neither token nor symbol
                // so get script value of the first child
                // and try to execute class method for that value
                return ExecuteList(space, nl);

                //throw new Exception(string.Format("List node must start with token or symbol. This occured at {0}.", node.nodeId));
            }
            else if (node is SVToken)
            {
                SVToken st = node as SVToken;
                if (space.ContainsObject(st.sValue))
                    return space.GetValue(st.sValue);
                throw new Exception("Cannot evaluate symbol " + st.sValue);
            }

            return node;
        }

        /// <summary>
        /// First evaluates first child of the node
        /// Then all remaining children considers as method + arguments for the evaluated first child 
        /// </summary>
        /// <param name="space"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private SValue ExecuteList(ScriptingSpace space, SVList node)
        {
            SValue sv = null;
            SValue head = node.getHead();
            if (head != null)
            {
                sv = ExecuteNode(space, head);
                if (sv != null)
                {
                    if (node.list.Count > 1)
                    {
                        SVToken st = null;
                        if (node.list[1] is SVToken)
                            st = (node.list[1] as SVToken);
                        else
                            throw new Exception("Expected method name as first argument for object at " + node.nodeId);
                        sv = sv.ExecuteMethod(this, space, st.sValue, node.SublistFrom(2));
                    }
                }
                if (sv == null)
                    throw new Exception(string.Format("Unknown method for value of first child at {0}", node.nodeId));

                return sv;
            }

            return space.nullValue;
        }

        /// <summary>
        /// Evaluates list which starts with token (non-parenthised string).
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private SValue ExecuteTokenList(ScriptingSpace space, SVList node)
        {
            SVToken head = node.getHead() as SVToken;
            ScriptingDefun funcDef = null;
            SValue sv = null;

            switch (head.sValue)
            {
                case "defun":
                    return execDefun(space, node);
                case "exec":
                    return execExec(space, node);
                case "if":
                    return execIf(space, node);
                case "while":
                    return execWhile(space, node);
                case "break":
                    return execBreak(space);
                case "return":
                    return execReturn(space, node);
                case "for":
                    return execFor(space, node);
                case "new":
                    return execNew(space, node);
                case "print":
                    return execPrint(space, node);
                case "+":
                case "sum":
                    return execPlus(space, node);
                case "-":
                case "sub":
                    return execMinus(space, node);
                case "/":
                case "div":
                    return execDivide(space, node);
                case "%":
                case "mod":
                    return execModulo(space, node);
                case "*":
                case "mult":
                    return execMultiply(space, node);
                case "**":
                case "pow":
                    return execPow(space, node);
                case "max":
                    return execMax(space, node);
                case "min":
                    return execMin(space, node);
                case "avg":
                    return execAvg(space, node);
                case ">": case "<":
                case "<=": case ">=":
                case "=": case "set":
                case "!=": case "ne":
                case "==": case "eq":
                case "&": case "and":
                case "or": case "xor": case "not":
                case "&&": case "|":
                case "||": case "^":
                case "^^": case "!":
                case "~~": case "!~":
                    return execMethod2(space, node, head.sValue);
                case "++":
                case "--":
                    return execMethod1(space, node, head.sValue);
                default:
                    funcDef = space.FindFunction(head.sValue);
                    if (funcDef != null)
                    {
                        // creates duplication of variable space
                        // because we share variables and their values
                        // from the level of function definition
                        // and not level of function execution
                        ScriptingSpace funcSpace = space.DuplicateUpToFunction(head.sValue);
                        return ExecuteFunction(funcSpace, funcDef, node);
                    }

                    // try name of variable
                    // if token is name of variable, then get its value
                    // and run object method
                    sv = space.GetValue(head.sValue);
                    if (sv != null)
                    {
                        if (node.list.Count > 1)
                        {
                            SVToken st = null;
                            if (node.list[1] is SVToken)
                                st = (node.list[1] as SVToken);
                            else
                                throw new Exception("Expected method name as first argument for object at " + node.nodeId);
                            sv = sv.ExecuteMethod(this, space, st.sValue, node.SublistFrom(2));
                        }

                        if (sv != null)
                            return sv;
                    }

                    // last chance is, that procedure is defined in delegate
                    if (externalDelegate != null)
                    {
                        return externalDelegate.ExecuteTokenList(this, space, node);
                    }

                    break;
            }

            return space.nullValue;
        }

        public SValue ExecuteFunction(ScriptingSpace space, ScriptingDefun funcDef, SVList node)
        {
            List<SValue> parts = node.list;
            SValue sv = space.nullValue;

            space.pushLevel();
            int i = 1;
            // copying arguments
            foreach (string s in funcDef.Args)
            {
                if (parts.Count > i)
                {
                    space.SetValue(s, ExecuteNode(space, parts[i]));
                }
                else
                {
                    space.SetValue(s, space.nullValue);
                }
                i++;
            }

            sv = ExecuteNode(space, funcDef.Body);
            sv.shouldExit = false;

            space.popLevel();

            return sv;
        }

        private bool listIsTwoTokens(SValue sv)
        {
            if (!(sv is SVList))
            {
                return false;
            }
            SVList svl = sv as SVList;
            if (svl.list.Count != 2)
                return false;
            return ((svl.list[0] is SVToken) && (svl.list[1] is SVToken));
        }

        /// <summary>
        /// Definition of the new function
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private SValue execDefun(ScriptingSpace space, SVList node)
        {
            SVToken name = null;
            SVList args = null;
            SValue body = null;

            if (node.list.Count < 1 || !(node.list[0] is SVToken))
            {
                throw new Exception(string.Format("Expected name of function for function definition at {0}", node.nodeId));
            }
            name = node.list[0] as SVToken;

            if (node.list.Count < 2 || !(node.list[1] is SVList))
            {
                throw new Exception(string.Format("Expected list of arguments for function definition at {0}", node.nodeId));
            }
            args = node.list[1] as SVList;

            if (node.list.Count < 3)
            {
                throw new Exception(string.Format("Expected body of function (as list) for function definition at {0}", node.nodeId));
            }
            body = node.list[2];

            ScriptingDefun sd = new ScriptingDefun();
            sd.Name = name.sValue;
            sd.Args = new List<string>();
            foreach (SValue tn in args.list)
            {
                if (tn is SVToken)
                {
                    sd.Args.Add((tn as SVToken).sValue);
                    sd.ArgTypes.Add(string.Empty);
                }
                else if (listIsTwoTokens(tn))
                {
                    SVList sla = tn as SVList;
                    sd.ArgTypes.Add(sla.list[0].getStringValue());
                    sd.Args.Add(sla.list[1].getStringValue());
                }
                else
                {
                    throw new Exception("List of arguments should contain items, where item is either token or list of 2 tokens");
                }
            }
            sd.Body = body;
            space.addFunctionDefinition(sd);

            return space.nullValue;
        }

        /// <summary>
        /// Execution of all nodes after keyword 'exec'
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private SValue execExec(ScriptingSpace space, SVList list)
        {
            bool firstWas = false;
            SValue sc = space.nullValue;
            foreach (SValue tn in list.list)
            {
                if (firstWas)
                {
                    sc = ExecuteNode(space, tn);
                }
                else
                {
                    firstWas = true;
                }
            }

            return sc;
        }

        private SValue execFor(ScriptingSpace space, SVList node)
        {
            List<SValue> parts = node.list;
            if (parts.Count == 4)
            {
                string varName;
                if (!(parts[1] is SVToken))
                {
                    throw new Exception(string.Format("Expected token at second position in command FOR at {0}", node.nodeId));
                }
                varName = (parts[1] as SVToken).sValue;
                SValue sv = ExecuteNode(space, parts[2]);
                if (sv is SVList)
                {
                    space.pushLevel();
                    List<SValue> list = (sv as SVList).list;
                    foreach (SValue val in list)
                    {
                        space.SetValue(varName, val);
                        sv = ExecuteNode(space, parts[3]);
                        if (sv.shouldBreak)
                        {
                            sv.shouldBreak = false;
                            break;
                        }
                        else if (sv.shouldExit)
                        {
                            space.popLevel();
                            return sv;
                        }
                    }
                    space.popLevel();
                }

                return space.nullValue;
            }
            else if (parts.Count < 3)
            {
                throw new Exception(string.Format("Too few parts of FOR construction at {0}", node.nodeId));
            }
            else if (parts.Count > 3)
            {
                throw new Exception(string.Format("Too many parts of FOR construction at {0}", node.nodeId));
            }

            return space.nullValue;

        }

        private SValue execIf(ScriptingSpace space, SVList node)
        {
            List<SValue> parts = node.list;
            SVNumber sn;
            if (parts.Count == 3)
            {
                SValue sv = ExecuteNode(space, parts[1]);
                SVNumber.AssertIsNumber(sv);
                sn = sv as SVNumber;
                if (sn.getBoolValue())
                {
                    sv = ExecuteNode(space, parts[2]);
                }
                else
                {
                    sv = space.nullValue;
                }
                return sv;
            }
            else if (parts.Count == 4)
            {
                SValue sv = ExecuteNode(space, parts[1]);
                SVNumber.AssertIsNumber(sv);
                sn = sv as SVNumber;
                if (sn.getBoolValue())
                {
                    sv = ExecuteNode(space, parts[2]);
                }
                else
                {
                    sv = ExecuteNode(space, parts[3]);
                }
                return sv;
            }
            else if (parts.Count < 3)
            {
                throw new Exception(string.Format("Too few parts of IF construction at {0}", node.nodeId));
            }
            else if (parts.Count > 4)
            {
                throw new Exception(string.Format("Too many parts of IF construction at {0}", node.nodeId));
            }

            return space.nullValue;
        }

        private SValue execWhile(ScriptingSpace space, SVList node)
        {
            List<SValue> parts = node.list;
            SVNumber sn;

            if (parts.Count == 3)
            {
                space.pushLevel();
                SValue sv = ExecuteNode(space, parts[1]);
                SVNumber.AssertIsNumber(sv);
                sn = sv as SVNumber;
                while (sn.getBoolValue())
                {
                    sv = ExecuteNode(space, parts[2]);
                    if (sv.shouldBreak)
                    {
                        sv.shouldBreak = false;
                        break;
                    }
                    else if (sv.shouldExit)
                    {
                        space.popLevel();
                        return sv;
                    }
                    sv = ExecuteNode(space, parts[1]);
                    SVNumber.AssertIsNumber(sv);
                    sn = sv as SVNumber;
                }
                space.popLevel();
                return space.nullValue;
            }
            else if (parts.Count < 3)
            {
                throw new Exception(string.Format("Too few parts of WHILE construction at {0}", node.nodeId));
            }
            else if (parts.Count > 3)
            {
                throw new Exception(string.Format("Too many parts of WHILE construction at {0}", node.nodeId));
            }

            return space.nullValue;
        }

        private SValue execBreak(ScriptingSpace space)
        {
            SValue sv = new SValue();
            sv.shouldBreak = true;
            return sv;
        }

        private SValue execReturn(ScriptingSpace space, SVList node)
        {
            List<SValue> parts = node.list;
            if (parts.Count == 2)
            {
                SValue sv = ExecuteNode(space, parts[1]);
                sv.shouldExit = true;
                return sv;
            }
            else if (parts.Count == 1)
            {
                SValue sv = new SValue();
                sv.shouldExit = true;
                return sv;
            }
            else
            {
                throw new Exception(string.Format("Too many arguments for return at {0}", node.nodeId));
            }
        }

        private SValue execPrint(ScriptingSpace space, SVList node)
        {
            List<SValue> parts = node.list;
            for (int j = 1; j < node.list.Count; j++)
            {
                SValue sv = ExecuteNode(space, parts[j]);
                Debugger.Log(0, "", sv.getStringValue());
                logtext.AppendFormat("{0}", sv.getStringValue());
            }

            return space.nullValue;
        }

        private int getMinNumberLevel(SVList node)
        {
            int nl = 10;
            if (node.list != null)
            {
                for (int i = 1; i < node.list.Count; i++)
                {
                    SValue sv = node.list[i];
                    if (sv is SVDouble)
                        nl = Math.Min(3, nl);
                    else if (sv is SVInt64)
                        nl = Math.Min(2, nl);
                    else if (sv is SVInt32)
                        nl = Math.Min(1, nl);
                    else
                        nl = Math.Min(0, nl);
                }
            }
            return nl;
        }

        private int getMaxNumberLevel(SVList node)
        {
            int nl = 0;
            if (node.list != null)
            {
                for (int i = 1; i < node.list.Count; i++)
                {
                    SValue sv = node.list[i];
                    if (sv is SVDouble)
                        nl = Math.Max(3, nl);
                    else if (sv is SVInt64)
                        nl = Math.Max(2, nl);
                    else if (sv is SVInt32)
                        nl = Math.Max(1, nl);
                }
            }
            return nl;
        }

        private SValue execPlus(ScriptingSpace space, SVList node)
        {
            SValue sv = null;
            node.AssertMinCount(3);
            int nl = getMaxNumberLevel(node);
            if (nl == 1)
            {
                node.AssertIsNumber(1);
                int result = node.list[1].getIntValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result += node.list[j].getIntValue();
                }
                sv = new SVInt32(result);
            }
            else if (nl == 2)
            {
                long result = node.list[1].getLongValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result += node.list[j].getLongValue();
                }
                sv = new SVInt64(result);
            }
            else if (nl == 3)
            {
                double result = node.list[1].getDoubleValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result += node.list[j].getDoubleValue();
                }
                sv = new SVDouble(result);
            }

            return sv;
        }

        private SValue execMinus(ScriptingSpace space, SVList node)
        {
            SValue sv = null;
            if (node.list == null || node.list.Count < 3)
                throw new Exception("Operator should have at least 2 arguments at " + node.nodeId);
            int nl = getMaxNumberLevel(node);
            if (nl == 1)
            {
                int result = node.list[1].getIntValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result -= node.list[j].getIntValue();
                }
                sv = new SVInt32(result);
            }
            else if (nl == 2)
            {
                long result = node.list[1].getLongValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result -= node.list[j].getLongValue();
                }
                sv = new SVInt64(result);
            }
            else if (nl == 3)
            {
                double result = node.list[1].getDoubleValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result -= node.list[j].getDoubleValue();
                }
                sv = new SVDouble(result);
            }

            return sv;
        }

        private SValue execMultiply(ScriptingSpace space, SVList node)
        {
            SValue sv = null;
            if (node.list == null || node.list.Count < 3)
                throw new Exception("Operator should have at least 2 arguments at " + node.nodeId);
            int nl = getMaxNumberLevel(node);
            if (nl == 1)
            {
                int result = node.list[1].getIntValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result *= node.list[j].getIntValue();
                }
                sv = new SVInt32(result);
            }
            else if (nl == 2)
            {
                long result = node.list[1].getLongValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result *= node.list[j].getLongValue();
                }
                sv = new SVInt64(result);
            }
            else if (nl == 3)
            {
                double result = node.list[1].getDoubleValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result *= node.list[j].getDoubleValue();
                }
                sv = new SVDouble(result);
            }

            return sv;
        }

        private SValue execDivide(ScriptingSpace space, SVList node)
        {
            SValue sv = null;
            if (node.list == null || node.list.Count < 3)
                throw new Exception("Operator should have at least 2 arguments at " + node.nodeId);
            int nl = getMaxNumberLevel(node);
            if (nl == 1)
            {
                int result = node.list[1].getIntValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result /= node.list[j].getIntValue();
                }
                sv = new SVInt32(result);
            }
            else if (nl == 2)
            {
                long result = node.list[1].getLongValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result /= node.list[j].getLongValue();
                }
                sv = new SVInt64(result);
            }
            else if (nl == 3)
            {
                double result = node.list[1].getDoubleValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result /= node.list[j].getDoubleValue();
                }
                sv = new SVDouble(result);
            }

            return sv;
        }

        private SValue execModulo(ScriptingSpace space, SVList node)
        {
            SValue sv = null;
            if (node.list == null || node.list.Count < 3)
                throw new Exception("Operator should have at least 2 arguments at " + node.nodeId);
            int nl = getMaxNumberLevel(node);
            if (nl == 1)
            {
                int result = node.list[1].getIntValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result %= node.list[j].getIntValue();
                }
                sv = new SVInt32(result);
            }
            else if (nl == 2)
            {
                long result = node.list[1].getLongValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result %= node.list[j].getLongValue();
                }
                sv = new SVInt64(result);
            }
            else if (nl == 3)
            {
                double result = node.list[1].getDoubleValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result %= node.list[j].getDoubleValue();
                }
                sv = new SVDouble(result);
            }

            return sv;
        }

        private SValue execMax(ScriptingSpace space, SVList node)
        {
            SValue sv = null;
            if (node.list == null || node.list.Count < 3)
                throw new Exception("Operator should have at least 2 arguments at " + node.nodeId);
            int nl = getMaxNumberLevel(node);
            if (nl == 1)
            {
                int result = node.list[1].getIntValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result = Math.Max(result, node.list[j].getIntValue());
                }
                sv = new SVInt32(result);
            }
            else if (nl == 2)
            {
                long result = node.list[1].getLongValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result = Math.Max(result, node.list[j].getLongValue());
                }
                sv = new SVInt64(result);
            }
            else if (nl == 3)
            {
                double result = node.list[1].getDoubleValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result = Math.Max(result, node.list[j].getDoubleValue());
                }
                sv = new SVDouble(result);
            }

            return sv;
        }

        private SValue execMin(ScriptingSpace space, SVList node)
        {
            SValue sv = null;
            if (node.list == null || node.list.Count < 3)
                throw new Exception("Operator should have at least 2 arguments at " + node.nodeId);
            int nl = getMaxNumberLevel(node);
            if (nl == 1)
            {
                int result = node.list[1].getIntValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result = Math.Min(result, node.list[j].getIntValue());
                }
                sv = new SVInt32(result);
            }
            else if (nl == 2)
            {
                long result = node.list[1].getLongValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result = Math.Min(result, node.list[j].getLongValue());
                }
                sv = new SVInt64(result);
            }
            else if (nl == 3)
            {
                double result = node.list[1].getDoubleValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result = Math.Min(result, node.list[j].getDoubleValue());
                }
                sv = new SVDouble(result);
            }

            return sv;
        }

        private SValue execAvg(ScriptingSpace space, SVList node)
        {
            SValue sv = null;
            if (node.list == null || node.list.Count < 3)
                throw new Exception("Operator should have at least 2 arguments at " + node.nodeId);
            int nl = getMaxNumberLevel(node);
            double cnt = 1;
            if (nl == 1)
            {
                int result = node.list[1].getIntValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result += node.list[j].getIntValue();
                    cnt++;
                }
                sv = new SVDouble(result/cnt);
            }
            else if (nl == 2)
            {
                long result = node.list[1].getLongValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result += node.list[j].getLongValue();
                    cnt++;
                }
                sv = new SVDouble(result/cnt);
            }
            else if (nl == 3)
            {
                double result = node.list[1].getDoubleValue();
                for (int j = 2; j < node.list.Count; j++)
                {
                    result += node.list[j].getDoubleValue();
                    cnt++;
                }
                sv = new SVDouble(result/cnt);
            }

            return sv;
        }

        private SValue execPow(ScriptingSpace space, SVList node)
        {
            if (node.list == null || node.list.Count != 3)
                throw new Exception("Operator should have 2 argument at " + node.nodeId);
            double a = node.list[1].getDoubleValue();
            double b = node.list[2].getDoubleValue();
            return new SVDouble(Math.Pow(a,b));
        }

        private SValue execMethod1(ScriptingSpace space, SVList node, string method)
        {
            if (node.list == null || node.list.Count != 2)
                throw new Exception("Operator should have 1 argument at " + node.nodeId);

            return node.list[1].ExecuteMethod(this, space, method, node.SublistFrom(2));
        }

        private SValue execMethod2(ScriptingSpace space, SVList node, string method)
        {
            if (node.list == null || node.list.Count != 3)
                throw new Exception("Operator should have 2 argument at " + node.nodeId);

            return node.list[1].ExecuteMethod(this, space, method, node.SublistFrom(2));
        }

        /// <summary>
        /// Creating instance of datatype, with (possibly) arguments
        /// </summary>
        /// <param name="node">List node, children of this node are starting with keyword 'new'</param>
        /// <returns>Instance of given data type</returns>
        private SValue execNew(ScriptingSpace space, SVList node)
        {
            List<SValue> parts = node.list;
            if (parts.Count >= 2)
            {
                string dataTypeName = "";
                // second position is datatype name
                if (!(parts[1] is SVToken))
                {
                    throw new Exception(string.Format("Expected datatype name at second position in NEW statement at {0}", node.nodeId));
                }
                dataTypeName = (parts[1] as SVToken).sValue;

                // third up to last position
                // contains optional arguments
                SVList scl = new SVList();
                for (int i = 2; i < parts.Count; i++)
                {
                    scl.list.Add(ExecuteNode(space, parts[i]));
                }

                SVDataType sdt = null;
                SValue returnValue = null;

                // looking for datatype in space's definitions
                if (space.dataTypes.ContainsKey(dataTypeName))
                {
                    sdt = space.dataTypes[dataTypeName];
                    returnValue = sdt.CreateInstance(scl.list);
                }
                else if (externalDelegate != null)
                {
                    // looking for datatype in external delegate
                    returnValue = externalDelegate.createInstance(this, dataTypeName, scl.list);
                }

                // if returnValue is null, that means datatype was not found
                if (returnValue == null)
                {
                    throw new Exception("Unknown type " + dataTypeName + " in NEW statement at " + node.nodeId);
                }

                return returnValue;
            }

            return space.nullValue;
        }
    }
}
