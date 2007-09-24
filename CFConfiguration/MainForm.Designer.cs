namespace CFConfiguration
{
	partial class MainForm
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
			this.lblDuration = new System.Windows.Forms.Label();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabRecording = new System.Windows.Forms.TabPage();
			this.cmdUploadRecordings = new System.Windows.Forms.Button();
			this.cmdRecordNow = new System.Windows.Forms.Button();
			this.cmdChooseRecordingPath = new System.Windows.Forms.Button();
			this.txtRecordingPath = new System.Windows.Forms.TextBox();
			this.lblRecordingPath = new System.Windows.Forms.Label();
			this.cmdSaveRecordings = new System.Windows.Forms.Button();
			this.txtDuration = new System.Windows.Forms.TextBox();
			this.txtFrequency = new System.Windows.Forms.TextBox();
			this.lblFrequency = new System.Windows.Forms.Label();
			this.tabServer = new System.Windows.Forms.TabPage();
			this.cmdResetServer = new System.Windows.Forms.Button();
			this.cmdTestServer = new System.Windows.Forms.Button();
			this.cmdSaveServer = new System.Windows.Forms.Button();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.lblServer = new System.Windows.Forms.Label();
			this.tabIdentity = new System.Windows.Forms.TabPage();
			this.cmdRefreshDeployment = new System.Windows.Forms.Button();
			this.txtDeploymentStarted = new System.Windows.Forms.TextBox();
			this.lblDeploymentStarted = new System.Windows.Forms.Label();
			this.cmdUpdateDeployment = new System.Windows.Forms.Button();
			this.txtDeploymentDescription = new System.Windows.Forms.TextBox();
			this.lblDeploymentDescription = new System.Windows.Forms.Label();
			this.txtDeploymentName = new System.Windows.Forms.TextBox();
			this.lblDeploymentName = new System.Windows.Forms.Label();
			this.txtDeploymentID = new System.Windows.Forms.TextBox();
			this.lblDeploymentID = new System.Windows.Forms.Label();
			this.lblNewDeploymentName = new System.Windows.Forms.Label();
			this.txtNewDeploymentName = new System.Windows.Forms.TextBox();
			this.txtHardwareID = new System.Windows.Forms.TextBox();
			this.lblHardwareID = new System.Windows.Forms.Label();
			this.cmdStartDeployment = new System.Windows.Forms.Button();
			this.tabInfo = new System.Windows.Forms.TabPage();
			this.lblVersion = new System.Windows.Forms.Label();
			this.chkDebugMode = new System.Windows.Forms.CheckBox();
			this.txtCurrentTime = new System.Windows.Forms.TextBox();
			this.lblCurrentTime = new System.Windows.Forms.Label();
			this.lblLog = new System.Windows.Forms.Label();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.cmdRefreshInfo = new System.Windows.Forms.Button();
			this.txtLastRecording = new System.Windows.Forms.TextBox();
			this.lblLastRecording = new System.Windows.Forms.Label();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.timer = new System.Windows.Forms.Timer();
			this.lblNextRecording = new System.Windows.Forms.Label();
			this.txtNextRecording = new System.Windows.Forms.TextBox();
			this.tabs.SuspendLayout();
			this.tabRecording.SuspendLayout();
			this.tabServer.SuspendLayout();
			this.tabIdentity.SuspendLayout();
			this.tabInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblDuration
			// 
			this.lblDuration.Location = new System.Drawing.Point(7, 37);
			this.lblDuration.Name = "lblDuration";
			this.lblDuration.Size = new System.Drawing.Size(56, 20);
			this.lblDuration.Text = "Duration:";
			// 
			// tabs
			// 
			this.tabs.Controls.Add(this.tabRecording);
			this.tabs.Controls.Add(this.tabServer);
			this.tabs.Controls.Add(this.tabIdentity);
			this.tabs.Controls.Add(this.tabInfo);
			this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabs.Location = new System.Drawing.Point(0, 0);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(240, 268);
			this.tabs.TabIndex = 1;
			// 
			// tabRecording
			// 
			this.tabRecording.Controls.Add(this.txtNextRecording);
			this.tabRecording.Controls.Add(this.lblNextRecording);
			this.tabRecording.Controls.Add(this.cmdUploadRecordings);
			this.tabRecording.Controls.Add(this.cmdRecordNow);
			this.tabRecording.Controls.Add(this.cmdChooseRecordingPath);
			this.tabRecording.Controls.Add(this.txtRecordingPath);
			this.tabRecording.Controls.Add(this.lblRecordingPath);
			this.tabRecording.Controls.Add(this.cmdSaveRecordings);
			this.tabRecording.Controls.Add(this.txtDuration);
			this.tabRecording.Controls.Add(this.txtFrequency);
			this.tabRecording.Controls.Add(this.lblFrequency);
			this.tabRecording.Controls.Add(this.lblDuration);
			this.tabRecording.Location = new System.Drawing.Point(0, 0);
			this.tabRecording.Name = "tabRecording";
			this.tabRecording.Size = new System.Drawing.Size(240, 245);
			this.tabRecording.Text = "Recordings";
			// 
			// cmdUploadRecordings
			// 
			this.cmdUploadRecordings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdUploadRecordings.Location = new System.Drawing.Point(7, 196);
			this.cmdUploadRecordings.Name = "cmdUploadRecordings";
			this.cmdUploadRecordings.Size = new System.Drawing.Size(123, 20);
			this.cmdUploadRecordings.TabIndex = 11;
			this.cmdUploadRecordings.Text = "Upload Recordings";
			this.cmdUploadRecordings.Click += new System.EventHandler(this.cmdUploadRecordings_Click);
			// 
			// cmdRecordNow
			// 
			this.cmdRecordNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdRecordNow.Location = new System.Drawing.Point(7, 222);
			this.cmdRecordNow.Name = "cmdRecordNow";
			this.cmdRecordNow.Size = new System.Drawing.Size(85, 20);
			this.cmdRecordNow.TabIndex = 10;
			this.cmdRecordNow.Text = "Record Now!";
			this.cmdRecordNow.Click += new System.EventHandler(this.cmdRecordNow_Click);
			// 
			// cmdChooseRecordingPath
			// 
			this.cmdChooseRecordingPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdChooseRecordingPath.Location = new System.Drawing.Point(210, 61);
			this.cmdChooseRecordingPath.Name = "cmdChooseRecordingPath";
			this.cmdChooseRecordingPath.Size = new System.Drawing.Size(23, 20);
			this.cmdChooseRecordingPath.TabIndex = 9;
			this.cmdChooseRecordingPath.Text = " ...";
			this.cmdChooseRecordingPath.Click += new System.EventHandler(this.cmdChooseRecordingPath_Click);
			// 
			// txtRecordingPath
			// 
			this.txtRecordingPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtRecordingPath.Location = new System.Drawing.Point(78, 61);
			this.txtRecordingPath.Name = "txtRecordingPath";
			this.txtRecordingPath.Size = new System.Drawing.Size(126, 21);
			this.txtRecordingPath.TabIndex = 7;
			// 
			// lblRecordingPath
			// 
			this.lblRecordingPath.Location = new System.Drawing.Point(7, 64);
			this.lblRecordingPath.Name = "lblRecordingPath";
			this.lblRecordingPath.Size = new System.Drawing.Size(56, 20);
			this.lblRecordingPath.Text = "Path:";
			// 
			// cmdSaveRecordings
			// 
			this.cmdSaveRecordings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdSaveRecordings.Location = new System.Drawing.Point(161, 222);
			this.cmdSaveRecordings.Name = "cmdSaveRecordings";
			this.cmdSaveRecordings.Size = new System.Drawing.Size(72, 20);
			this.cmdSaveRecordings.TabIndex = 5;
			this.cmdSaveRecordings.Text = "&Save";
			this.cmdSaveRecordings.Click += new System.EventHandler(this.cmdSaveRecordings_Click);
			// 
			// txtDuration
			// 
			this.txtDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtDuration.Location = new System.Drawing.Point(78, 34);
			this.txtDuration.Name = "txtDuration";
			this.txtDuration.Size = new System.Drawing.Size(155, 21);
			this.txtDuration.TabIndex = 4;
			// 
			// txtFrequency
			// 
			this.txtFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtFrequency.Location = new System.Drawing.Point(78, 7);
			this.txtFrequency.Name = "txtFrequency";
			this.txtFrequency.Size = new System.Drawing.Size(155, 21);
			this.txtFrequency.TabIndex = 3;
			// 
			// lblFrequency
			// 
			this.lblFrequency.Location = new System.Drawing.Point(7, 9);
			this.lblFrequency.Name = "lblFrequency";
			this.lblFrequency.Size = new System.Drawing.Size(65, 21);
			this.lblFrequency.Text = "Frequency:";
			// 
			// tabServer
			// 
			this.tabServer.Controls.Add(this.cmdResetServer);
			this.tabServer.Controls.Add(this.cmdTestServer);
			this.tabServer.Controls.Add(this.cmdSaveServer);
			this.tabServer.Controls.Add(this.txtServer);
			this.tabServer.Controls.Add(this.lblServer);
			this.tabServer.Location = new System.Drawing.Point(0, 0);
			this.tabServer.Name = "tabServer";
			this.tabServer.Size = new System.Drawing.Size(232, 242);
			this.tabServer.Text = "Server";
			// 
			// cmdResetServer
			// 
			this.cmdResetServer.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.cmdResetServer.Location = new System.Drawing.Point(80, 219);
			this.cmdResetServer.Name = "cmdResetServer";
			this.cmdResetServer.Size = new System.Drawing.Size(72, 20);
			this.cmdResetServer.TabIndex = 10;
			this.cmdResetServer.Text = "Reset";
			this.cmdResetServer.Click += new System.EventHandler(this.cmdResetServer_Click);
			// 
			// cmdTestServer
			// 
			this.cmdTestServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdTestServer.Location = new System.Drawing.Point(7, 219);
			this.cmdTestServer.Name = "cmdTestServer";
			this.cmdTestServer.Size = new System.Drawing.Size(72, 20);
			this.cmdTestServer.TabIndex = 8;
			this.cmdTestServer.Text = "&Test";
			this.cmdTestServer.Click += new System.EventHandler(this.cmdTestServer_Click);
			// 
			// cmdSaveServer
			// 
			this.cmdSaveServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdSaveServer.Location = new System.Drawing.Point(153, 219);
			this.cmdSaveServer.Name = "cmdSaveServer";
			this.cmdSaveServer.Size = new System.Drawing.Size(72, 20);
			this.cmdSaveServer.TabIndex = 7;
			this.cmdSaveServer.Text = "&Save";
			this.cmdSaveServer.Click += new System.EventHandler(this.cmdSaveServer_Click);
			// 
			// txtServer
			// 
			this.txtServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtServer.Location = new System.Drawing.Point(57, 7);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(168, 21);
			this.txtServer.TabIndex = 5;
			// 
			// lblServer
			// 
			this.lblServer.Location = new System.Drawing.Point(7, 9);
			this.lblServer.Name = "lblServer";
			this.lblServer.Size = new System.Drawing.Size(44, 21);
			this.lblServer.Text = "Server:";
			// 
			// tabIdentity
			// 
			this.tabIdentity.Controls.Add(this.cmdRefreshDeployment);
			this.tabIdentity.Controls.Add(this.txtDeploymentStarted);
			this.tabIdentity.Controls.Add(this.lblDeploymentStarted);
			this.tabIdentity.Controls.Add(this.cmdUpdateDeployment);
			this.tabIdentity.Controls.Add(this.txtDeploymentDescription);
			this.tabIdentity.Controls.Add(this.lblDeploymentDescription);
			this.tabIdentity.Controls.Add(this.txtDeploymentName);
			this.tabIdentity.Controls.Add(this.lblDeploymentName);
			this.tabIdentity.Controls.Add(this.txtDeploymentID);
			this.tabIdentity.Controls.Add(this.lblDeploymentID);
			this.tabIdentity.Controls.Add(this.lblNewDeploymentName);
			this.tabIdentity.Controls.Add(this.txtNewDeploymentName);
			this.tabIdentity.Controls.Add(this.txtHardwareID);
			this.tabIdentity.Controls.Add(this.lblHardwareID);
			this.tabIdentity.Controls.Add(this.cmdStartDeployment);
			this.tabIdentity.Location = new System.Drawing.Point(0, 0);
			this.tabIdentity.Name = "tabIdentity";
			this.tabIdentity.Size = new System.Drawing.Size(232, 242);
			this.tabIdentity.Text = "Identity";
			// 
			// cmdRefreshDeployment
			// 
			this.cmdRefreshDeployment.Location = new System.Drawing.Point(7, 168);
			this.cmdRefreshDeployment.Name = "cmdRefreshDeployment";
			this.cmdRefreshDeployment.Size = new System.Drawing.Size(133, 20);
			this.cmdRefreshDeployment.TabIndex = 18;
			this.cmdRefreshDeployment.Text = "Refresh from server";
			this.cmdRefreshDeployment.Click += new System.EventHandler(this.cmdRefreshDeployment_Click);
			// 
			// txtDeploymentStarted
			// 
			this.txtDeploymentStarted.Location = new System.Drawing.Point(106, 61);
			this.txtDeploymentStarted.Name = "txtDeploymentStarted";
			this.txtDeploymentStarted.ReadOnly = true;
			this.txtDeploymentStarted.Size = new System.Drawing.Size(127, 21);
			this.txtDeploymentStarted.TabIndex = 16;
			// 
			// lblDeploymentStarted
			// 
			this.lblDeploymentStarted.Location = new System.Drawing.Point(7, 62);
			this.lblDeploymentStarted.Name = "lblDeploymentStarted";
			this.lblDeploymentStarted.Size = new System.Drawing.Size(93, 20);
			this.lblDeploymentStarted.Text = "Date Started:";
			// 
			// cmdUpdateDeployment
			// 
			this.cmdUpdateDeployment.Location = new System.Drawing.Point(131, 142);
			this.cmdUpdateDeployment.Name = "cmdUpdateDeployment";
			this.cmdUpdateDeployment.Size = new System.Drawing.Size(102, 20);
			this.cmdUpdateDeployment.TabIndex = 14;
			this.cmdUpdateDeployment.Text = "Update Details";
			this.cmdUpdateDeployment.Click += new System.EventHandler(this.cmdUpdateDeployment_Click);
			// 
			// txtDeploymentDescription
			// 
			this.txtDeploymentDescription.Location = new System.Drawing.Point(106, 115);
			this.txtDeploymentDescription.Name = "txtDeploymentDescription";
			this.txtDeploymentDescription.Size = new System.Drawing.Size(127, 21);
			this.txtDeploymentDescription.TabIndex = 12;
			// 
			// lblDeploymentDescription
			// 
			this.lblDeploymentDescription.Location = new System.Drawing.Point(7, 116);
			this.lblDeploymentDescription.Name = "lblDeploymentDescription";
			this.lblDeploymentDescription.Size = new System.Drawing.Size(93, 20);
			this.lblDeploymentDescription.Text = "Description:";
			// 
			// txtDeploymentName
			// 
			this.txtDeploymentName.Location = new System.Drawing.Point(106, 88);
			this.txtDeploymentName.Name = "txtDeploymentName";
			this.txtDeploymentName.Size = new System.Drawing.Size(127, 21);
			this.txtDeploymentName.TabIndex = 9;
			// 
			// lblDeploymentName
			// 
			this.lblDeploymentName.Location = new System.Drawing.Point(7, 89);
			this.lblDeploymentName.Name = "lblDeploymentName";
			this.lblDeploymentName.Size = new System.Drawing.Size(93, 20);
			this.lblDeploymentName.Text = "Name:";
			// 
			// txtDeploymentID
			// 
			this.txtDeploymentID.Location = new System.Drawing.Point(106, 34);
			this.txtDeploymentID.Name = "txtDeploymentID";
			this.txtDeploymentID.ReadOnly = true;
			this.txtDeploymentID.Size = new System.Drawing.Size(127, 21);
			this.txtDeploymentID.TabIndex = 7;
			// 
			// lblDeploymentID
			// 
			this.lblDeploymentID.Location = new System.Drawing.Point(7, 35);
			this.lblDeploymentID.Name = "lblDeploymentID";
			this.lblDeploymentID.Size = new System.Drawing.Size(93, 20);
			this.lblDeploymentID.Text = "Deployment ID:";
			// 
			// lblNewDeploymentName
			// 
			this.lblNewDeploymentName.Location = new System.Drawing.Point(7, 222);
			this.lblNewDeploymentName.Name = "lblNewDeploymentName";
			this.lblNewDeploymentName.Size = new System.Drawing.Size(42, 20);
			this.lblNewDeploymentName.Text = "Name:";
			// 
			// txtNewDeploymentName
			// 
			this.txtNewDeploymentName.Location = new System.Drawing.Point(55, 221);
			this.txtNewDeploymentName.Name = "txtNewDeploymentName";
			this.txtNewDeploymentName.Size = new System.Drawing.Size(178, 21);
			this.txtNewDeploymentName.TabIndex = 3;
			// 
			// txtHardwareID
			// 
			this.txtHardwareID.Location = new System.Drawing.Point(92, 7);
			this.txtHardwareID.Name = "txtHardwareID";
			this.txtHardwareID.ReadOnly = true;
			this.txtHardwareID.Size = new System.Drawing.Size(141, 21);
			this.txtHardwareID.TabIndex = 2;
			// 
			// lblHardwareID
			// 
			this.lblHardwareID.Location = new System.Drawing.Point(7, 8);
			this.lblHardwareID.Name = "lblHardwareID";
			this.lblHardwareID.Size = new System.Drawing.Size(79, 20);
			this.lblHardwareID.Text = "Hardware ID:";
			// 
			// cmdStartDeployment
			// 
			this.cmdStartDeployment.Location = new System.Drawing.Point(7, 195);
			this.cmdStartDeployment.Name = "cmdStartDeployment";
			this.cmdStartDeployment.Size = new System.Drawing.Size(145, 20);
			this.cmdStartDeployment.TabIndex = 0;
			this.cmdStartDeployment.Text = "Start new deployment";
			this.cmdStartDeployment.Click += new System.EventHandler(this.cmdStartDeployment_Click);
			// 
			// tabInfo
			// 
			this.tabInfo.Controls.Add(this.lblVersion);
			this.tabInfo.Controls.Add(this.chkDebugMode);
			this.tabInfo.Controls.Add(this.txtCurrentTime);
			this.tabInfo.Controls.Add(this.lblCurrentTime);
			this.tabInfo.Controls.Add(this.lblLog);
			this.tabInfo.Controls.Add(this.txtLog);
			this.tabInfo.Controls.Add(this.cmdRefreshInfo);
			this.tabInfo.Controls.Add(this.txtLastRecording);
			this.tabInfo.Controls.Add(this.lblLastRecording);
			this.tabInfo.Location = new System.Drawing.Point(0, 0);
			this.tabInfo.Name = "tabInfo";
			this.tabInfo.Size = new System.Drawing.Size(232, 242);
			this.tabInfo.Text = "Info";
			// 
			// lblVersion
			// 
			this.lblVersion.Location = new System.Drawing.Point(133, 4);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(100, 20);
			this.lblVersion.Text = "Version";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// chkDebugMode
			// 
			this.chkDebugMode.Location = new System.Drawing.Point(7, 7);
			this.chkDebugMode.Name = "chkDebugMode";
			this.chkDebugMode.Size = new System.Drawing.Size(226, 20);
			this.chkDebugMode.TabIndex = 11;
			this.chkDebugMode.Text = "Debug Mode";
			this.chkDebugMode.CheckStateChanged += new System.EventHandler(this.chkDebugMode_CheckStateChanged);
			// 
			// txtCurrentTime
			// 
			this.txtCurrentTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtCurrentTime.Location = new System.Drawing.Point(104, 33);
			this.txtCurrentTime.Name = "txtCurrentTime";
			this.txtCurrentTime.ReadOnly = true;
			this.txtCurrentTime.Size = new System.Drawing.Size(121, 21);
			this.txtCurrentTime.TabIndex = 7;
			// 
			// lblCurrentTime
			// 
			this.lblCurrentTime.Location = new System.Drawing.Point(7, 36);
			this.lblCurrentTime.Name = "lblCurrentTime";
			this.lblCurrentTime.Size = new System.Drawing.Size(91, 20);
			this.lblCurrentTime.Text = "Current Time:";
			// 
			// lblLog
			// 
			this.lblLog.Location = new System.Drawing.Point(7, 83);
			this.lblLog.Name = "lblLog";
			this.lblLog.Size = new System.Drawing.Size(29, 20);
			this.lblLog.Text = "Log:";
			// 
			// txtLog
			// 
			this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLog.Location = new System.Drawing.Point(7, 106);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size(218, 105);
			this.txtLog.TabIndex = 3;
			// 
			// cmdRefreshInfo
			// 
			this.cmdRefreshInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdRefreshInfo.Location = new System.Drawing.Point(153, 217);
			this.cmdRefreshInfo.Name = "cmdRefreshInfo";
			this.cmdRefreshInfo.Size = new System.Drawing.Size(72, 20);
			this.cmdRefreshInfo.TabIndex = 2;
			this.cmdRefreshInfo.Text = "&Refresh";
			this.cmdRefreshInfo.Click += new System.EventHandler(this.cmdRefreshInfo_Click);
			// 
			// txtLastRecording
			// 
			this.txtLastRecording.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLastRecording.Location = new System.Drawing.Point(104, 60);
			this.txtLastRecording.Name = "txtLastRecording";
			this.txtLastRecording.ReadOnly = true;
			this.txtLastRecording.Size = new System.Drawing.Size(121, 21);
			this.txtLastRecording.TabIndex = 1;
			// 
			// lblLastRecording
			// 
			this.lblLastRecording.Location = new System.Drawing.Point(7, 63);
			this.lblLastRecording.Name = "lblLastRecording";
			this.lblLastRecording.Size = new System.Drawing.Size(91, 20);
			this.lblLastRecording.Text = "Last Recording:";
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 500;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// lblNextRecording
			// 
			this.lblNextRecording.Location = new System.Drawing.Point(7, 89);
			this.lblNextRecording.Name = "lblNextRecording";
			this.lblNextRecording.Size = new System.Drawing.Size(95, 20);
			this.lblNextRecording.Text = "Next Recording:";
			// 
			// txtNextRecording
			// 
			this.txtNextRecording.Location = new System.Drawing.Point(108, 88);
			this.txtNextRecording.Name = "txtNextRecording";
			this.txtNextRecording.Size = new System.Drawing.Size(125, 21);
			this.txtNextRecording.TabIndex = 16;
			this.txtNextRecording.TextChanged += new System.EventHandler(this.txtNextRecording_TextChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(240, 268);
			this.Controls.Add(this.tabs);
			this.Menu = this.mainMenu;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.Text = "QUT Sensors Configuration";
			this.tabs.ResumeLayout(false);
			this.tabRecording.ResumeLayout(false);
			this.tabServer.ResumeLayout(false);
			this.tabIdentity.ResumeLayout(false);
			this.tabInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblDuration;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabRecording;
		private System.Windows.Forms.TextBox txtFrequency;
		private System.Windows.Forms.Label lblFrequency;
		private System.Windows.Forms.TabPage tabIdentity;
		private System.Windows.Forms.TextBox txtDuration;
		private System.Windows.Forms.Button cmdSaveRecordings;
		private System.Windows.Forms.TabPage tabInfo;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.Button cmdRefreshInfo;
		private System.Windows.Forms.TextBox txtLastRecording;
		private System.Windows.Forms.Label lblLastRecording;
		private System.Windows.Forms.Label lblLog;
		private System.Windows.Forms.Button cmdChooseRecordingPath;
		private System.Windows.Forms.TextBox txtRecordingPath;
		private System.Windows.Forms.Label lblRecordingPath;
		private System.Windows.Forms.TabPage tabServer;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.Button cmdRecordNow;
		private System.Windows.Forms.TextBox txtCurrentTime;
		private System.Windows.Forms.Label lblCurrentTime;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.TextBox txtServer;
		private System.Windows.Forms.Label lblServer;
		private System.Windows.Forms.Button cmdSaveServer;
		private System.Windows.Forms.Button cmdTestServer;
		private System.Windows.Forms.Button cmdUploadRecordings;
		private System.Windows.Forms.CheckBox chkDebugMode;
		private System.Windows.Forms.Button cmdStartDeployment;
		private System.Windows.Forms.Button cmdResetServer;
		private System.Windows.Forms.TextBox txtHardwareID;
		private System.Windows.Forms.Label lblHardwareID;
		private System.Windows.Forms.TextBox txtDeploymentName;
		private System.Windows.Forms.Label lblDeploymentName;
		private System.Windows.Forms.TextBox txtDeploymentID;
		private System.Windows.Forms.Label lblDeploymentID;
		private System.Windows.Forms.Label lblNewDeploymentName;
		private System.Windows.Forms.TextBox txtNewDeploymentName;
		private System.Windows.Forms.TextBox txtDeploymentStarted;
		private System.Windows.Forms.Label lblDeploymentStarted;
		private System.Windows.Forms.Button cmdUpdateDeployment;
		private System.Windows.Forms.TextBox txtDeploymentDescription;
		private System.Windows.Forms.Label lblDeploymentDescription;
		private System.Windows.Forms.Button cmdRefreshDeployment;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.TextBox txtNextRecording;
		private System.Windows.Forms.Label lblNextRecording;
	}
}

