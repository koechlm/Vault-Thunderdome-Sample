namespace Thunderdome
{
    partial class FolderBrowseDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderBrowseDialog));
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._localDestFolderTextBox = new System.Windows.Forms.TextBox();
            this._folderBrowserButton = new System.Windows.Forms.Button();
            this._folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._setCustomDestCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            resources.ApplyResources(this._okButton, "_okButton");
            this._okButton.Name = "_okButton";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // _cancelButton
            // 
            resources.ApplyResources(this._cancelButton, "_cancelButton");
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // _localDestFolderTextBox
            // 
            resources.ApplyResources(this._localDestFolderTextBox, "_localDestFolderTextBox");
            this._localDestFolderTextBox.Name = "_localDestFolderTextBox";
            // 
            // _folderBrowserButton
            // 
            resources.ApplyResources(this._folderBrowserButton, "_folderBrowserButton");
            this._folderBrowserButton.Name = "_folderBrowserButton";
            this._folderBrowserButton.UseVisualStyleBackColor = true;
            this._folderBrowserButton.Click += new System.EventHandler(this._folderBrowserButton_Click);
            // 
            // _folderBrowserDialog
            // 
            resources.ApplyResources(this._folderBrowserDialog, "_folderBrowserDialog");
            // 
            // _setCustomDestCheckBox
            // 
            resources.ApplyResources(this._setCustomDestCheckBox, "_setCustomDestCheckBox");
            this._setCustomDestCheckBox.Name = "_setCustomDestCheckBox";
            this._setCustomDestCheckBox.UseVisualStyleBackColor = true;
            this._setCustomDestCheckBox.CheckedChanged += new System.EventHandler(this._setCustomDestCheckBox_CheckedChanged);
            // 
            // FolderBrowseDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._setCustomDestCheckBox);
            this.Controls.Add(this._folderBrowserButton);
            this.Controls.Add(this._localDestFolderTextBox);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.Name = "FolderBrowseDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.TextBox _localDestFolderTextBox;
        private System.Windows.Forms.Button _folderBrowserButton;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;
        private System.Windows.Forms.CheckBox _setCustomDestCheckBox;
    }
}