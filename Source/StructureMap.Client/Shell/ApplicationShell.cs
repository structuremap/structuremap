using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using StructureMap.Client.Controllers;
using StructureMap.Client.TreeNodes;

namespace StructureMap.Client.Shell
{
    /// <summary>
    /// Summary description for ApplicationShell.
    /// </summary>
    public class ApplicationShell : Form, IApplicationShell
    {
        private ApplicationController _controller;
        private HtmlElement _mainDiv;
        private Button binaryFolderButton;
        private TextBox binaryFolderTextBox;
        private WebBrowser browser;
        private IContainer components;
        private Button configFileButton;
        private TextBox configPathTextBox;
        private FolderBrowserDialog folderBrowserDialog;
        private ImageList images;
        private Label label1;
        private Label label2;
        private CheckBox lockFolderCheckbox;
        private Panel mainPanel;
        private OpenFileDialog openFileDialog;
        private Button refreshButton;
        private Splitter splitter;
        private Panel topPanel;
        private TreeView treeView;

        public ApplicationShell()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            _controller = new ApplicationController(this, new ReportSource());
            treeView.MouseDown += new MouseEventHandler(treeView_MouseDown);


            browser.Navigate("about:blank");
            browser.Navigated += new WebBrowserNavigatedEventHandler(browser_Navigated);
            browser.NewWindow += new CancelEventHandler(browser_NewWindow);
        }

        #region IApplicationShell Members

        public GraphObjectNode TopNode
        {
            get { return treeView.TopNode as GraphObjectNode; }
            set
            {
                treeView.Nodes.Clear();
                treeView.Nodes.Add(value);

                value.Expand();
                foreach (TreeNode node in value.Nodes)
                {
                    node.Expand();
                }

                _controller.ShowView(value.ViewName, value.Subject);
            }
        }

        public string ConfigurationPath
        {
            get { return configPathTextBox.Text; }
        }

        public string AssemblyFolder
        {
            get { return binaryFolderTextBox.Text; }
        }

        public bool LockFolders
        {
            get { return lockFolderCheckbox.Checked; }
        }

        public void DisplayHTML(string html)
        {
            _mainDiv.InnerHtml = html;
        }

        #endregion

        private void browser_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;


            string search = browser.Url.ToString();

            SearchPart[] searchParts = SearchPart.ParseParts(search);
            GraphObjectNode targetNode = (GraphObjectNode) treeView.TopNode;
            foreach (SearchPart part in searchParts)
            {
                targetNode = part.FindNode(targetNode);
                if (targetNode == null)
                {
                    break;
                }
            }

            if (targetNode != null)
            {
                targetNode.EnsureVisible();
                treeView.SelectedNode = targetNode;
                _controller.ShowView(targetNode.ViewName, targetNode.Subject);
            }
        }

        private void browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType(), "Template.htm");
            StreamReader reader = new StreamReader(stream);

            string html = reader.ReadToEnd();

            browser.Document.Body.InnerHtml = html;
            _mainDiv = browser.Document.GetElementById("divContents");
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void configFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                configPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void binaryFolderButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                binaryFolderTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            _controller.RefreshReport();
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            Point point = new Point(e.X, e.Y);
            GraphObjectNode node = treeView.GetNodeAt(point) as GraphObjectNode;
            if (node != null)
            {
                _controller.ShowView(node.ViewName, node.Subject);
            }
        }


        private void lockFolderCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            binaryFolderButton.Enabled = binaryFolderTextBox.Enabled = !lockFolderCheckbox.Checked;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof (ApplicationShell));
            this.topPanel = new System.Windows.Forms.Panel();
            this.refreshButton = new System.Windows.Forms.Button();
            this.lockFolderCheckbox = new System.Windows.Forms.CheckBox();
            this.binaryFolderButton = new System.Windows.Forms.Button();
            this.binaryFolderTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.configFileButton = new System.Windows.Forms.Button();
            this.configPathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.browser = new WebBrowser();
            this.splitter = new System.Windows.Forms.Splitter();
            this.treeView = new System.Windows.Forms.TreeView();
            this.images = new System.Windows.Forms.ImageList(this.components);
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.topPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();

            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.refreshButton);
            this.topPanel.Controls.Add(this.lockFolderCheckbox);
            this.topPanel.Controls.Add(this.binaryFolderButton);
            this.topPanel.Controls.Add(this.binaryFolderTextBox);
            this.topPanel.Controls.Add(this.label2);
            this.topPanel.Controls.Add(this.configFileButton);
            this.topPanel.Controls.Add(this.configPathTextBox);
            this.topPanel.Controls.Add(this.label1);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1192, 129);
            this.topPanel.TabIndex = 0;
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(10, 83);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(90, 27);
            this.refreshButton.TabIndex = 7;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // lockFolderCheckbox
            // 
            this.lockFolderCheckbox.Checked = true;
            this.lockFolderCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lockFolderCheckbox.Location = new System.Drawing.Point(701, 18);
            this.lockFolderCheckbox.Name = "lockFolderCheckbox";
            this.lockFolderCheckbox.Size = new System.Drawing.Size(192, 84);
            this.lockFolderCheckbox.TabIndex = 6;
            this.lockFolderCheckbox.Text = "Look for assemblies in the same folder as the configuration file";
            this.lockFolderCheckbox.CheckedChanged += new System.EventHandler(this.lockFolderCheckbox_CheckedChanged);
            // 
            // binaryFolderButton
            // 
            this.binaryFolderButton.Enabled = false;
            this.binaryFolderButton.Location = new System.Drawing.Point(643, 46);
            this.binaryFolderButton.Name = "binaryFolderButton";
            this.binaryFolderButton.Size = new System.Drawing.Size(29, 27);
            this.binaryFolderButton.TabIndex = 5;
            this.binaryFolderButton.Text = "...";
            this.binaryFolderButton.Click += new System.EventHandler(this.binaryFolderButton_Click);
            // 
            // binaryFolderTextBox
            // 
            this.binaryFolderTextBox.Enabled = false;
            this.binaryFolderTextBox.Location = new System.Drawing.Point(134, 46);
            this.binaryFolderTextBox.Name = "binaryFolderTextBox";
            this.binaryFolderTextBox.Size = new System.Drawing.Size(509, 22);
            this.binaryFolderTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 28);
            this.label2.TabIndex = 3;
            this.label2.Text = "Binary Folder";
            // 
            // configFileButton
            // 
            this.configFileButton.Location = new System.Drawing.Point(643, 9);
            this.configFileButton.Name = "configFileButton";
            this.configFileButton.Size = new System.Drawing.Size(29, 27);
            this.configFileButton.TabIndex = 2;
            this.configFileButton.Text = "...";
            this.configFileButton.Click += new System.EventHandler(this.configFileButton_Click);
            // 
            // configPathTextBox
            // 
            this.configPathTextBox.Location = new System.Drawing.Point(134, 9);
            this.configPathTextBox.Name = "configPathTextBox";
            this.configPathTextBox.Size = new System.Drawing.Size(509, 22);
            this.configPathTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "Configuration File";
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.browser);
            this.mainPanel.Controls.Add(this.splitter);
            this.mainPanel.Controls.Add(this.treeView);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 129);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1192, 445);
            this.mainPanel.TabIndex = 1;
            // 
            // browser
            // 
            this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browser.Location = new System.Drawing.Point(359, 0);
            this.browser.Size = new System.Drawing.Size(833, 445);
            this.browser.TabIndex = 2;
            // 
            // splitter
            // 
            this.splitter.Location = new System.Drawing.Point(355, 0);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(4, 445);
            this.splitter.TabIndex = 1;
            this.splitter.TabStop = false;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.images;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(355, 445);
            this.treeView.TabIndex = 0;
            // 
            // images
            // 
            this.images.ImageStream =
                ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("images.ImageStream")));
            this.images.TransparentColor = System.Drawing.Color.Transparent;
            this.images.Images.SetKeyName(0, "");
            this.images.Images.SetKeyName(1, "");
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "config";
            this.openFileDialog.FileName = "StructureMap.config";
            // 
            // ApplicationShell
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(1192, 574);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.topPanel);
            this.Name = "ApplicationShell";
            this.Text = "StructureMap Configuration Explorer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}