using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace COBOLparser
{
    public partial class FormDataTables : Form
    {
        class columnData
        {
            public string name;
            public string regex;
            public string defval;
            public string canull;
            public string notes;
        }

        public FormDataTables()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string s = richTextBox1.Text;
            string[] lines = s.Split('\r', '\n');
            int mode = 0;
            int pos = 0;

            sb.Append("<html>\n<body>\n");
            foreach (string line in lines)
            {
                string ts = line.Trim();

                if (ts.StartsWith("invoke "))
                {
                    ts = ts.Substring(7, ts.Length - 8).ToUpper();
                    sb.AppendFormat("<h3>Table {0}</h3>\n\n", ts);
                }
                else if (ts.Equals("("))
                {
                    mode = 1;
                    pos = 1;
                    sb.AppendFormat("<table><tr><td>Position</td><td>Column Name</td><td>Regular Expression</td><td>Can be NULL?</td><td>Default Value</td><td>Notes</td>\n\n");
                }
                else if (ts.Equals(")"))
                {
                    mode = 0;
                    sb.AppendFormat("</table>\n\n");
                }
                else if (mode == 1)
                {
                    string spp = "";
                    int ni = 2;
                    columnData cd = new columnData();
                    if (ts.StartsWith(","))
                        ts = ts.Substring(1).Trim();
                    string[] p = ts.Split(new char [] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    cd.name = p[0];
                    if (p[1] == "PIC")
                    {
                        ni++;
                        spp = p[1] + " " + p[2];
                    }
                    else if (p[1].StartsWith("VARCHAR"))
                    {
                        spp = p[1];
                    }
                    spp = spp.Replace("PIC X", "\\w");
                    spp = spp.Replace("PIC 9", "\\d");
                    spp = spp.Replace("(", "[").Replace(")", "]");
                    spp = spp.Replace("VARCHAR", "\\w");

                    cd.regex = spp;
                    cd.canull = "YES";
                    cd.defval = "N/A";

                    for (int i = ni; i < p.Length; i++)
                    {
                        if (p[i].Equals("UPSHIFT"))
                        {
                            cd.notes += " ";
                            cd.notes += p[i];
                        }
                        else if (p[i].Equals("COMP"))
                        {
                            cd.notes += " ";
                            cd.notes += p[i];
                        }
                        else if (p[i].Equals("NO") && p[i+1].Equals("DEFAULT"))
                        {
                            i++;
                        }
                        else if (p[i].Equals("NOT") && p[i+1].Equals("NULL"))
                        {
                            i++;
                            cd.canull = "NO";
                        }
                        else if (p[i].Equals("DEFAULT"))
                        {
                            cd.defval = p[i + 1];
                            i++;
                        }
                    }

                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", pos);
                    sb.AppendFormat("<td>{0}</td>", cd.name);
                    sb.AppendFormat("<td>{0}</td>", cd.regex);
                    sb.AppendFormat("<td>{0}</td>", cd.canull);
                    sb.AppendFormat("<td>{0}</td>", cd.defval);
                    sb.AppendFormat("<td>{0}</td>", cd.notes);
                    sb.AppendFormat("</tr>");
                    sb.AppendLine();

                    pos++;
                }
            }

            sb.Append("</body>\n</html>\n");

            File.WriteAllText("c:\\Users\\peter.kollath\\Documents\\CKR\\tbls.html", sb.ToString());
            
            richTextBox2.Text = sb.ToString();
        }
    }
}
