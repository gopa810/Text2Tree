﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTreeParser
{
    public class TTTreeNode
    {
        // attributes of tree node
        public string Type;
        public string Value;
        public Dictionary<String, String> Attributes;
        protected TTTreeNode firstSubnode = null;
        protected TTTreeNode lastSubnode = null;
        protected TTTreeNode nextNode = null;
        protected TTTreeNode parentNodeRef = null;
        public TextPosition startPos;
        public TextPosition endPos;

        // constructors
        public TTTreeNode()
        {
            Type = String.Empty;
            Value = String.Empty;
            Attributes = new Dictionary<string,string>();
        }

        public TTTreeNode firstChild
        {
            get
            {
                return firstSubnode;
            }
        }

        public TTTreeNode nextSibling
        {
            get
            {
                return nextNode;
            }
        }

        public TTTreeNode parentNode
        {
            get
            {
                return parentNodeRef;
            }
        }

        // methods
        public void SetAttribute(string attrName, string attrVal)
        {
            if (Attributes.ContainsKey(attrName))
                Attributes[attrName] = attrVal;
            Attributes.Add(attrName, attrVal);
        }

        public string GetAttribute(string attrName)
        {
            if (Attributes.ContainsKey(attrName))
                return Attributes[attrName];
            return "";
        }

        public int getSubnodesCount()
        {
            int count = 0;
            TTTreeNode item = firstSubnode;
            while (item != null)
            {
                count++;
                item = item.nextNode;
            }
            return count;
        }

        public TTTreeNode getSubnodeAtIndex(int index)
        {
            int count = 0;
            TTTreeNode item = firstSubnode;
            while (item != null)
            {
                if (count == index)
                    return item;
                count++;
                item = item.nextNode;
            }

            return null;
        }

        public void addSubnode(TTTreeNode node)
        {
            node.parentNodeRef = this;
            if (firstSubnode == null)
            {
                firstSubnode = node;
                lastSubnode = node.lastNode;
            }
            else
            {
                lastSubnode.nextNode = node;
                lastSubnode = node.lastNode;
            }

        }

        public TTTreeNode lastNode
        {
            get
            {
                TTTreeNode item = nextNode;
                while (item != null)
                {
                    if (item.nextNode == null)
                        return item;
                    item = item.nextNode;
                }

                return this;
            }
        }

        public void insertNodesAfter(TTTreeNode nodes)
        {
            TTTreeNode item = nodes;
            while (item != null)
            {
                item.parentNodeRef = this.parentNode;
                if (item.nextNode == null)
                {
                    item.nextNode = this.nextNode;
                    break;
                }
                item = item.nextNode;
            }
            this.nextNode = nodes;
        }

        public void removeSelf()
        {
            if (parentNode != null)
            {
                parentNode.removeNode(this);
            }
        }

        public void removeSubnodesWithType(string subType)
        {
            TTTreeNode removing;
            TTTreeNode tn = this.firstSubnode;
            while (tn != null)
            {
                if (tn.Type.Equals(subType))
                {
                    removing = tn;
                    tn = tn.nextSibling;
                    removing.removeSelf();
                }
                else
                {
                    tn.removeSubnodesWithType(subType);
                    tn = tn.nextSibling;
                }
            }
        }

        public void makeSubranges(string subTypeStart, string subValueStart, string subTypeEnd, string subValueEnd, string name)
        {
            TTTreeNode tn = this.firstSubnode;
            TTTreeNode next;
            List<TTTreeNode> stack = new List<TTTreeNode>();
            while (tn != null)
            {
                tn.makeSubranges(subTypeStart, subValueStart, subTypeEnd, subValueEnd, name);

                if ((subTypeStart == null || tn.Type.Equals(subTypeStart))
                    && (subValueStart == null || tn.Value.Equals(subValueStart)))
                {
                    stack.Add(tn);
                    tn = tn.nextNode;
                }
                else if ((subTypeEnd == null || tn.Type.Equals(subTypeEnd))
                    && (subValueEnd == null || tn.Value.Equals(subValueEnd)))
                {
                    if (stack.Count > 0)
                    {
                        next = tn.nextNode;
                        TTTreeNode t1 = stack[stack.Count - 1];
                        if (tn.parentNode != null)
                        {
                            TTTreeNode t2 = tn.parentNode.extractRange(t1, tn);
                            if (t2 != null)
                                t1.addSubnode(t2);
                            tn.removeSelf();
                            t1.Type = name;
                            while (t2 != null)
                            {
                                t2.parentNodeRef = t1;
                                t2 = t2.nextNode;
                            }
                        }
                        stack.RemoveAt(stack.Count - 1);
                        tn = next;
                    }
                }
                else
                {
                    tn = tn.nextNode;
                }

            }
        }


        public TTTreeNode extractRange(TTTreeNode t1, TTTreeNode t2)
        {
            if (t1.parentNode != t2.parentNode)
                return null;

            TTTreeNode tret = t1.nextNode;
            TTTreeNode t3 = tret;
            while (t3 != null && t3.nextNode != t2)
            {
                t3.parentNodeRef = null;
                t3 = t3.nextNode;
            }

            if (t3 != null)
            {
                t3.nextNode = null;
            }
            t1.nextNode = t2;


            return tret;
        }

        public void removeNode(TTTreeNode tn)
        {
            if (this.firstSubnode == tn)
            {
                if (this.lastSubnode == tn)
                {
                    this.firstSubnode = null;
                    this.lastSubnode = null;
                }
                else
                {
                    this.firstSubnode = tn.nextNode;
                }
            }
            else
            {
                TTTreeNode item = this.firstSubnode;
                while (item != null)
                {
                    if (item.nextNode == tn)
                    {
                        item.nextNode = tn.nextNode;
                        break;
                    }
                    item = item.nextNode;
                }
            }
            tn.nextNode = null;
            tn.parentNodeRef = null;
        }
    }
}
