using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NamePostfixB
{
    public partial class Form1 : Form
    {
        public class Pair
        {
            public string origPostfix;
            public string newPostfix;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] source = richTextBox1.Text.Split('\n', '\r');
            string[] rulesA = richTextBox2.Text.Split('\n', '\r');
            List<Pair> rules = new List<Pair>();

            StringBuilder res = new StringBuilder();
            StringBuilder not = new StringBuilder();

            foreach (string s in rulesA)
            {
                if (s.Length > 0)
                {
                    string[] pp = s.Split(' ');
                    if (pp.Length == 2)
                    {
                        Pair p = new Pair();
                        p.origPostfix = pp[0];
                        p.newPostfix = pp[1];
                        rules.Add(p);
                    }
                }
            }

            bool found = false;
            string nn;

            foreach (string a2 in source)
            {
                string a = a2.Trim();
                if (a.Length > 0)
                {
                    found = false;
                    foreach (Pair p in rules)
                    {
                        if (a.EndsWith(p.origPostfix))
                        {
                            nn = a.Substring(0, a.Length - p.origPostfix.Length) + p.newPostfix;
                            res.AppendLine(nn);
                            found = true;
                            break;
                        }
                    }
                    if (found == false)
                    {
                        not.AppendLine(a);
                    }
                }
            }


            richTextBox3.Text = res.ToString();
            richTextBox4.Text = not.ToString();
        }
    }
}
