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
    public partial class SMVariableDialog : Form
    {
        public SMVariableDialog()
        {
            InitializeComponent();
        }

        public string smDataType
        {
            get { return comboBox1.Text; }
            set { comboBox1.Text = value; }
        }

        public string smVariableName
        {
            get { return textBox2.Text; }
            set { textBox2.Text = value; }
        }

        public string smInitialValue
        {
            get { return textBox3.Text; }
            set { textBox3.Text = value; }
        }

    }
}
