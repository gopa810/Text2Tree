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

            inputTextBox.Text = "            VAR CHARSET newline; \r\n" +
            "VAR PATTERN pat2; \r\n" +
            "    \r\n" +
            "[pat2 SETMETHOD FIRST]; \r\n" +
            "[pat3 ADDSTRING 1 1 'func']; \r\n" +
            "[pat4 REMOVE [pat2 HEAD 2]]; \r\n" +
            "func main { \r\n" +
            "   [patMain exec]; \r\n" +
            "   [pat2 reset];\r\n" +
            "   a = (w + 3) - 2;" + 
            "} \r\n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // input text
            string inputText = inputTextBox.Text;


            TTTest test = new TTTest();

            try
            {
                test.main(inputTextBox.Text, treeView1, treeView2);
            }
            catch (Exception ex)
            {
                TTErrorLog.Shared.addLog("{0}", ex.Message);
            }

            TTErrorLog.Shared.resolveLastError();

            richTextBox2.Text = TTErrorLog.Shared.FinalMessage;
            richTextBox1.Text = test.inputFile;



        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null && (e.Node.Tag is TTErrorLog.TreeItem))
            {
                TTErrorLog.TreeItem treeItem = (TTErrorLog.TreeItem)e.Node.Tag;

                try
                {
                    richTextBox1.Select(treeItem.pos.position, 1);
                }
                catch
                {
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            treeView2.ExpandAll();
        }
    }
}
