using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace COBOLparser
{
    public partial class FormResults : Form
    {
        public FormResults()
        {
            InitializeComponent();
        }

        public void setText(string s)
        {
            richTextBox1.Text = s;
        }

        public void setText(StringBuilder sb)
        {
            richTextBox1.Text = sb.ToString();
        }

        public void setTree(Cobol.Node tree)
        {
            treeView1.Nodes.Clear();
            addTreeNodes(treeView1.Nodes, tree);
        }

        public void addTreeNodes(TreeNodeCollection tnc, Cobol.Node node)
        {
            TreeNode tn = tnc.Add(node.VisualText(false));
            tn.Tag = node;

            foreach (Cobol.Node nn in node.Children)
            {
                addTreeNodes(tn.Nodes, nn);
            }
        }

        private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Text = (e.Node.Tag as Cobol.Node).VisualText(false);
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Text = (e.Node.Tag as Cobol.Node).VisualText(true);
        }
    }
}
