namespace Thunderdome
{
    partial class _deployForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_deployForm));
            this.label1 = new System.Windows.Forms.Label();
            this._deploymentsComboBox = new System.Windows.Forms.ComboBox();
            this._deployButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _deploymentsComboBox
            // 
            resources.ApplyResources(this._deploymentsComboBox, "_deploymentsComboBox");
            this._deploymentsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._deploymentsComboBox.FormattingEnabled = true;
            this._deploymentsComboBox.Name = "_deploymentsComboBox";
            // 
            // _deployButton
            // 
            resources.ApplyResources(this._deployButton, "_deployButton");
            this._deployButton.Name = "_deployButton";
            this._deployButton.UseVisualStyleBackColor = true;
            this._deployButton.Click += new System.EventHandler(this._deployButton_Click);
            // 
            // _cancelButton
            // 
            resources.ApplyResources(this._cancelButton, "_cancelButton");
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _deployForm
            // 
            this.AcceptButton = this._deployButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._deployButton);
            this.Controls.Add(this._deploymentsComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "_deployForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _deploymentsComboBox;
        private System.Windows.Forms.Button _deployButton;
        private System.Windows.Forms.Button _cancelButton;
    }
}