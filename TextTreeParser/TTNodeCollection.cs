using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrepInterpreter;

namespace TextTreeParser
{
    public class TTNodeCollection: SValue, IEnumerable
    {
        public TreeNodeRef first = null;
        public TreeNodeRef last = null;

        public class TreeNodeRef
        {
            public TTNode node = null;
            public TreeNodeRef next = null;
            public TreeNodeRef prev = null;

            public TreeNodeRef()
            {
            }

            public TreeNodeRef(TTNode n)
            {
                this.node = n;
            }


        }


        public override SValue CreateInstance(List<SValue> args)
        {
            return new TTNodeCollection();
        }

        public override SValue ExecuteMethod(Scripting parent, ScriptingSpace space, string method, SVList args)
        {
            if (method.Equals("getNodeList"))
            {
                SVList list = new SVList();
                list.nodeId = nodeId;
                list.list.AddRange(getNodeList());
                return list;
            }
            else if (method.Equals("count"))
            {
                return new SVInt32(count());
            }
            else if (method.Equals("first"))
            {
                return firstChild() ?? space.nullValue;
            }
            else if (method.Equals("last"))
            {
                return (last != null) ? last.node : space.nullValue;
            }
            else if (method.Equals("addNode"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTNode));
                addNode(args.list[0] as TTNode);
                return space.nullValue;
            }
            else if (method.Equals("addCollection"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTNodeCollection));
                addCollection(args.list[0] as TTNodeCollection);
                return space.nullValue;
            }
            else if (method.Equals("removeNode"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTNode));
                removeNode(args.list[0] as TTNode);
                return space.nullValue;
            }
            else if (method.Equals("removeFirstNode"))
            {
                return removeFirstNode();
            }
            else if (method.Equals("removeLastNode"))
            {
                return removeLastNode();
            }
            else if (method.Equals("removeNodesOfType"))
            {
                args.AssertCount(1);
                removeNodesOfType(args.list[0].getStringValue());
                return space.nullValue;
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
            else if (method.Equals("checkIsSubnode"))
            {
                args.AssertCount(1);
                args.AssertIsType(0, typeof(TTNode));
                return new SVBoolean(checkIsSubnode(args.list[0] as TTNode));
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
            else if (method.Equals("createEncapsulationNode"))
            {
                args.AssertCount(1);
                return createEncapsulationNode(args.list[0].getStringValue());
            }

            return base.ExecuteMethod(parent, space, method, args);
        }


        public List<TTNode> getNodeList()
        {
            List<TTNode> list = new List<TTNode>();
            foreach (TTNode tn in this)
            {
                list.Add(tn);
            }
            return list;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public TTTreeNodeCollectionEnumerator GetEnumerator()
        {
            return new TTTreeNodeCollectionEnumerator(this.first);
        }

        public int count()
        {
            TreeNodeRef tnr = this.first;
            int count = 0;

            while (tnr != null)
            {
                count++;
                tnr = tnr.next;
            }

            return count;
        }

        public TTNode firstChild()
        {
            if (first != null)
                return first.node;
            return null;
        }

        public void addNode(TTNode node)
        {
            TreeNodeRef refn = new TreeNodeRef(node);
            if (first == null)
            {
                first = refn;
                last = refn;
            }
            else
            {
                last.next = refn;
                refn.prev = last;
                last = refn;
            }
        }

        public void addCollection(TTNodeCollection nodes)
        {
            foreach (TTNode node in nodes)
            {
                addNode(node);
            }
        }

        /// <summary>
        /// Removes node from collection. Node is given by reference.
        /// </summary>
        /// <param name="tn"></param>
        public void removeNode(TTNode tn)
        {
            if (this.first == null)
                return;

            if (this.first.node == tn)
            {
                if (this.last == this.first)
                {
                    this.first = null;
                    this.last = null;
                }
                else
                {
                    removeFirstNode();
                }
            }
            else if (this.last.node == tn)
            {
                removeLastNode();
            }
            else
            {
                TreeNodeRef item = this.first;
                while (item != null)
                {
                    if (item.node == tn)
                    {
                        removeNodeRef(item);
                        break;
                    }
                    item = item.next;
                }
            }
        }

        private void removeNodeRef(TreeNodeRef item)
        {
            TreeNodeRef tprev = item.prev;
            TreeNodeRef tnext = item.next;
            if (tprev != null)
            {
                tprev.next = tnext;
            }
            if (tnext != null)
            {
                tnext.prev = tprev;
            }
            if (item == this.first)
            {
                this.first = tnext;
            }
            if (item == this.last)
            {
                this.last = tprev;
            }
        }

        /// <summary>
        /// Removes head of collection
        /// </summary>
        public TTNode removeFirstNode()
        {
            TTNode ret = null;
            TreeNodeRef temp = this.first;
            if (temp != null)
            {
                this.first = this.first.next;
                this.first.prev = null;
                temp.next = null;
                ret = temp.node;
                temp = null;
            }
            return ret;
        }

        /// <summary>
        /// Removes tail of collection
        /// </summary>
        public TTNode removeLastNode()
        {
            TTNode ret = null;
            TreeNodeRef temp = this.last;
            if (temp != null)
            {
                this.last = this.last.prev;
                this.last.next = null;
                temp.prev = null;
                ret = temp.node;
                temp = null;
            }
            return ret;
        }


        public void removeNodesOfType(string subType)
        {
            TreeNodeRef tn = this.first;
            while (tn != null)
            {
                if (tn.node != null //&& tn.node.atom != null
                    && tn.node.Name.Equals(subType))
                {
                    removeNodeRef(tn);
                }

                tn = tn.next;
            }
        }

        private TreeNodeRef findNodeRef(TTNode node)
        {
            if (node == null)
                return null;
            TreeNodeRef r = this.first;
            while (r != null)
            {
                if (r.node == node)
                {
                    return r;
                }
                r = r.next;
            }

            return null;
        }

        private TreeNodeRef findNodeRefBack(TTNode node)
        {
            if (node == null)
                return null;
            TreeNodeRef r = this.last;
            while (r != null)
            {
                if (r.node == node)
                {
                    return r;
                }
                r = r.prev;
            }

            return null;
        }

        /// <summary>
        /// Finding node either from the beginning of node's children
        /// or from given 'sinceChild' node - in this case 'sinceChild' is not included in search.
        /// </summary>
        /// <param name="name">Name of tree node, or null if not checked</param>
        /// <param name="atomType">Type of atom or null if not checked</param>
        /// <param name="atomValue">Value of atom or null if not checked</param>
        /// <returns>Returns node with given characteristics</returns>
        public TTNode findNodeForward(string atomType, string atomValue, TTNode sinceChild)
        {
            TreeNodeRef wi = findNodeRef(sinceChild);
            if (wi == null)
                wi = this.first;
            else
                wi = wi.next;

            while(wi != null)
            {
                if (wi.node.canMatchAtom(atomType, atomValue))
                    return wi.node;
                wi = wi.next;
            }

            return null;
        }

        /// <summary>
        /// Finding node either from the end of node's children
        /// or from given 'sinceChild' node - in this case 'sinceChild' is not included in search.
        /// </summary>
        /// <param name="name">Name of tree node, or null if not checked</param>
        /// <param name="atomType">Type of atom or null if not checked</param>
        /// <param name="atomValue">Value of atom or null if not checked</param>
        /// <returns>Returns node with given characteristics</returns>
        public TTNode findNodeBackward(string atomType, string atomValue, TTNode sinceChild)
        {
            TreeNodeRef wi = findNodeRefBack(sinceChild);
            if (wi == null)
                wi = this.last;
            else
                wi = wi.prev;

            while (wi != null)
            {
                if (wi.node != null && wi.node.canMatchAtom(atomType, atomValue))
                    return wi.node;
                wi = wi.prev;
            }

            return null;
        }

        public bool checkIsSubnode(TTNode dividerNode)
        {
            TreeNodeRef tr = findNodeRef(dividerNode);
            return tr != null;
        }

        public TTNodeCollection takeChildrenBefore(TTNode tn)
        {
            TTNodeCollection tnc = new TTNodeCollection();
            TreeNodeRef iter = this.first;
            bool a = false;
            if (tn != iter.node)
            {
                while (iter != null)
                {
                    if (iter.node == tn)
                    {
                        a = true;
                    }
                    else if (!a)
                    {
                        tnc.addNode(iter.node);
                        iter.node = null;
                    }
                    iter = iter.next;
                }
            }

            while (this.first != null && this.first.node == null)
            {
                removeFirstNode();
            }

            return tnc;
        }

        public TTNodeCollection takeChildrenAfter(TTNode tn)
        {
            TTNodeCollection tnc = new TTNodeCollection();
            TreeNodeRef iter = this.first;
            bool a = false;
            if (tn != iter.node)
            {
                while (iter != null)
                {
                    if (iter.node == tn)
                    {
                        a = true;
                    }
                    else if (a)
                    {
                        tnc.addNode(iter.node);
                        iter.node = null;
                    }
                    iter = iter.next;
                }
            }

            while (this.first != null && this.first.node == null)
            {
                removeLastNode();
            }

            return tnc;
        }

        public TTNode createEncapsulationNode(string name)
        {
            TTNode tn = new TTNode(name);
            tn.Children = this;
            return tn;
        }


    }

    public class TTTreeNodeCollectionEnumerator: IEnumerator
    {
        public TTNodeCollection.TreeNodeRef original = null;
        public TTNodeCollection.TreeNodeRef collection = null;

        public TTTreeNodeCollectionEnumerator(TTNodeCollection.TreeNodeRef col)
        {
            original = new TTNodeCollection.TreeNodeRef();
            original.next = col;
            collection = original;
        }

        public bool MoveNext()
        {
            if (collection != null)
                collection = collection.next;
            return (collection != null);
        }

        public void Reset()
        {
            collection = original;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public TTNode Current
        {
            get
            {
                return collection.node;
            }
        }
    }
}
