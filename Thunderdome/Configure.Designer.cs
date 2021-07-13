namespace Thunderdome
{
    partial class Configure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configure));
            this._allUsersButton = new System.Windows.Forms.Button();
            this._deployFolderTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this._warningsButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._deploymentNameTextBox = new System.Windows.Forms.TextBox();
            this._addFolderButton = new System.Windows.Forms.Button();
            this._createButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this._deploymentTypeComboBox = new System.Windows.Forms.ComboBox();
            this._forceDeploymentCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _allUsersButton
            // 
            resources.ApplyResources(this._allUsersButton, "_allUsersButton");
            this._allUsersButton.Image = global::Thunderdome.ExtensionRes._000440_folder_search;
            this._allUsersButton.Name = "_allUsersButton";
            this._allUsersButton.UseVisualStyleBackColor = true;
            this._allUsersButton.Click += new System.EventHandler(this.AllUsersButton_Click);
            // 
            // _deployFolderTextBox
            // 
            resources.ApplyResources(this._deployFolderTextBox, "_deployFolderTextBox");
            this._deployFolderTextBox.AutoCompleteCustomSource.AddRange(new string[] {
            resources.GetString("_deployFolderTextBox.AutoCompleteCustomSource")});
            this._deployFolderTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._deployFolderTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this._deployFolderTextBox.Name = "_deployFolderTextBox";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // _cancelButton
            // 
            resources.ApplyResources(this._cancelButton, "_cancelButton");
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // _okButton
            // 
            resources.ApplyResources(this._okButton, "_okButton");
            this._okButton.Name = "_okButton";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // label3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.label3, 2);
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // _warningsButton
            // 
            resources.ApplyResources(this._warningsButton, "_warningsButton");
            this._warningsButton.Name = "_warningsButton";
            this._warningsButton.UseVisualStyleBackColor = true;
            this._warningsButton.Click += new System.EventHandler(this.WarningsButton_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._deployFolderTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._allUsersButton, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._deploymentNameTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._addFolderButton, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this._createButton, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 3);
            this.tableLayoutPanel2.Controls.Add(this._warningsButton, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._cancelButton, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this._okButton, 2, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // _deploymentNameTextBox
            // 
            resources.ApplyResources(this._deploymentNameTextBox, "_deploymentNameTextBox");
            this._deploymentNameTextBox.Name = "_deploymentNameTextBox";
            // 
            // _addFolderButton
            // 
            this._addFolderButton.Image = global::Thunderdome.ExtensionRes._000410_folder_add;
            resources.ApplyResources(this._addFolderButton, "_addFolderButton");
            this._addFolderButton.Name = "_addFolderButton";
            this._addFolderButton.UseVisualStyleBackColor = true;
            this._addFolderButton.Click += new System.EventHandler(this.AddFolderButton_Click);
            // 
            // _createButton
            // 
            resources.ApplyResources(this._createButton, "_createButton");
            this._createButton.Image = global::Thunderdome.ExtensionRes._000210_computer_add;
            this._createButton.Name = "_createButton";
            this._createButton.UseVisualStyleBackColor = true;
            this._createButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._deploymentTypeComboBox);
            this.panel1.Controls.Add(this._forceDeploymentCheckBox);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // _deploymentTypeComboBox
            // 
            resources.ApplyResources(this._deploymentTypeComboBox, "_deploymentTypeComboBox");
            this._deploymentTypeComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._deploymentTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._deploymentTypeComboBox.FormattingEnabled = true;
            this._deploymentTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("_deploymentTypeComboBox.Items")});
            this._deploymentTypeComboBox.Name = "_deploymentTypeComboBox";
            this._deploymentTypeComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.DeploymentType_DrawItem);
            this._deploymentTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.DeploymentType_Changed);
            // 
            // _forceDeploymentCheckBox
            // 
            resources.ApplyResources(this._forceDeploymentCheckBox, "_forceDeploymentCheckBox");
            this._forceDeploymentCheckBox.Name = "_forceDeploymentCheckBox";
            this._forceDeploymentCheckBox.UseVisualStyleBackColor = true;
            // 
            // Configure
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Configure";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _allUsersButton;
        private System.Windows.Forms.TextBox _deployFolderTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button _warningsButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _deploymentNameTextBox;
        private System.Windows.Forms.Button _addFolderButton;
        private System.Windows.Forms.Button _createButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox _forceDeploymentCheckBox;
        private System.Windows.Forms.ComboBox _deploymentTypeComboBox;
    }
}