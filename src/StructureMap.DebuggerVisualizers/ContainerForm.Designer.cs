namespace StructureMap.DebuggerVisualizers
{
    partial class ContainerForm
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
            this.BrowserTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // BrowserTree
            // 
            this.BrowserTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowserTree.Location = new System.Drawing.Point(12, 12);
            this.BrowserTree.Name = "BrowserTree";
            this.BrowserTree.ShowRootLines = false;
            this.BrowserTree.Size = new System.Drawing.Size(548, 249);
            this.BrowserTree.TabIndex = 4;
            this.BrowserTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.BrowserTree_AfterExpand);
            // 
            // ContainerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 273);
            this.Controls.Add(this.BrowserTree);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(280, 80);
            this.Name = "ContainerForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "StructureMap Container Browser";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView BrowserTree;



    }
}