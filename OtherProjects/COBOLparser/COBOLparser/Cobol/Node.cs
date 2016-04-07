using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COBOLparser.Cobol
{
    /// <summary>
    /// This class inherits Type and Value from PathElement
    /// </summary>
    public class Node: PathElement
    {
        public List<Node> Children = new List<Node>();

        public Node FindOrCreateChild(PathElement pe)
        {
            foreach (Node n in Children)
            {
                if (pe.Type.Length == 0)
                {
                    if (pe.Value.Length == 0)
                        return n;
                    if (n.Value.Equals(pe.Value))
                        return n;
                }
                else if (pe.Value.Length == 0)
                {
                    if (n.Type.Equals(pe.Type))
                        return n;
                }
                else if (n.Type.Equals(pe.Type) && n.Value.Equals(pe.Value))
                {
                    return n;
                }
            }

            Node n1 = new Node();
            n1.Type = pe.Type;
            n1.Value = pe.Value;
            Children.Add(n1);

            return n1;
        }



        public string VisualText(bool expanded)
        {
            if (expanded)
            {
                if (Type.Length == 0)
                {
                    return Value;
                }
                else
                {
                    return string.Format("[{0}] {1}", Type, Value);
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(VisualText(true));
                sb.Append(" ==> ");
                foreach (Node n in Children)
                {
                    sb.AppendFormat("{0} ", n.VisualText(true));
                    if (sb.Length > 40)
                    {
                        sb.Append(" ...");
                        break;
                    }
                }
                return sb.ToString();
            }
        }
    }
}
