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
    public class ForEachFileNameEnumeratorUI : ForEachEnumeratorUI
    {
        public ForEachFileNameEnumeratorUI()
        {
            InitializeComponent();
        }

        private ForEachEnumeratorHost _host;
        private Variables _variables;
        private Connections _connections;
        private TextBox txtDirectory;
        private Label label4;
        private Label label5;
        private Button btnBrowse;
        private TextBox txtFileSpec;
        private FolderBrowserDialog folderBrowserDialog1;
        private GroupBox groupBox1;
        private RadioButton rbName;
        private RadioButton rbQual;
        private RadioButton rbExt;
        private CheckBox chkSubfolders;
        private Label lblError;

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

            if (_host != null)
            {
                if (PropertyHasValue("SourceDirectory"))
                    txtDirectory.Text = _host.Properties["SourceDirectory"].GetValue(_host).ToString();

                if (PropertyHasValue("FileSpec"))
                    txtFileSpec.Text = _host.Properties["FileSpec"].GetValue(_host).ToString();

                if (PropertyHasValue("TraverseSubfolders"))
                    chkSubfolders.Checked = (bool)_host.Properties["TraverseSubfolders"].GetValue(_host);

                //Check the current file path type
                if (PropertyHasValue("FilePath"))
                {
                    ForEachFileNameEnumerator.FilePathType type = (ForEachFileNameEnumerator.FilePathType)_host.Properties["FilePath"].GetValue(_host);
                    switch (type)
                    {
                        case ForEachFileNameEnumerator.FilePathType.NameAndExtension:
                            rbExt.Checked = true;
                            break;
                        case ForEachFileNameEnumerator.FilePathType.NameOnly:
                            rbName.Checked = true;
                            break;
                        case ForEachFileNameEnumerator.FilePathType.FullyQualified:
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
            _host.Properties["TraverseSubfolders"].SetValue(_host, chkSubfolders.Checked);

            //Check how to retrieve the file path from the enumerator
            if (rbQual.Checked)
                _host.Properties["FilePath"].SetValue(_host, ForEachFileNameEnumerator.FilePathType.FullyQualified);
            else
            {
                if (rbExt.Checked)
                    _host.Properties["FilePath"].SetValue(_host, ForEachFileNameEnumerator.FilePathType.NameAndExtension);
                else
                {
                    if (rbName.Checked)
                        _host.Properties["FilePath"].SetValue(_host, ForEachFileNameEnumerator.FilePathType.NameOnly);
                    else
                        _host.Properties["FilePath"].SetValue(_host, ForEachFileNameEnumerator.FilePathType.FullyQualified); //Default in case something goes wrong
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
            this.chkSubfolders = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFileSpec = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbName = new System.Windows.Forms.RadioButton();
            this.rbQual = new System.Windows.Forms.RadioButton();
            this.rbExt = new System.Windows.Forms.RadioButton();
            this.lblError = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkSubfolders
            // 
            this.chkSubfolders.AutoSize = true;
            this.chkSubfolders.Location = new System.Drawing.Point(7, 155);
            this.chkSubfolders.Name = "chkSubfolders";
            this.chkSubfolders.Size = new System.Drawing.Size(119, 17);
            this.chkSubfolders.TabIndex = 5;
            this.chkSubfolders.Text = "Traverse subfolders";
            this.chkSubfolders.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(335, 156);
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
            this.label5.Location = new System.Drawing.Point(4, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Files:";
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
            this.txtFileSpec.Location = new System.Drawing.Point(7, 63);
            this.txtFileSpec.Name = "txtFileSpec";
            this.txtFileSpec.Size = new System.Drawing.Size(416, 20);
            this.txtFileSpec.TabIndex = 19;
            this.txtFileSpec.Text = "*.*";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbName);
            this.groupBox1.Controls.Add(this.rbQual);
            this.groupBox1.Controls.Add(this.rbExt);
            this.groupBox1.Location = new System.Drawing.Point(7, 97);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(317, 43);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Retrieve file name";
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
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(10, 195);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(35, 13);
            this.lblError.TabIndex = 22;
            this.lblError.Text = "label6";
            this.lblError.Visible = false;
            // 
            // ForEachFileNameEnumeratorUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtFileSpec);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.chkSubfolders);
            this.Name = "ForEachFileNameEnumeratorUI";
            this.Size = new System.Drawing.Size(450, 250);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;

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
