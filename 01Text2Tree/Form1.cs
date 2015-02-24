using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TextTreeParser;

namespace Text2Tree
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // input text
            string inputText = inputTextBox.Text;


            TTTest test = new TTTest();

            test.main(treeView1);
            richTextBox1.Text = test.inputFile;

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null && (e.Node.Tag is TTErrorLog.TreeItem))
            {
                TTErrorLog.TreeItem treeItem = (TTErrorLog.TreeItem)e.Node.Tag;

                richTextBox1.Select(treeItem.pos.position - 1, 1);
            }
        }
    }
}
