namespace Thunderdome
{
    partial class FolderBrowseControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._folderTreeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // _folderTreeView
            // 
            this._folderTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._folderTreeView.Location = new System.Drawing.Point(0, 0);
            this._folderTreeView.Name = "_folderTreeView";
            this._folderTreeView.Size = new System.Drawing.Size(150, 150);
            this._folderTreeView.TabIndex = 0;
            this._folderTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.m_folderTreeView_BeforeExpand);
            // 
            // FolderBrowseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._folderTreeView);
            this.Name = "FolderBrowseControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView _folderTreeView;
    }
}
