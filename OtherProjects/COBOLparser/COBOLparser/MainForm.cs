using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace COBOLparser
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            textBox1.Text = "c:\\Users\\peter.kollath\\Documents\\CKR\\COBOL Trans";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormSingleCobol frame = new FormSingleCobol();

            frame.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cobol.Node root = new Cobol.Node();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbe = new StringBuilder();
            StringBuilder sbf = new StringBuilder();
            HashSet<string> srcs = new HashSet<string>();
            HashSet<string> dest = new HashSet<string>();
            Dictionary<string, string> procToFile = new Dictionary<string, string>();

            int maxerrs = 1;
            int errs = 0;
            foreach (string file in Directory.EnumerateFiles(textBox1.Text))
            {
                if (maxerrs < errs) break;

                if (file.EndsWith(".txt"))
                {
                    Cobol.Parser parser = new Cobol.Parser();
                    parser.catchCommands.Add("ENTER");
                    parser.catchCommands.Add("CALL");

                    using (TextReader tr = new StreamReader(file))
                    {
                        try
                        {
                            Debugger.Log(0, "", "FILE: " + file + "\n");
                            parser.programName = string.Empty;
                            parser.filtered = new List<List<string>>();
                            parser.ParseInput(tr);

                            FileInfo fi = new FileInfo(file);
                            sbf.AppendFormat("{0};{1};{2}\n", file, parser.programName, fi.Length);
                            if (!procToFile.ContainsKey(parser.programName))
                                procToFile.Add(parser.programName, file);
                            foreach (List<string> los in parser.filtered)
                            {
                                if (los.Count > 0)
                                {
                                    switch (los[0])
                                    {
                                        case "CALL":
                                            if (los.Count > 1)
                                            {
                                                srcs.Add(parser.programName);
                                                string ssdd = los[1].Trim().Trim('\"');
                                                dest.Add(ssdd);
                                                sb.AppendFormat("OK;{0};{1};", file, parser.programName);
                                                sb.AppendFormat("CALL;{0}\n", los[1]);
                                            }
                                            break;
                                        case "ENTER":
                                            switch (los[1])
                                            {
                                                case "TAL":
                                                    sb.AppendFormat("OK;{0};{1};", file, parser.programName);
                                                    sb.AppendFormat("ENTER TAL;{0}\n", los[2]);
                                                    break;
                                                case "C":
                                                    sb.AppendFormat("OK;{0};{1};", file, parser.programName);
                                                    sb.AppendFormat("ENTER C;{0}\n", los[2]);
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                    }
                                }
                                //sb.AppendLine();
                            }
                            //sb.AppendLine();
                        }
                        catch (Exception ex)
                        {
                            sbe.AppendFormat("[FILE] {0}\n", file);
                            sbe.Append(ex.Message);
                            errs++;
                        }
                    }

                    parser.Root.Type = "FILE";
                    parser.Root.Value = file;
                    root.Children.Add(parser.Root);

                }
            }

            foreach (string s in srcs)
            {
                if (!dest.Contains(s))
                {
                    sb.AppendFormat("OK;{0};{1};", procToFile[s], "");
                    sb.AppendFormat("CALL;{0}\n", s);
                }
            }

            setTree(root);
            richTextBox1.Text = sb.ToString();

            richTextBox1.AppendText("\n\n");
            richTextBox1.AppendText(sbe.ToString());
            richTextBox1.AppendText("\n\n");
            richTextBox1.AppendText(sbf.ToString());


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

        private void button3_Click(object sender, EventArgs e)
        {
            FormDataTables fd = new FormDataTables();

            fd.Show();
        }


    }
}
