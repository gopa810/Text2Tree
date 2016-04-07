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
    public partial class SMStatePropertiesDialog : Form
    {
        public SMStatePropertiesDialog()
        {
            InitializeComponent();
        }

        public string smName
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
    }
}
