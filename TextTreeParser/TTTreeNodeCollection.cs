using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTreeParser
{
    public class TTTreeNodeCollection: IEnumerable
    {
        public TreeNodeRef first = null;
        public TreeNodeRef last = null;

        public class TreeNodeRef
        {
            public TTTreeNode node = null;
            public TreeNodeRef next = null;
            public TreeNodeRef prev = null;

            public TreeNodeRef()
            {
            }

            public TreeNodeRef(TTTreeNode n)
            {
                this.node = n;
            }


        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public TTTreeNodeCollectionEnumerator GetEnumerator()
        {
            return new TTTreeNodeCollectionEnumerator(this.first);
        }

        public void addNode(TTTreeNode node)
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

        public void addCollection(TTTreeNodeCollection nodes)
        {
            foreach (TTTreeNode node in nodes)
            {
                addNode(node);
            }
        }

        /// <summary>
        /// Removes node from collection. Node is given by reference.
        /// </summary>
        /// <param name="tn"></param>
        public void removeNode(TTTreeNode tn)
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
        public TTTreeNode removeFirstNode()
        {
            TTTreeNode ret = null;
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
        public TTTreeNode removeLastNode()
        {
            TTTreeNode ret = null;
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
                if (tn.node != null && tn.node.atom != null
                    && tn.node.atom.Type.Equals(subType))
                {
                    removeNodeRef(tn);
                }

                tn = tn.next;
            }
        }

        private TreeNodeRef findNodeRef(TTTreeNode node)
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

        private TreeNodeRef findNodeRefBack(TTTreeNode node)
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
        public TTTreeNode findNodeForward(string name, string atomType, string atomValue, TTTreeNode sinceChild)
        {
            TreeNodeRef wi = findNodeRef(sinceChild);
            if (wi == null)
                wi = this.first;
            else
                wi = wi.next;

            while(wi != null)
            {
                if (wi.node.canMatchAtom(name, atomType, atomValue))
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
        public TTTreeNode findNodeBackward(string name, string atomType, string atomValue, TTTreeNode sinceChild)
        {
            TreeNodeRef wi = findNodeRefBack(sinceChild);
            if (wi == null)
                wi = this.last;
            else
                wi = wi.prev;

            while (wi != null)
            {
                if (wi.node != null && wi.node.canMatchAtom(name, atomType, atomValue))
                    return wi.node;
                wi = wi.prev;
            }

            return null;
        }

        public bool checkIsSubnode(TTTreeNode dividerNode)
        {
            TreeNodeRef tr = findNodeRef(dividerNode);
            return tr != null;
        }

        public TTTreeNodeCollection takeChildrenBefore(TTTreeNode tn)
        {
            TTTreeNodeCollection tnc = new TTTreeNodeCollection();
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

        public TTTreeNodeCollection takeChildrenAfter(TTTreeNode tn)
        {
            TTTreeNodeCollection tnc = new TTTreeNodeCollection();
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

        public TTTreeNode createEncapsulationNode(string name)
        {
            TTTreeNode tn = new TTTreeNode(name);
            tn.Children = this;
            return tn;
        }


    }

    public class TTTreeNodeCollectionEnumerator: IEnumerator
    {
        public TTTreeNodeCollection.TreeNodeRef original = null;
        public TTTreeNodeCollection.TreeNodeRef collection = null;

        public TTTreeNodeCollectionEnumerator(TTTreeNodeCollection.TreeNodeRef col)
        {
            original = new TTTreeNodeCollection.TreeNodeRef();
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

        public TTTreeNode Current
        {
            get
            {
                return collection.node;
            }
        }
    }
}
