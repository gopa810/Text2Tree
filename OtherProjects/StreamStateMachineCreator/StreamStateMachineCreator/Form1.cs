using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using TrepInterpreter;
using System.Diagnostics;

namespace StreamStateMachineCreator
{
    public partial class Form1 : Form
    {
        string CurrentFileName = null;
        SMState CurrentState;
        SMachine CurrentMachine;
        private SMScriptDelegate Script = null;

        public Form1()
        {
            InitializeComponent();
            CurrentMachine = new SMachine();
        }

        /// <summary>
        /// Sets state machine properties to user interface
        /// </summary>
        /// <param name="sm">State Machine object</param>
        public void SetStateMachine(SMachine sm)
        {
            CurrentMachine = sm;

            this.textBox1.Text = sm.Name;

            this.comboBox1.Text = sm.Input;

            foreach (SMVariable sv in sm.Variables)
            {
                ListViewItem lvi = new ListViewItem(new string[] { sv.DataType, sv.Variable, sv.InitialValue });
                lvi.Tag = sv;
                listViewVariables.Items.Add(lvi);
            }

            foreach (SMState ss in sm.States)
            {
                ListViewItem lvi = new ListViewItem(ss.Name);
                lvi.Tag = ss;
                listViewStates.Items.Add(lvi);
            }

            if (listViewStates.Items.Count > 0)
                listViewStates.Items[0].Selected = true;
        }

        public bool IsSelectedChar
        {
            get
            {
                return (comboBox1.SelectedIndex < 2) ;
            }
        }

        public bool IsSelectedStringAsSource
        {
            get
            {
                return (comboBox1.SelectedIndex % 2 == 0);
            }
        }

        /// <summary>
        /// Gets State Machine for saving or export
        /// </summary>
        /// <returns>State Machine object</returns>
        public SMachine GetStateMachine()
        {
            if (CurrentMachine == null)
            {
                CurrentMachine = new SMachine();
                CurrentMachine.Name = textBox1.Text;
                CurrentMachine.Input = comboBox1.Text;
                foreach (ListViewItem lvi in listViewVariables.Items)
                {
                    SMVariable smv = lvi.Tag as SMVariable;
                    if (smv != null)
                        CurrentMachine.Variables.Add(smv);
                }
                foreach (ListViewItem lvi in listViewStates.Items)
                {
                    SMState sms = lvi.Tag as SMState;
                    if (sms != null)
                        CurrentMachine.States.Add(sms);
                }
            }
            else
            {
                CurrentMachine.Name = textBox1.Text;
                CurrentMachine.Input = comboBox1.Text;
            }

            return CurrentMachine;
        }

        #region SM State Processing

        private void buttonAddP_Click(object sender, EventArgs e)
        {
            if (CurrentState == null)
                return;

            SMProcessingDialog dlg = new SMProcessingDialog(CurrentMachine);

            dlg.smStateName = CurrentState.Name;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SMStateProcessing sp = new SMStateProcessing();
                sp.Actions = dlg.smActions;
                sp.Comparer = dlg.smComparer;
                sp.Values = dlg.smValues;
                sp.NextState = dlg.smNextState;
                sp.ReprocessInput = dlg.smReprocess;
                CurrentState.Process.Add(sp);

                ListViewItem lvi = new ListViewItem(new string[] { sp.Comparer, sp.Values, sp.Actions, sp.NextState, (sp.ReprocessInput ? "Y" : "") });
                lvi.Tag = sp;

                foreach (ListViewItem ll in listViewStateProcessing.Items)
                    ll.Selected = false;
                listViewStateProcessing.Items.Add(lvi);
                lvi.Selected = true;

                if (CurrentMachine.HasState(sp.NextState) == false)
                {
                    SMState ss = CurrentMachine.CreateState(sp.NextState);
                    ListViewItem lss = new ListViewItem(ss.Name);
                    lss.Tag = ss;
                    listViewStates.Items.Add(lss);
                }
            }
        }

        private void buttonInsertP_Click(object sender, EventArgs e)
        {
            if (CurrentState == null)
                return;

            SMProcessingDialog dlg = new SMProcessingDialog(CurrentMachine);

            dlg.smStateName = CurrentState.Name;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SMStateProcessing sp = new SMStateProcessing();
                sp.Actions = dlg.smActions;
                sp.Comparer = dlg.smComparer;
                sp.Values = dlg.smValues;
                CurrentState.Process.Add(sp);

                ListViewItem lvi = new ListViewItem(new string[] { sp.Comparer, sp.Values, sp.Actions, sp.NextState, (sp.ReprocessInput ? "Y" : "") });
                lvi.Tag = sp;

                if (listViewStateProcessing.SelectedIndices.Count > 0)
                {
                    foreach (ListViewItem ll in listViewStateProcessing.Items)
                        ll.Selected = false;
                    listViewStateProcessing.Items.Insert(listViewStateProcessing.SelectedIndices[0], lvi);
                    lvi.Selected = true;
                }
                else
                {
                    foreach (ListViewItem ll in listViewStateProcessing.Items)
                        ll.Selected = false;
                    lvi = listViewStateProcessing.Items.Add(lvi);
                    lvi.Selected = true;
                }

                if (CurrentMachine.HasState(sp.NextState) == false)
                {
                    SMState ss = CurrentMachine.CreateState(sp.NextState);
                    ListViewItem lss = new ListViewItem(ss.Name);
                    lss.Tag = ss;
                    listViewStates.Items.Add(lss);
                }
            }
        }

        private void buttonMoveUpP_Click(object sender, EventArgs e)
        {

        }

        private void buttonMoveDownP_Click(object sender, EventArgs e)
        {

        }

        private void buttonEditP_Click(object sender, EventArgs e)
        {
            if (listViewStateProcessing.SelectedItems.Count > 0)
            {
                editP_Item(listViewStateProcessing.SelectedItems[0]);
            }
        }

        private void buttonRemoveP_Click(object sender, EventArgs e)
        {
            if (CurrentState == null)
                return;

            if (listViewStateProcessing.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Do you want to delete selected items?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                {
                    List<ListViewItem> ls = new List<ListViewItem>();
                    foreach (ListViewItem lvi in listViewStateProcessing.SelectedItems)
                    {
                        ls.Add(lvi);
                    }
                    foreach (ListViewItem lvi in ls)
                    {
                        if (lvi.Tag is SMStateProcessing)
                            CurrentState.Process.Remove(lvi.Tag as SMStateProcessing);
                        listViewStateProcessing.Items.Remove(lvi);
                    }
                }
            }
        }

        private void listView3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listViewStateProcessing.GetItemAt(e.X, e.Y);
            if (lvi != null)
                editP_Item(lvi);
        }

        private void editP_Item(ListViewItem lvi)
        {
            SMProcessingDialog dlg = new SMProcessingDialog(CurrentMachine);
            SMStateProcessing sp = lvi.Tag as SMStateProcessing;

            if (sp == null)
                return;

            dlg.smActions = sp.Actions;
            dlg.smComparer = sp.Comparer;
            dlg.smStateName = CurrentState.Name;
            dlg.smValues = sp.Values;
            dlg.smNextState = sp.NextState;
            dlg.smReprocess = sp.ReprocessInput;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lvi.SubItems[0].Text = dlg.smComparer;
                lvi.SubItems[1].Text = dlg.smValues;
                lvi.SubItems[2].Text = dlg.smActions;
                lvi.SubItems[3].Text = dlg.smNextState;
                lvi.SubItems[4].Text = (dlg.smReprocess ? "Y" : "");

                sp.Actions = dlg.smActions;
                sp.Comparer = dlg.smComparer;
                sp.Values = dlg.smValues;
                sp.NextState = dlg.smNextState;
                sp.ReprocessInput = dlg.smReprocess;
            }
        }

        #endregion

        #region SM States

        private void RefreshInitialStateCombo()
        {
            comboBox2.Items.Clear();
            foreach (SMState ss in CurrentMachine.States)
            {
                comboBox2.Items.Add(ss.Name);
            }

            int ist = CurrentMachine.GetStateID(CurrentMachine.InitialState);
            if (ist < 0)
            {
                CurrentMachine.InitialState = string.Empty;
            }
            comboBox2.Text = CurrentMachine.InitialState;

        }

        private void buttonAddState_Click(object sender, EventArgs e)
        {
            SMStatePropertiesDialog dlg = new SMStatePropertiesDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SMState sm = new SMState();
                sm.Name = dlg.smName;
                CurrentMachine.States.Add(sm);

                ListViewItem lvi = new ListViewItem(sm.Name);
                lvi.Tag = sm;
                listViewStates.Items.Add(lvi);

                foreach (ListViewItem l in listViewStates.Items)
                {
                    l.Selected = false;
                }
                lvi.Selected = true;
                CurrentState = sm;
                listViewStateProcessing.Items.Clear();
                RefreshInitialStateCombo();
            }
        }

        private void buttonRemoveState_Click(object sender, EventArgs e)
        {
            if (listViewStates.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Do you want to delete selected items?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                {
                    List<ListViewItem> ls = new List<ListViewItem>();
                    foreach (ListViewItem lvi in listViewStates.SelectedItems)
                    {
                        ls.Add(lvi);
                    }
                    foreach (ListViewItem lvi in ls)
                    {
                        if (lvi.Tag is SMState)
                            CurrentMachine.States.Remove(lvi.Tag as SMState);
                        listViewStates.Items.Remove(lvi);
                    }
                }

                listViewStateProcessing.Items.Clear();
                RefreshInitialStateCombo();
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewStates.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listViewStates.SelectedItems[0];
                SMStatePropertiesDialog dlg = new SMStatePropertiesDialog();
                dlg.smName = lvi.SubItems[0].Text;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    lvi.SubItems[0].Text = dlg.smName;
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewStates.SelectedItems.Count > 0)
            {
                CurrentState = listViewStates.SelectedItems[0].Tag as SMState;
                fillProcessingsForState(listViewStates.SelectedItems[0]);
            }
        }

        private void fillProcessingsForState(ListViewItem lvi)
        {
            SMState ss = lvi.Tag as SMState;

            if (ss != null)
            {
                listViewStateProcessing.Items.Clear();
                foreach (SMStateProcessing sp in ss.Process)
                {
                    ListViewItem lv = new ListViewItem(new string[] { sp.Comparer, sp.Values, sp.Actions });
                    listViewStateProcessing.Items.Add(lv);
                }
            }
        }

        #endregion

        #region SM variables

        private void buttonAddVariable_Click(object sender, EventArgs e)
        {
            SMVariableDialog dlg = new SMVariableDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SMVariable sv = new SMVariable();
                sv.DataType = dlg.smDataType;
                sv.Variable = dlg.smVariableName;
                sv.InitialValue = dlg.smInitialValue;
                CurrentMachine.Variables.Add(sv);

                ListViewItem lvi = new ListViewItem(new string[] { dlg.smDataType, dlg.smVariableName, dlg.smInitialValue });
                lvi.Tag = sv;
                listViewVariables.Items.Add(lvi);
            }
        }

        private void buttonRemoveVariable_Click(object sender, EventArgs e)
        {
            if (listViewVariables.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Do you want to delete selected items?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                {
                    List<ListViewItem> ls = new List<ListViewItem>();
                    foreach (ListViewItem lvi in listViewVariables.SelectedItems)
                    {
                        ls.Add(lvi);
                    }
                    foreach (ListViewItem lvi in ls)
                    {
                        if (lvi.Tag is SMVariable && CurrentMachine != null)
                            CurrentMachine.Variables.Remove(lvi.Tag as SMVariable);
                        listViewVariables.Items.Remove(lvi);
                    }
                }

            }
        }

        private void listViewVariables_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewVariables.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listViewVariables.SelectedItems[0];
                SMVariableDialog dlg = new SMVariableDialog();
                dlg.smDataType = lvi.SubItems[0].Text;
                dlg.smVariableName = lvi.SubItems[1].Text;
                dlg.smInitialValue = lvi.SubItems[2].Text;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SMVariable sv = lvi.Tag as SMVariable;
                    if (sv != null)
                    {
                        sv.DataType = dlg.smDataType;
                        sv.Variable = dlg.smVariableName;
                        sv.InitialValue = dlg.smInitialValue;
                    }
                    lvi.SubItems[0].Text = dlg.smDataType;
                    lvi.SubItems[1].Text = dlg.smVariableName;
                    lvi.SubItems[2].Text = dlg.smInitialValue;
                }
            }

        }

        #endregion

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 main = new Form1();
            main.Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SMachine sm = new SMachine();
                sm.Open(ofd.FileName);
                CurrentFileName = ofd.FileName;
                SetStateMachine(sm);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(CurrentFileName);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(null);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool Save(string fileName)
        {
            if (fileName == null)
            {
                SaveFileDialog sfd = new SaveFileDialog();

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CurrentFileName = sfd.FileName;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                CurrentFileName = fileName;
            }

            SMachine sm = GetStateMachine();

            sm.Save(CurrentFileName);

            return true;
        }

        private void testWithStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonTestProcStr_Click(sender, e);
        }

        private void testWithFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonTestProcFile_Click(sender, e);
        }

        private void buttonTestFileBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = ofd.FileName;
            }
        }

        private void buttonTestProcStr_Click(object sender, EventArgs e)
        {
            using (StringReader sr = new StringReader(richTextBox1.Text))
            {
                parseTestStream(sr);
            }

        }

        private void buttonTestProcFile_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox2.Text))
            {
                MessageBox.Show("File does not exist");
                return;
            }

            using (StreamReader sr = new StreamReader(textBox2.Text))
            {
                parseTestStream(sr);
            }

        }

        /// <summary>
        /// Parses stream on input a uses state machine definition
        /// </summary>
        /// <param name="rd"></param>
        private void parseTestStream(TextReader rd)
        {
            CurrentMachine.PrepareForRun();
            Script = new SMScriptDelegate();

            foreach (SMVariable sv in CurrentMachine.Variables)
            {
                Script.DefineVariable(sv);
            }

            richTextBox2.Text = CurrentMachine.ErrorLog.ToString();
            Script.CurrentStateId = CurrentMachine.GetInitialStateID();
            SMStateProcessing sp = null;

            if (IsSelectedChar)
            {
                string line = rd.ReadToEnd();
                foreach (char rc in line)
                {
                    do
                    {
                        sp = parseTestChar(rc);
                        if (sp.CompiledNextState > 0)
                            Script.CurrentStateId = sp.CompiledNextState;
                    }
                    while (sp.ReprocessInput);
                }
            }
            else
            {
                string line = rd.ReadLine();
                while (line != null)
                {
                    do
                    {
                        sp = parseTestString(line);
                        if (sp.CompiledNextState > 0)
                            Script.CurrentStateId = sp.CompiledNextState;
                    }
                    while (sp.ReprocessInput);
                    line = rd.ReadLine();
                }
            }
        }

        private SMStateProcessing parseTestChar(char c)
        {
            SMState ss = CurrentMachine.GetState(Script.CurrentStateId);
            if (ss != null)
            {
                foreach (SMStateProcessing sp in ss.Process)
                {
                    if (sp.IsCharAcceptable(c))
                    {
                        // execute actions
                        Script.ExecuteNode(sp.CompiledActions);
                        return sp;
                    }
                }

                throw new Exception(string.Format("Don't know what to do with character \'{0}\' (\\d{2}) in state {1}", c, ss.Name, Convert.ToInt32(c)));
            }

            throw new Exception(string.Format("Cannot find state for id = {0}", Script.CurrentStateId));
        }

        private SMStateProcessing parseTestString(string s)
        {
            SMState ss = CurrentMachine.GetState(Script.CurrentStateId);
            if (ss != null)
            {
                foreach (SMStateProcessing sp in ss.Process)
                {
                    if (sp.IsStringAcceptable(s))
                    {
                        // execute actions
                        Script.ExecuteNode(sp.CompiledActions);
                        return sp;
                    }
                }

                throw new Exception(string.Format("Don't know what to do with string \'{0}\' in state {1}", s, ss.Name));
            }

            throw new Exception(string.Format("Cannot find state for id = {0}", Script.CurrentStateId));
        }

        private void richTextBox3_SelectionChanged(object sender, EventArgs e)
        {
            Debugger.Log(0, "", "Selection: " + richTextBox3.SelectionStart + "\n");
        }

    }
}
