using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CobolToCsharp
{
    public partial class Form1 : Form
    {
        public static string lastFileOpen;

        public MainText WorkText = new MainText();

        public Form1()
        {
            InitializeComponent();
            lastFileOpen = Properties.Settings.Default.LastInputFile;
            if (File.Exists(lastFileOpen))
            {
                InitListboxInput();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lastFileOpen = ofd.FileName;
                InitListboxInput();
            }
        }

        private void InitListboxInput()
        {
            WorkText.OpenFile(lastFileOpen);

            listBox1.Items.Clear();
            listBox1.Items.AddRange(WorkText.lines.ToArray<string>());

            listBox3.Items.Clear();
            foreach (MainText.LineRef lr in WorkText.takecareIndexes)
            {
                listBox3.Items.Add(lr);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LastInputFile = lastFileOpen;
            Properties.Settings.Default.Save();
        }

        private void buttonJoinLines_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection si = listBox1.SelectedIndices;
            int[] six = new int[si.Count];
            int i = 0;
            foreach (int index in si)
            {
                six[i] = index;
                i++;
            }

            int lastj = -1;
            for (int j = si.Count - 1; j >= 0; j--)
            {
                if (lastj == six[j] + 1)
                {
                    listBox1.Items.RemoveAt(lastj);
                    WorkText.lines[six[j]] = WorkText.lines[six[j]] + " " + WorkText.lines[lastj].Trim();
                    WorkText.lines.RemoveAt(lastj);
                    listBox1.Items.Insert(six[j], WorkText.lines[six[j]]);
                    listBox1.Items.RemoveAt(lastj);
                }

                lastj = six[j];
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (File.Exists(lastFileOpen))
            {
                File.WriteAllLines(lastFileOpen, WorkText.lines);
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.SelectedItems.Count > 0)
            {
                MainText.LineRef lr = listBox3.SelectedItems[0] as MainText.LineRef;
                listBox1.SelectedIndex = lr.Index;
                listBox1.TopIndex = lr.Index;
            }
        }

        private void buttonProcess_Click(object sender, EventArgs e)
        {
            WorkText.Convert();
            listBox2.Items.Clear();
            foreach (string s in WorkText.Converted)
            {
                listBox2.Items.Add(s);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in WorkText.Converted)
            {
                sb.AppendLine(s);
            }

            Clipboard.Clear();
            Clipboard.SetText(sb.ToString());
        }
    }
}
