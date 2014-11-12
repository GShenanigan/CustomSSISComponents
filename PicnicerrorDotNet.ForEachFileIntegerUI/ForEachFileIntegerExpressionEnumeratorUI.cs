using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;

namespace PicnicerrorDotNet.SSIS.Enumerators
{
    public class ForEachFileIntegerExpressionEnumeratorUI : ForEachEnumeratorUI
    {
        public ForEachFileIntegerExpressionEnumeratorUI()
        {
            InitializeComponent();
        }

        private ForEachEnumeratorHost _host;
        private Variables _variables;
        private Connections _connections;
        private TextBox txtDirectory;
        private Label label4;
        private Label label5;
        private Label label1;
        private ComboBox cbOperator;
        private Label label2;
        private TextBox txtValue;
        private ComboBox cbExclusionVariable;
        private Label label3;
        private Button btnBrowse;
        private TextBox txtFileSpec;
        private FolderBrowserDialog folderBrowserDialog1;
        private Label lblError;
        private GroupBox groupBox1;
        private RadioButton rbName;
        private RadioButton rbQual;
        private RadioButton rbExt;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public override void Initialize(ForEachEnumeratorHost FEEHost, IServiceProvider serviceProvider, Connections connections, Variables variables)
        {
            base.Initialize(FEEHost, serviceProvider, connections, variables);
            _host = FEEHost;
            _variables = variables;
            _connections = connections;
            PopulateOperatorList();
            PopulateVariableList();

            if (_host != null)
            {
                if (PropertyHasValue("SourceDirectory"))
                    txtDirectory.Text = _host.Properties["SourceDirectory"].GetValue(_host).ToString();

                if (PropertyHasValue("FileSpec"))
                        txtFileSpec.Text = _host.Properties["FileSpec"].GetValue(_host).ToString();

                if (PropertyHasValue("UseComparison"))
                    chkComparison.Checked = (bool)_host.Properties["UseComparison"].GetValue(_host);

                if (PropertyHasValue("ComparisonValue"))
                    txtValue.Text = _host.Properties["ComparisonValue"].GetValue(_host).ToString();

                if (PropertyHasValue("ConditionalOperator"))
                {
                    ForEachFileIntegerExpressionEnumerator.ConditionalOperatorType type = (ForEachFileIntegerExpressionEnumerator.ConditionalOperatorType)_host.Properties["ConditionalOperator"].GetValue(_host);
                    cbOperator.SelectedIndex = cbOperator.Items.IndexOf(type);
                }

                if (PropertyHasValue("UseExclusionList"))
                    chkExclusion.Checked = (bool)_host.Properties["UseExclusionList"].GetValue(_host);

                if (PropertyHasValue("ExclusionListName"))
                {
                    //cbExclusionVariable.SelectedIndex = cbExclusionVariable.Items.IndexOf(_host.Properties["ExclusionList"].GetValue(_host));
                    if (_variables.Contains(_host.Properties["ExclusionListName"].GetValue(_host).ToString()))
                    {
                        Variable var = _variables[_host.Properties["ExclusionListName"].GetValue(_host).ToString()];
                        //cbExclusionVariable.SelectedIndex = cbExclusionVariable.Items.IndexOf(var);
                        cbExclusionVariable.SelectedIndex = cbExclusionVariable.FindString(var.Name);
                    }
                }

                //Check the current file path type
                if (PropertyHasValue("FilePath"))
                {
                    ForEachFileIntegerExpressionEnumerator.FilePathType type = (ForEachFileIntegerExpressionEnumerator.FilePathType)_host.Properties["FilePath"].GetValue(_host);
                    switch (type)
                    {
                        case ForEachFileIntegerExpressionEnumerator.FilePathType.NameAndExtension:
                            rbExt.Checked = true;
                            break;
                        case ForEachFileIntegerExpressionEnumerator.FilePathType.NameOnly:
                            rbName.Checked = true;
                            break;
                        case ForEachFileIntegerExpressionEnumerator.FilePathType.FullyQualified:
                            rbQual.Checked = true;
                            break;
                        default:
                            rbQual.Checked = true;
                            break;
                    }
                }
                else
                    rbQual.Checked = true;
            }
        }

        /// <summary>
        /// Inspects the propert to see if it, or its value is null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool PropertyHasValue(string name)
        {
            if (_host.Properties[name] != null)
            {
                if (_host.Properties[name].GetValue(_host) != null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Populates the list of comparison operators
        /// </summary>
        private void PopulateOperatorList()
        {
            //Add operators to the list box
            List<ForEachFileIntegerExpressionEnumerator.ConditionalOperatorType> operators = new List<ForEachFileIntegerExpressionEnumerator.ConditionalOperatorType>();
            foreach(ForEachFileIntegerExpressionEnumerator.ConditionalOperatorType type in Enum.GetValues(typeof(ForEachFileIntegerExpressionEnumerator.ConditionalOperatorType)))
                operators.Add(type);
            
            cbOperator.DataSource = operators;
            cbOperator.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates the list of valid variables to be an exclusion list (User, type Object only)
        /// </summary>
        private void PopulateVariableList()
        {
            //Add User variables only to the list box
            foreach (Variable v in _variables)
            {
                if (!v.SystemVariable && v.DataType == TypeCode.Object)
                    cbExclusionVariable.Items.Add(v.Name);
            }
            if (cbExclusionVariable.Items.Count > 0)
                cbExclusionVariable.SelectedIndex = 0;
        }

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

        /// <summary>
        /// Saves the properties of the loop
        /// </summary>
        public override void SaveSettings()
        {
            base.SaveSettings();

            //Validate the source directory input
            if (txtDirectory.Text == "")
            {
                if (_host.Properties["SourceDirectory"] == null)
                    throw new ArgumentException("Folder must be specified.");
            }
            else
                _host.Properties["SourceDirectory"].SetValue(_host, txtDirectory.Text);

            //Validate the file spec input
            if (txtFileSpec.Text == "")
            {
                if (_host.Properties["FileSpec"] == null)
                    _host.Properties["FileSpec"].SetValue(_host, "*.*");//Just assign default
            }
            else
                _host.Properties["FileSpec"].SetValue(_host, txtFileSpec.Text);

            //Assign the values of the booleans
            _host.Properties["UseComparison"].SetValue(_host, chkComparison.Checked);
            _host.Properties["UseExclusionList"].SetValue(_host, chkExclusion.Checked);

            if (chkComparison.Checked)
            {
                //Check if empty to allow for direct expression entry
                if (txtValue.Text != "")
                {
                    int value = 0;
                    bool success = Int32.TryParse(txtValue.Text, out value);
                    if (success)
                    {
                        _host.Properties["ComparisonValue"].SetValue(_host, value);
                    }
                    else
                        throw new ArgumentException("The comparison value provided is not a valid integer.");
                }
                else
                {
                    if (_host.Properties["ComparisonValue"] == null)
                        throw new ArgumentException("No comparison value provided.");
                }

                _host.Properties["ConditionalOperator"].SetValue(_host, (ForEachFileIntegerExpressionEnumerator.ConditionalOperatorType)Enum.Parse(typeof(ForEachFileIntegerExpressionEnumerator.ConditionalOperatorType), cbOperator.SelectedItem.ToString()));
            }
            else
            {
                _host.Properties["ComparisonValue"].SetValue(_host, null);
                _host.Properties["ConditionalOperator"].SetValue(_host, null);
            }

            if (chkExclusion.Checked)
            {
                _host.Properties["ExclusionListName"].SetValue(_host, cbExclusionVariable.SelectedItem.ToString());
                //_host.Properties["ExclusionList"].SetValue(_host, _variables[cbExclusionVariable.SelectedItem.ToString()]);
            }
            else
            {
                _host.Properties["ExclusionListName"].SetValue(_host, null);
                //_host.Properties["ExclusionList"].SetValue(_host, null);
            }

            //Check how to retrieve the file path from the enumerator
            if (rbQual.Checked)
                _host.Properties["FilePath"].SetValue(_host, ForEachFileIntegerExpressionEnumerator.FilePathType.FullyQualified);
            else
            {
                if (rbExt.Checked)
                    _host.Properties["FilePath"].SetValue(_host, ForEachFileIntegerExpressionEnumerator.FilePathType.NameAndExtension);
                else
                {
                    if (rbName.Checked)
                        _host.Properties["FilePath"].SetValue(_host, ForEachFileIntegerExpressionEnumerator.FilePathType.NameOnly);
                    else
                        _host.Properties["FilePath"].SetValue(_host, ForEachFileIntegerExpressionEnumerator.FilePathType.FullyQualified); //Default in case something goes wrong
                }
            }
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkComparison = new System.Windows.Forms.CheckBox();
            this.chkExclusion = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbOperator = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.cbExclusionVariable = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFileSpec = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.lblError = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbExt = new System.Windows.Forms.RadioButton();
            this.rbQual = new System.Windows.Forms.RadioButton();
            this.rbName = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkComparison
            // 
            this.chkComparison.AutoSize = true;
            this.chkComparison.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkComparison.Checked = true;
            this.chkComparison.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkComparison.Location = new System.Drawing.Point(3, 86);
            this.chkComparison.Name = "chkComparison";
            this.chkComparison.Size = new System.Drawing.Size(162, 17);
            this.chkComparison.TabIndex = 3;
            this.chkComparison.Text = "Compare filename as integer:";
            this.chkComparison.UseVisualStyleBackColor = true;
            this.chkComparison.CheckedChanged += new System.EventHandler(this.chkComparison_CheckedChanged);
            // 
            // chkExclusion
            // 
            this.chkExclusion.AutoSize = true;
            this.chkExclusion.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkExclusion.Location = new System.Drawing.Point(3, 137);
            this.chkExclusion.Name = "chkExclusion";
            this.chkExclusion.Size = new System.Drawing.Size(114, 17);
            this.chkExclusion.TabIndex = 5;
            this.chkExclusion.Text = "Exclude filenames:";
            this.chkExclusion.UseVisualStyleBackColor = true;
            this.chkExclusion.CheckedChanged += new System.EventHandler(this.chkExclusion_CheckedChanged);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(337, 190);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(74, 13);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "picnicerror.net";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // txtDirectory
            // 
            this.txtDirectory.Location = new System.Drawing.Point(7, 19);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(335, 20);
            this.txtDirectory.TabIndex = 9;
            this.txtDirectory.Text = "C:\\";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Folder:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Files:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(175, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Value:";
            // 
            // cbOperator
            // 
            this.cbOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOperator.FormattingEnabled = true;
            this.cbOperator.Location = new System.Drawing.Point(223, 84);
            this.cbOperator.Name = "cbOperator";
            this.cbOperator.Size = new System.Drawing.Size(200, 21);
            this.cbOperator.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(175, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Where:";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(223, 110);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(200, 20);
            this.txtValue.TabIndex = 12;
            this.txtValue.Text = "0";
            // 
            // cbExclusionVariable
            // 
            this.cbExclusionVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExclusionVariable.Enabled = false;
            this.cbExclusionVariable.FormattingEnabled = true;
            this.cbExclusionVariable.Location = new System.Drawing.Point(223, 135);
            this.cbExclusionVariable.Name = "cbExclusionVariable";
            this.cbExclusionVariable.Size = new System.Drawing.Size(200, 21);
            this.cbExclusionVariable.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(126, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Listed in variable:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(348, 18);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 18;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFileSpec
            // 
            this.txtFileSpec.Location = new System.Drawing.Point(7, 58);
            this.txtFileSpec.Name = "txtFileSpec";
            this.txtFileSpec.Size = new System.Drawing.Size(416, 20);
            this.txtFileSpec.TabIndex = 19;
            this.txtFileSpec.Text = "*.*";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(4, 113);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(35, 13);
            this.lblError.TabIndex = 20;
            this.lblError.Text = "label6";
            this.lblError.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbName);
            this.groupBox1.Controls.Add(this.rbQual);
            this.groupBox1.Controls.Add(this.rbExt);
            this.groupBox1.Location = new System.Drawing.Point(7, 170);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(317, 43);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Retrieve file name";
            // 
            // rbExt
            // 
            this.rbExt.AutoSize = true;
            this.rbExt.Location = new System.Drawing.Point(100, 18);
            this.rbExt.Name = "rbExt";
            this.rbExt.Size = new System.Drawing.Size(122, 17);
            this.rbExt.TabIndex = 0;
            this.rbExt.TabStop = true;
            this.rbExt.Text = "Name and extension";
            this.rbExt.UseVisualStyleBackColor = true;
            // 
            // rbQual
            // 
            this.rbQual.AutoSize = true;
            this.rbQual.Location = new System.Drawing.Point(6, 18);
            this.rbQual.Name = "rbQual";
            this.rbQual.Size = new System.Drawing.Size(88, 17);
            this.rbQual.TabIndex = 1;
            this.rbQual.TabStop = true;
            this.rbQual.Text = "Fully qualified";
            this.rbQual.UseVisualStyleBackColor = true;
            // 
            // rbName
            // 
            this.rbName.AutoSize = true;
            this.rbName.Location = new System.Drawing.Point(233, 18);
            this.rbName.Name = "rbName";
            this.rbName.Size = new System.Drawing.Size(75, 17);
            this.rbName.TabIndex = 2;
            this.rbName.TabStop = true;
            this.rbName.Text = "Name only";
            this.rbName.UseVisualStyleBackColor = true;
            // 
            // ForEachFileIntegerExpressionEnumeratorUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.txtFileSpec);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.cbExclusionVariable);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbOperator);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.chkExclusion);
            this.Controls.Add(this.chkComparison);
            this.Name = "ForEachFileIntegerExpressionEnumeratorUI";
            this.Size = new System.Drawing.Size(450, 250);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkComparison;
        private System.Windows.Forms.CheckBox chkExclusion;
        private System.Windows.Forms.LinkLabel linkLabel1;

        private void chkComparison_CheckedChanged(object sender, EventArgs e)
        {
            if (chkComparison.Checked)
            {
                txtValue.Enabled = true;
                cbOperator.Enabled = true;
            }
            else
            {
                txtValue.Enabled = false;
                cbOperator.Enabled = false;
            }
        }

        private void chkExclusion_CheckedChanged(object sender, EventArgs e)
        {
            if (chkExclusion.Checked)
                cbExclusionVariable.Enabled = true;
            else
                cbExclusionVariable.Enabled = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://picnicerror.net/");
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == this.folderBrowserDialog1.ShowDialog())
            {
                this.txtDirectory.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
