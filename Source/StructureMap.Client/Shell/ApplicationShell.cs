using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using AxSHDocVw;
using mshtml;
using StructureMap.Client.Controllers;
using StructureMap.Client.TreeNodes;

namespace StructureMap.Client.Shell
{
	/// <summary>
	/// Summary description for ApplicationShell.
	/// </summary>
	public class ApplicationShell : Form, IApplicationShell
	{
		private Panel topPanel;
		private Label label1;
		private TextBox configPathTextBox;
		private Button configFileButton;
		private Button binaryFolderButton;
		private TextBox binaryFolderTextBox;
		private Label label2;
		private CheckBox lockFolderCheckbox;
		private Button refreshButton;
		private Panel mainPanel;
		private TreeView treeView;
		private AxWebBrowser browser;
		private Splitter splitter;
		private FolderBrowserDialog folderBrowserDialog;
		private OpenFileDialog openFileDialog;
		private IHTMLElement _mainDiv;

		private ApplicationController _controller;
		private ImageList images;
		private IContainer components;

		public ApplicationShell()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_controller = new ApplicationController(this, new ReportSource());
			treeView.MouseDown += new MouseEventHandler(treeView_MouseDown);

			object url = "about:blank";
			browser.Navigate2(ref url);
			browser.NavigateComplete2 += new DWebBrowserEvents2_NavigateComplete2EventHandler(browser_navigatecomplete2);
			browser.NewWindow3 += new DWebBrowserEvents2_NewWindow3EventHandler(browser_NewWindow3);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ApplicationShell));
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
			this.browser = new AxSHDocVw.AxWebBrowser();
			this.splitter = new System.Windows.Forms.Splitter();
			this.treeView = new System.Windows.Forms.TreeView();
			this.images = new System.Windows.Forms.ImageList(this.components);
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.topPanel.SuspendLayout();
			this.mainPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.browser)).BeginInit();
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
			this.topPanel.Size = new System.Drawing.Size(1192, 112);
			this.topPanel.TabIndex = 0;
			// 
			// refreshButton
			// 
			this.refreshButton.Location = new System.Drawing.Point(8, 72);
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.TabIndex = 7;
			this.refreshButton.Text = "Refresh";
			this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
			// 
			// lockFolderCheckbox
			// 
			this.lockFolderCheckbox.Checked = true;
			this.lockFolderCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.lockFolderCheckbox.Location = new System.Drawing.Point(584, 16);
			this.lockFolderCheckbox.Name = "lockFolderCheckbox";
			this.lockFolderCheckbox.Size = new System.Drawing.Size(160, 72);
			this.lockFolderCheckbox.TabIndex = 6;
			this.lockFolderCheckbox.Text = "Look for assemblies in the same folder as the configuration file";
			this.lockFolderCheckbox.CheckedChanged += new System.EventHandler(this.lockFolderCheckbox_CheckedChanged);
			// 
			// binaryFolderButton
			// 
			this.binaryFolderButton.Enabled = false;
			this.binaryFolderButton.Location = new System.Drawing.Point(536, 40);
			this.binaryFolderButton.Name = "binaryFolderButton";
			this.binaryFolderButton.Size = new System.Drawing.Size(24, 23);
			this.binaryFolderButton.TabIndex = 5;
			this.binaryFolderButton.Text = "...";
			this.binaryFolderButton.Click += new System.EventHandler(this.binaryFolderButton_Click);
			// 
			// binaryFolderTextBox
			// 
			this.binaryFolderTextBox.Enabled = false;
			this.binaryFolderTextBox.Location = new System.Drawing.Point(112, 40);
			this.binaryFolderTextBox.Name = "binaryFolderTextBox";
			this.binaryFolderTextBox.Size = new System.Drawing.Size(424, 20);
			this.binaryFolderTextBox.TabIndex = 4;
			this.binaryFolderTextBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 24);
			this.label2.TabIndex = 3;
			this.label2.Text = "Binary Folder";
			// 
			// configFileButton
			// 
			this.configFileButton.Location = new System.Drawing.Point(536, 8);
			this.configFileButton.Name = "configFileButton";
			this.configFileButton.Size = new System.Drawing.Size(24, 23);
			this.configFileButton.TabIndex = 2;
			this.configFileButton.Text = "...";
			this.configFileButton.Click += new System.EventHandler(this.configFileButton_Click);
			// 
			// configPathTextBox
			// 
			this.configPathTextBox.Location = new System.Drawing.Point(112, 8);
			this.configPathTextBox.Name = "configPathTextBox";
			this.configPathTextBox.Size = new System.Drawing.Size(424, 20);
			this.configPathTextBox.TabIndex = 1;
			this.configPathTextBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "Configuration File";
			// 
			// mainPanel
			// 
			this.mainPanel.Controls.Add(this.browser);
			this.mainPanel.Controls.Add(this.splitter);
			this.mainPanel.Controls.Add(this.treeView);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 112);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(1192, 462);
			this.mainPanel.TabIndex = 1;
			// 
			// browser
			// 
			this.browser.ContainingControl = this;
			this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.browser.Enabled = true;
			this.browser.Location = new System.Drawing.Point(299, 0);
			this.browser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("browser.OcxState")));
			this.browser.Size = new System.Drawing.Size(893, 462);
			this.browser.TabIndex = 2;
			// 
			// splitter
			// 
			this.splitter.Location = new System.Drawing.Point(296, 0);
			this.splitter.Name = "splitter";
			this.splitter.Size = new System.Drawing.Size(3, 462);
			this.splitter.TabIndex = 1;
			this.splitter.TabStop = false;
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeView.ImageList = this.images;
			this.treeView.Location = new System.Drawing.Point(0, 0);
			this.treeView.Name = "treeView";
			this.treeView.Size = new System.Drawing.Size(296, 462);
			this.treeView.TabIndex = 0;
			// 
			// images
			// 
			this.images.ImageSize = new System.Drawing.Size(16, 16);
			this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
			this.images.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "config";
			this.openFileDialog.FileName = "StructureMap.config";
			// 
			// ApplicationShell
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1192, 574);
			this.Controls.Add(this.mainPanel);
			this.Controls.Add(this.topPanel);
			this.Name = "ApplicationShell";
			this.Text = "StructureMap Configuration Explorer";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.topPanel.ResumeLayout(false);
			this.mainPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.browser)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

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
			_mainDiv.innerHTML = html;
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

		private void browser_navigatecomplete2(object sender, DWebBrowserEvents2_NavigateComplete2Event e)
		{
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType(), "Template.htm");
			StreamReader reader = new StreamReader(stream);

			string html = reader.ReadToEnd();

			IHTMLDocument2 document = (IHTMLDocument2) browser.Document;
			IHTMLElement body = (document).body;
			body.innerHTML = html;

			_mainDiv = ((HTMLDocument)document).getElementById("divContents");
		}

		private void browser_NewWindow3(object sender, DWebBrowserEvents2_NewWindow3Event e)
		{
			e.cancel = true;
			string search = e.bstrUrl;

			SearchPart[] searchParts = SearchPart.ParseParts(search);
			GraphObjectNode targetNode = (GraphObjectNode)treeView.TopNode;
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

		private void lockFolderCheckbox_CheckedChanged(object sender, System.EventArgs e)
		{
			binaryFolderButton.Enabled = binaryFolderTextBox.Enabled = !lockFolderCheckbox.Checked;
		}

		

	}
}
