using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TextTreeParser;
using TI = TrepInterpreter;

namespace Text2Tree
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            inputTextBox.Text = "           (set newline ( new CHARSET 'newline')) \r\n" +
            "(set pat2 (new PATTERN pat2)) \r\n" +
            "    \r\n" +
            "(pat2 SETMETHOD FIRST) \r\n" +
            "(pat3 ADDSTRING 1 1 'func') \r\n" +
            "(pat4 REMOVE (pat2 HEAD 2)) \r\n" +
            "(defun main () ( \r\n" +
            "   (if (a) () (run abc 1)) \r\n" + 
            "   (patMain exec) \r\n" +
            "   (pat2 reset)\r\n" +
            "   (set a (- (+ w 3) 2))" + 
            ") \r\n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // input text
            string inputText = inputTextBox.Text;


            TTTest test = new TTTest();
            treeView1.Nodes.Clear();
            treeView2.Nodes.Clear();

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
            treeView1.BeginUpdate();
            treeView1.ExpandAll();
            treeView1.EndUpdate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            treeView2.BeginUpdate();
            treeView2.ExpandAll();
            treeView2.EndUpdate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //TLanguageGScript script = new TLanguageGScript();
            TLanguageCPP script = new TLanguageCPP();

            script.Initialize();

            TI.Scripting scr = new TI.Scripting();
            TTDelegate td = new TTDelegate();

            try
            {
                scr.Execute(inputTextBox.Text, td);
                richTextBox3.Text = scr.logtext.ToString();
            }
            catch (Exception ex)
            {
                richTextBox3.Text = scr.logtext.ToString() + "\n\n" + ex.Message + "\n\n" + ex.StackTrace;
            }


        }
    }
}
