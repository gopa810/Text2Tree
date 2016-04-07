using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace TextTreeParser
{
    public class TTNode: SValue
    {
        //public TTAtom atom = null;
        public string Name = string.Empty;
        public string Value = string.Empty;

        // children nodes
        protected TTNodeCollection child = null;

        // constructors
        public TTNode()
        {
            //atom = null;
        }

        public TTNode(string name)
        {
            Name = name;
        }


        public override SValue CreateInstance(List<SValue> args)
        {
            if (args.Count == 1)
            {
                return new TTNode(args[0].getStringValue());
            }

            return new TTNode();
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("name"))
            {
                return new SVString(Name);
            }
            else if (method.Equals("value"))
            {
                return new SVString(Value);
            }
            else if (method.Equals("setName"))
            {
                args.AssertCount(1);
                Name = args.list[0].getStringValue();
                return args.list[0];
            }
            else if (method.Equals("setValue"))
            {
                args.AssertCount(1);
                Value = args.list[0].getStringValue();
                return args.list[0];
            }
            else if (method.Equals("children"))
            {
                return Children;
            }
            else if (method.Equals("addSubnode"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTNode));
                addSubnode(args.list[0] as TTNode);
                return args.list[0];
            }
            else if (method.Equals("addCollection"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTNodeCollection));
                addCollection(args.list[0] as TTNodeCollection);
                return args.list[0];
            }
            else if (method.Equals("canMatchAtom"))
            {
                args.AssertCount(2);
                string atomType = (args.list[0] is SVNull) ? null : args.list[0].getStringValue();
                string atomValue = (args.list[1] is SVNull) ? null : args.list[1].getStringValue();
                return new SVBoolean(canMatchAtom(atomType, atomValue));
            }
            else if (method.Equals("takeAllChildren"))
            {
                return takeAllChildren() ?? space.nullValue;
            }
            else if (method.Equals("takeChildrenBefore"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTNode));
                return takeChildrenBefore(args.list[0] as TTNode) ?? space.nullValue;
            }
            else if (method.Equals("takeChildrenAfter"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTNode));
                return takeChildrenBefore(args.list[0] as TTNode) ?? space.nullValue;
            }
            else if (method.Equals("findNodeForward"))
            {
                args.AssertCount(3);
                string atomType = null;
                string atomValue = null;
                TTNode startChild = null;
                if (!(args.list[0] is SVNull))
                    atomType = args.list[0].getStringValue();
                if (!(args.list[1] is SVNull))
                    atomValue = args.list[1].getStringValue();
                if (args.list[2] is TTNode)
                    startChild = args.list[2] as TTNode;
                return findNodeForward(atomType, atomValue, startChild) ?? space.nullValue;
            }
            else if (method.Equals("findNodeBackward"))
            {
                args.AssertCount(3);
                string atomType = null;
                string atomValue = null;
                TTNode startChild = null;
                if (!(args.list[0] is SVNull))
                    atomType = args.list[0].getStringValue();
                if (!(args.list[1] is SVNull))
                    atomValue = args.list[1].getStringValue();
                if (args.list[2] is TTNode)
                    startChild = args.list[2] as TTNode;
                return findNodeBackward(atomType, atomValue, startChild) ?? space.nullValue;
            }
            else if (method.Equals("removeHead"))
            {
                return removeHead() ?? space.nullValue;
            }
            else if (method.Equals("removeTail"))
            {
                return removeTail() ?? space.nullValue;
            }
            else if (method.Equals("getHead"))
            {
                return getHead() ?? space.nullValue;
            }
            else if (method.Equals("getTail"))
            {
                return getTail() ?? space.nullValue;
            }

            return base.ExecuteMethod(parent, space, method, args);
        }

        public TTNodeCollection Children
        {
            get
            {
                if (child == null)
                    return new TTNodeCollection();
                return child;
            }
            set
            {
                child = value;
            }
        }

        public void addSubnode(TTNode node)
        {
            if (child == null)
                child = new TTNodeCollection();
            child.addNode(node);
        }

        public void addCollection(TTNodeCollection col)
        {
            if (child == null)
                child = new TTNodeCollection(); 
            child.addCollection(col);
        }

        public bool canMatchAtom(string atomType, string atomValue)
        {
            if (atomType != null && !this.Name.Equals(atomType))
                return false;
            if (atomValue != null && !this.Value.Equals(atomValue))
                return false;

            return true;
        }

        public TTNodeCollection takeAllChildren()
        {
            TTNodeCollection tnc = this.child;
            if (tnc != null)
            {
                this.child = new TTNodeCollection();
            }
            return tnc;
        }

        public TTNodeCollection takeChildrenBefore(TTNode tn)
        {
            if (child == null)
                return null;

            if (this.child.checkIsSubnode(tn))
            {
                return this.child.takeChildrenBefore(tn);
            }

            return null;
        }

        public TTNodeCollection takeChildrenAfter(TTNode tn)
        {
            if (child == null)
                return null;

            if (this.child.checkIsSubnode(tn))
            {
                return this.child.takeChildrenAfter(tn);
            }

            return null;
        }

        public TTNode findNodeForward(string atomType, string atomValue, TTNode sinceChild)
        {
            if (child == null) return null;
            return child.findNodeForward(atomType, atomValue, sinceChild);
        }

        public TTNode findNodeBackward(string atomType, string atomValue, TTNode sinceChild)
        {
            if (child == null) return null;
            return child.findNodeBackward(atomType, atomValue, sinceChild);
        }

        public TTNode removeHead()
        {
            if (child == null) return null;
            return child.removeFirstNode();
        }

        public TTNode removeTail()
        {
            if (child != null)
                return child.removeLastNode();
            return null;
        }

        public TTNode getTail()
        {
            if (child == null)
                return null;
            if (child.last == null)
                return null;
            return child.last.node;
        }

        public TTNode getHead()
        {
            if (child == null)
                return null;
            if (child.first == null)
                return null;
            return child.first.node;
        }

        /// <summary>
        /// Adjustment of tree nodes
        /// This functions applies additional reducing operations for tree
        /// with the aim of reducing and optimization of tree
        /// Available operations are described in TTTreeAdjustmentRule class.
        /// </summary>
        /// <param name="treeAdjustmentRules"></param>
        public void AdjustTree(List<TTTreeAdjustmentRule> treeAdjustmentRules)
        {
            foreach (TTTreeAdjustmentRule rule in treeAdjustmentRules)
            {
                if (rule.rule == TTTreeAdjustmentRule.RULE_MERGE_CHILD_TYPE_TYPE)
                {
                    if (child == null)
                        continue;
                    if (this.child.count() == 1)
                    {
                        TTNode firstChild = this.child.firstChild();

                        if (firstChild != null)
                        {
                            if (rule.evaluate1(this.Name) && rule.evaluate2(firstChild.Name))
                            {
                                this.child = firstChild.child;
                                this.Name = firstChild.Name;
                                this.Value = firstChild.Value;
                                firstChild.child = null;
//                                firstChild.atom = null;
                            }
                        }
                    }
                }
            }

            foreach (TTNode tn in Children)
            {
                tn.AdjustTree(treeAdjustmentRules);
            }
        }

        public override int ReorderIds(int initialId)
        {
            nodeId = initialId;
            initialId++;

            foreach (TTNode tn in Children)
            {
                initialId = tn.ReorderIds(initialId);
            }

            return initialId;
        }
    }
}
