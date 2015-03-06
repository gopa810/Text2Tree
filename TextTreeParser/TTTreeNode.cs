using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTreeParser
{
    public class TTTreeNode
    {
        // attributes of tree node
        public TTAtom atom = null;
        public string Name = string.Empty;

        // children nodes
        protected TTTreeNodeCollection child = null;

        // constructors
        public TTTreeNode()
        {
            atom = null;
        }

        public TTTreeNode(string name)
        {
            Name = name;
        }


        public TTTreeNodeCollection Children
        {
            get
            {
                if (child == null)
                    return new TTTreeNodeCollection();
                return child;
            }
            set
            {
                child = value;
            }
        }

        public void addSubnode(TTTreeNode node)
        {
            if (child == null)
                child = new TTTreeNodeCollection();
            child.addNode(node);
        }

        public void addCollection(TTTreeNodeCollection col)
        {
            if (child == null)
                child = new TTTreeNodeCollection(); 
            child.addCollection(col);
        }

        public bool canMatchAtom(string name, string atomType, string atomValue)
        {
            if (name != null && !this.Name.Equals(name))
                return false;
            if (this.atom != null)
            {
                if (atomType != null && !this.atom.Type.Equals(atomType))
                    return false;
                if (atomValue != null && !this.atom.Value.Equals(atomValue))
                    return false;
            }
            else
            {
                return false;
            }

            return true;
        }

        public TTTreeNodeCollection takeAllChildren()
        {
            TTTreeNodeCollection tnc = this.child;
            if (tnc != null)
            {
                this.child = new TTTreeNodeCollection();
            }
            return tnc;
        }

        public TTTreeNodeCollection takeChildrenBefore(TTTreeNode tn)
        {
            if (child == null)
                return null;

            if (this.child.checkIsSubnode(tn))
            {
                return this.child.takeChildrenBefore(tn);
            }

            return null;
        }

        public TTTreeNodeCollection takeChildrenAfter(TTTreeNode tn)
        {
            if (child == null)
                return null;

            if (this.child.checkIsSubnode(tn))
            {
                return this.child.takeChildrenAfter(tn);
            }

            return null;
        }

        public TTTreeNode findNodeForward(string name, string atomType, string atomValue, TTTreeNode sinceChild)
        {
            if (child == null) return null;
            return child.findNodeForward(name, atomType, atomValue, sinceChild);
        }

        public TTTreeNode findNodeBackward(string name, string atomType, string atomValue, TTTreeNode sinceChild)
        {
            if (child == null) return null;
            return child.findNodeBackward(name, atomType, atomValue, sinceChild);
        }

        public TTTreeNode removeHead()
        {
            if (child == null) return null;
            return child.removeFirstNode();
        }

        public TTTreeNode removeTail()
        {
            if (child != null)
                return child.removeLastNode();
            return null;
        }

        public TTTreeNode getTail()
        {
            if (child == null)
                return null;
            if (child.last == null)
                return null;
            return child.last.node;
        }

        public TTTreeNode getHead()
        {
            if (child == null)
                return null;
            if (child.first == null)
                return null;
            return child.first.node;
        }

    }
}
