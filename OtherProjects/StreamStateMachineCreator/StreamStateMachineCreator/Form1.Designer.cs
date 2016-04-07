namespace StreamStateMachineCreator
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testWithStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testWithFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.listViewVariables = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonRemoveVariable = new System.Windows.Forms.Button();
            this.buttonAddVariable = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.buttonRemoveP = new System.Windows.Forms.Button();
            this.buttonEditP = new System.Windows.Forms.Button();
            this.buttonMoveDownP = new System.Windows.Forms.Button();
            this.buttonMoveUpP = new System.Windows.Forms.Button();
            this.buttonInsertP = new System.Windows.Forms.Button();
            this.buttonAddP = new System.Windows.Forms.Button();
            this.listViewStateProcessing = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonAddState = new System.Windows.Forms.Button();
            this.listViewStates = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.buttonTestProcFile = new System.Windows.Forms.Button();
            this.buttonTestProcStr = new System.Windows.Forms.Button();
            this.buttonTestFileBrowse = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(854, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(111, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testWithStringToolStripMenuItem,
            this.testWithFileToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // testWithStringToolStripMenuItem
            // 
            this.testWithStringToolStripMenuItem.Name = "testWithStringToolStripMenuItem";
            this.testWithStringToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.testWithStringToolStripMenuItem.Text = "Test with string";
            this.testWithStringToolStripMenuItem.Click += new System.EventHandler(this.testWithStringToolStripMenuItem_Click);
            // 
            // testWithFileToolStripMenuItem
            // 
            this.testWithFileToolStripMenuItem.Name = "testWithFileToolStripMenuItem";
            this.testWithFileToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.testWithFileToolStripMenuItem.Text = "Test with file";
            this.testWithFileToolStripMenuItem.Click += new System.EventHandler(this.testWithFileToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Location = new System.Drawing.Point(12, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(830, 496);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.comboBox2);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.comboBox1);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.listViewVariables);
            this.tabPage2.Controls.Add(this.buttonRemoveVariable);
            this.tabPage2.Controls.Add(this.buttonAddVariable);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(822, 470);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Properties";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(103, 80);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(279, 21);
            this.comboBox2.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Initial State:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "char from string",
            "char from file",
            "line from string",
            "line from file"});
            this.comboBox1.Location = new System.Drawing.Point(103, 49);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(279, 21);
            this.comboBox1.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Input Type:";
            // 
            // listViewVariables
            // 
            this.listViewVariables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listViewVariables.FullRowSelect = true;
            this.listViewVariables.GridLines = true;
            this.listViewVariables.HideSelection = false;
            this.listViewVariables.Location = new System.Drawing.Point(22, 126);
            this.listViewVariables.Name = "listViewVariables";
            this.listViewVariables.Size = new System.Drawing.Size(360, 225);
            this.listViewVariables.TabIndex = 6;
            this.listViewVariables.UseCompatibleStateImageBehavior = false;
            this.listViewVariables.View = System.Windows.Forms.View.Details;
            this.listViewVariables.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewVariables_MouseDoubleClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Data Type";
            this.columnHeader2.Width = 102;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Variable Name";
            this.columnHeader3.Width = 117;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Initial Value";
            this.columnHeader4.Width = 106;
            // 
            // buttonRemoveVariable
            // 
            this.buttonRemoveVariable.Location = new System.Drawing.Point(103, 357);
            this.buttonRemoveVariable.Name = "buttonRemoveVariable";
            this.buttonRemoveVariable.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveVariable.TabIndex = 5;
            this.buttonRemoveVariable.Text = "Remove";
            this.buttonRemoveVariable.UseVisualStyleBackColor = true;
            this.buttonRemoveVariable.Click += new System.EventHandler(this.buttonRemoveVariable_Click);
            // 
            // buttonAddVariable
            // 
            this.buttonAddVariable.Location = new System.Drawing.Point(22, 357);
            this.buttonAddVariable.Name = "buttonAddVariable";
            this.buttonAddVariable.Size = new System.Drawing.Size(75, 23);
            this.buttonAddVariable.TabIndex = 4;
            this.buttonAddVariable.Text = "Add";
            this.buttonAddVariable.UseVisualStyleBackColor = true;
            this.buttonAddVariable.Click += new System.EventHandler(this.buttonAddVariable_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Variables";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(60, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(201, 20);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tabControl2);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.buttonAddState);
            this.tabPage1.Controls.Add(this.listViewStates);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(822, 470);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Definition";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Location = new System.Drawing.Point(225, 6);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(591, 422);
            this.tabControl2.TabIndex = 3;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.buttonRemoveP);
            this.tabPage3.Controls.Add(this.buttonEditP);
            this.tabPage3.Controls.Add(this.buttonMoveDownP);
            this.tabPage3.Controls.Add(this.buttonMoveUpP);
            this.tabPage3.Controls.Add(this.buttonInsertP);
            this.tabPage3.Controls.Add(this.buttonAddP);
            this.tabPage3.Controls.Add(this.listViewStateProcessing);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(583, 396);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Processing";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // buttonRemoveP
            // 
            this.buttonRemoveP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemoveP.Location = new System.Drawing.Point(414, 367);
            this.buttonRemoveP.Name = "buttonRemoveP";
            this.buttonRemoveP.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveP.TabIndex = 6;
            this.buttonRemoveP.Text = "Remove";
            this.buttonRemoveP.UseVisualStyleBackColor = true;
            this.buttonRemoveP.Click += new System.EventHandler(this.buttonRemoveP_Click);
            // 
            // buttonEditP
            // 
            this.buttonEditP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEditP.Location = new System.Drawing.Point(330, 367);
            this.buttonEditP.Name = "buttonEditP";
            this.buttonEditP.Size = new System.Drawing.Size(75, 23);
            this.buttonEditP.TabIndex = 5;
            this.buttonEditP.Text = "Edit";
            this.buttonEditP.UseVisualStyleBackColor = true;
            this.buttonEditP.Click += new System.EventHandler(this.buttonEditP_Click);
            // 
            // buttonMoveDownP
            // 
            this.buttonMoveDownP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMoveDownP.Location = new System.Drawing.Point(249, 367);
            this.buttonMoveDownP.Name = "buttonMoveDownP";
            this.buttonMoveDownP.Size = new System.Drawing.Size(75, 23);
            this.buttonMoveDownP.TabIndex = 4;
            this.buttonMoveDownP.Text = "Move Down";
            this.buttonMoveDownP.UseVisualStyleBackColor = true;
            this.buttonMoveDownP.Click += new System.EventHandler(this.buttonMoveDownP_Click);
            // 
            // buttonMoveUpP
            // 
            this.buttonMoveUpP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMoveUpP.Location = new System.Drawing.Point(168, 367);
            this.buttonMoveUpP.Name = "buttonMoveUpP";
            this.buttonMoveUpP.Size = new System.Drawing.Size(75, 23);
            this.buttonMoveUpP.TabIndex = 3;
            this.buttonMoveUpP.Text = "Move Up";
            this.buttonMoveUpP.UseVisualStyleBackColor = true;
            this.buttonMoveUpP.Click += new System.EventHandler(this.buttonMoveUpP_Click);
            // 
            // buttonInsertP
            // 
            this.buttonInsertP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonInsertP.Location = new System.Drawing.Point(87, 367);
            this.buttonInsertP.Name = "buttonInsertP";
            this.buttonInsertP.Size = new System.Drawing.Size(75, 23);
            this.buttonInsertP.TabIndex = 2;
            this.buttonInsertP.Text = "Insert";
            this.buttonInsertP.UseVisualStyleBackColor = true;
            this.buttonInsertP.Click += new System.EventHandler(this.buttonInsertP_Click);
            // 
            // buttonAddP
            // 
            this.buttonAddP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddP.Location = new System.Drawing.Point(6, 367);
            this.buttonAddP.Name = "buttonAddP";
            this.buttonAddP.Size = new System.Drawing.Size(75, 23);
            this.buttonAddP.TabIndex = 1;
            this.buttonAddP.Text = "Add";
            this.buttonAddP.UseVisualStyleBackColor = true;
            this.buttonAddP.Click += new System.EventHandler(this.buttonAddP_Click);
            // 
            // listViewStateProcessing
            // 
            this.listViewStateProcessing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewStateProcessing.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.listViewStateProcessing.FullRowSelect = true;
            this.listViewStateProcessing.GridLines = true;
            this.listViewStateProcessing.HideSelection = false;
            this.listViewStateProcessing.Location = new System.Drawing.Point(6, 3);
            this.listViewStateProcessing.Name = "listViewStateProcessing";
            this.listViewStateProcessing.Size = new System.Drawing.Size(571, 358);
            this.listViewStateProcessing.TabIndex = 0;
            this.listViewStateProcessing.UseCompatibleStateImageBehavior = false;
            this.listViewStateProcessing.View = System.Windows.Forms.View.Details;
            this.listViewStateProcessing.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView3_MouseDoubleClick);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Comparer";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Values";
            this.columnHeader6.Width = 72;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Actions";
            this.columnHeader7.Width = 396;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "NextState";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Rep";
            this.columnHeader9.Width = 36;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(583, 396);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Next states";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(87, 434);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 30);
            this.button2.TabIndex = 2;
            this.button2.Text = "Remove";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.buttonRemoveState_Click);
            // 
            // buttonAddState
            // 
            this.buttonAddState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddState.Location = new System.Drawing.Point(6, 434);
            this.buttonAddState.Name = "buttonAddState";
            this.buttonAddState.Size = new System.Drawing.Size(75, 30);
            this.buttonAddState.TabIndex = 1;
            this.buttonAddState.Text = "Add";
            this.buttonAddState.UseVisualStyleBackColor = true;
            this.buttonAddState.Click += new System.EventHandler(this.buttonAddState_Click);
            // 
            // listViewStates
            // 
            this.listViewStates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewStates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewStates.FullRowSelect = true;
            this.listViewStates.GridLines = true;
            this.listViewStates.HideSelection = false;
            this.listViewStates.Location = new System.Drawing.Point(6, 6);
            this.listViewStates.Name = "listViewStates";
            this.listViewStates.Size = new System.Drawing.Size(213, 422);
            this.listViewStates.TabIndex = 0;
            this.listViewStates.UseCompatibleStateImageBehavior = false;
            this.listViewStates.View = System.Windows.Forms.View.Details;
            this.listViewStates.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listViewStates.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 178;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.richTextBox2);
            this.tabPage5.Controls.Add(this.buttonTestProcFile);
            this.tabPage5.Controls.Add(this.buttonTestProcStr);
            this.tabPage5.Controls.Add(this.buttonTestFileBrowse);
            this.tabPage5.Controls.Add(this.textBox2);
            this.tabPage5.Controls.Add(this.label5);
            this.tabPage5.Controls.Add(this.label4);
            this.tabPage5.Controls.Add(this.richTextBox1);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(822, 470);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Test";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // richTextBox2
            // 
            this.richTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox2.Location = new System.Drawing.Point(43, 198);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(485, 269);
            this.richTextBox2.TabIndex = 7;
            this.richTextBox2.Text = "";
            // 
            // buttonTestProcFile
            // 
            this.buttonTestProcFile.Location = new System.Drawing.Point(178, 158);
            this.buttonTestProcFile.Name = "buttonTestProcFile";
            this.buttonTestProcFile.Size = new System.Drawing.Size(115, 23);
            this.buttonTestProcFile.TabIndex = 6;
            this.buttonTestProcFile.Text = "Process File";
            this.buttonTestProcFile.UseVisualStyleBackColor = true;
            this.buttonTestProcFile.Click += new System.EventHandler(this.buttonTestProcFile_Click);
            // 
            // buttonTestProcStr
            // 
            this.buttonTestProcStr.Location = new System.Drawing.Point(43, 158);
            this.buttonTestProcStr.Name = "buttonTestProcStr";
            this.buttonTestProcStr.Size = new System.Drawing.Size(129, 23);
            this.buttonTestProcStr.TabIndex = 5;
            this.buttonTestProcStr.Text = "Process String";
            this.buttonTestProcStr.UseVisualStyleBackColor = true;
            this.buttonTestProcStr.Click += new System.EventHandler(this.buttonTestProcStr_Click);
            // 
            // buttonTestFileBrowse
            // 
            this.buttonTestFileBrowse.Location = new System.Drawing.Point(534, 10);
            this.buttonTestFileBrowse.Name = "buttonTestFileBrowse";
            this.buttonTestFileBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonTestFileBrowse.TabIndex = 4;
            this.buttonTestFileBrowse.Text = "Browse";
            this.buttonTestFileBrowse.UseVisualStyleBackColor = true;
            this.buttonTestFileBrowse.Click += new System.EventHandler(this.buttonTestFileBrowse_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(43, 10);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(485, 20);
            this.textBox2.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "File";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "String";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(43, 45);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(485, 96);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.richTextBox3);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(822, 470);
            this.tabPage6.TabIndex = 3;
            this.tabPage6.Text = "Script";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // richTextBox3
            // 
            this.richTextBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox3.Font = new System.Drawing.Font("Lucida Console", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox3.Location = new System.Drawing.Point(3, 3);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.Size = new System.Drawing.Size(816, 464);
            this.richTextBox3.TabIndex = 0;
            this.richTextBox3.Text = "";
            this.richTextBox3.SelectionChanged += new System.EventHandler(this.richTextBox3_SelectionChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 535);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listViewVariables;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button buttonRemoveVariable;
        private System.Windows.Forms.Button buttonAddVariable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonAddState;
        private System.Windows.Forms.ListView listViewStates;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonRemoveP;
        private System.Windows.Forms.Button buttonEditP;
        private System.Windows.Forms.Button buttonMoveDownP;
        private System.Windows.Forms.Button buttonMoveUpP;
        private System.Windows.Forms.Button buttonInsertP;
        private System.Windows.Forms.Button buttonAddP;
        private System.Windows.Forms.ListView listViewStateProcessing;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testWithStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testWithFileToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Button buttonTestProcFile;
        private System.Windows.Forms.Button buttonTestProcStr;
        private System.Windows.Forms.Button buttonTestFileBrowse;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.RichTextBox richTextBox3;
    }
}

