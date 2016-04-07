using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace RTFChangeToUnicode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "RTF files (*.rtf)|*.rtf||";
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
                string text = File.ReadAllText(textBox1.Text);

                richTextBox.Text = text;
            }
        }


        public class FoundPos
        {
            public string text = string.Empty;
            public int pos;

            public override string ToString()
            {
                return text;
            }
        }

        public class OperState
        {
            public string srchText;
            public string replText;
            public string origText;
            public string result;

            private string _s = null;

            public override string ToString()
            {
                if (_s == null)
                 _s = srchText + " => " + replText;
                return _s;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBoxFind.Items.Clear();
            // find
            string txt = richTextBox.Text;
            string srchText = textBoxSearch.Text;

            int startIndex = 0;
            int found = 0;
            int ab, ac;
            bool ba, bb;

            found = txt.IndexOf(srchText, startIndex);
            while (found >= 0)
            {
                ba = true;
                bb = true;
                ab = found - 10;
                ac = found + srchText.Length + 10;
                if (ab < 0)
                {
                    ab = 0;
                    ba = false;
                }
                if (ac > txt.Length - 1)
                {
                    ac = txt.Length - 1;
                    bb = false;
                }

                FoundPos fp = new FoundPos();
                fp.pos = ab;
                fp.text = (ba ? "..." : "") + txt.Substring(ab, ac - ab) + (bb ? " ..." : "");
                listBoxFind.Items.Add(fp);

                startIndex = found + srchText.Length;
                found = txt.IndexOf(srchText, startIndex);
            }

            tabControl1.SelectedTab = tabPage3;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // replace
            OperState os = new OperState();
            os.origText = richTextBox.Text;
            os.replText = textBoxReplace.Text;
            os.srchText = textBoxSearch.Text;

            listBoxOpers.Items.Insert(0, os);

            string res = richTextBox.Text.Replace(os.srchText, os.replText);

            richTextBox.Text = res;
            os.result = res;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // save file
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // save opers
        }



    }
}
