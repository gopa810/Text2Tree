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
    public partial class FormSingleCobol : Form
    {
        public FormSingleCobol()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            Cobol.InputFile input = new Cobol.InputFile();

            input.setText(richTextBox1.Text);

            Cobol.Parser parser = new Cobol.Parser();

            try
            {
                parser.ParseInput(input.getReader());
            }
            catch(Exception ex)
            {
                sb.Append(ex.Message);
            }


            sb.AppendLine("-----------------------");
            richTextBox2.Text = sb.ToString();

        }
    }
}
