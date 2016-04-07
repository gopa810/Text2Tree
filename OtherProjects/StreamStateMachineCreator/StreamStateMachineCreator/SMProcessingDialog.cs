using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StreamStateMachineCreator
{
    public partial class SMProcessingDialog : Form
    {
        public SMProcessingDialog(SMachine sm)
        {
            InitializeComponent();

            comboBox2.Items.Clear();
            comboBox2.Items.Add("");
            foreach (SMState st in sm.States)
            {
                comboBox2.Items.Add(st.Name);
            }
        }

        public string smStateName
        {
            get { return textBox2.Text; }
            set { textBox2.Text = value; }
        }

        public string smValues
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public string smActions
        {
            get { return richTextBox1.Text; }
            set { richTextBox1.Text = value; }
        }

        public string smComparer
        {
            get { return comboBox1.Text; }
            set { comboBox1.Text = value; }
        }

        public bool smReprocess
        {
            get { return checkBox1.Checked; }
            set { checkBox1.Checked = value; }
        }

        public string smNextState
        {
            get { return comboBox2.Text; }
            set { comboBox2.Text = value; }
        }
    }
}
