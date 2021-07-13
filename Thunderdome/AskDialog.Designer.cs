namespace Thunderdome
{
    partial class AskDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AskDialog));
            this._neverButton = new System.Windows.Forms.Button();
            this._noButton = new System.Windows.Forms.Button();
            this._yesButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this._updateListTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _neverButton
            // 
            resources.ApplyResources(this._neverButton, "_neverButton");
            this._neverButton.Name = "_neverButton";
            this._neverButton.UseVisualStyleBackColor = true;
            this._neverButton.Click += new System.EventHandler(this.NeverButton_Click);
            // 
            // _noButton
            // 
            resources.ApplyResources(this._noButton, "_noButton");
            this._noButton.Name = "_noButton";
            this._noButton.UseVisualStyleBackColor = true;
            this._noButton.Click += new System.EventHandler(this.NoButton_Click);
            // 
            // _yesButton
            // 
            resources.ApplyResources(this._yesButton, "_yesButton");
            this._yesButton.Name = "_yesButton";
            this._yesButton.UseVisualStyleBackColor = true;
            this._yesButton.Click += new System.EventHandler(this.YesButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _updateListTextBox
            // 
            resources.ApplyResources(this._updateListTextBox, "_updateListTextBox");
            this._updateListTextBox.BackColor = System.Drawing.SystemColors.Window;
            this._updateListTextBox.CausesValidation = false;
            this._updateListTextBox.Name = "_updateListTextBox";
            this._updateListTextBox.ReadOnly = true;
            // 
            // AskDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._updateListTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._yesButton);
            this.Controls.Add(this._noButton);
            this.Controls.Add(this._neverButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AskDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _neverButton;
        private System.Windows.Forms.Button _noButton;
        private System.Windows.Forms.Button _yesButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _updateListTextBox;
    }
}