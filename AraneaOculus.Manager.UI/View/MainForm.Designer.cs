namespace AraneaOculus.Manager.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            powerButton = new Button();
            portNumericUpDown = new NumericUpDown();
            statusLabel = new Label();
            powerPanel = new Panel();
            networkInterfacesComboBox = new ComboBox();
            qrPictureBox = new PictureBox();
            devicesListView = new ListView();
            tabControl1 = new TabControl();
            devicesTabPage = new TabPage();
            statisticsTabPage2 = new TabPage();
            tabControl2 = new TabControl();
            statisticsCommonTabPage = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            connectionsNumberGroupBox = new GroupBox();
            connectionsNumberChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            networkLoadGroupBox = new GroupBox();
            networkLoadChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            statisticsDetailedTabPage = new TabPage();
            detailedStatisticsDeviceInformationLabel = new Label();
            detailedNetworkLoadTimelineGroupBox = new GroupBox();
            detailedNetworkLoadTimelineChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            detailedConnectionsTimelineGroupBox = new GroupBox();
            detailedConnectionsTimelineChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            detailedStatisticsDeviceComboBox = new ComboBox();
            mainStatusStrip = new StatusStrip();
            mainToolStripStatusLabel = new ToolStripStatusLabel();
            mainToolStripProgressBar = new ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)portNumericUpDown).BeginInit();
            powerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)qrPictureBox).BeginInit();
            tabControl1.SuspendLayout();
            devicesTabPage.SuspendLayout();
            statisticsTabPage2.SuspendLayout();
            tabControl2.SuspendLayout();
            statisticsCommonTabPage.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            connectionsNumberGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)connectionsNumberChart).BeginInit();
            networkLoadGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)networkLoadChart).BeginInit();
            statisticsDetailedTabPage.SuspendLayout();
            detailedNetworkLoadTimelineGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)detailedNetworkLoadTimelineChart).BeginInit();
            detailedConnectionsTimelineGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)detailedConnectionsTimelineChart).BeginInit();
            mainStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // powerButton
            // 
            powerButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            powerButton.Location = new Point(647, 3);
            powerButton.Name = "powerButton";
            powerButton.Size = new Size(75, 23);
            powerButton.TabIndex = 0;
            powerButton.Text = "Start";
            powerButton.UseVisualStyleBackColor = true;
            powerButton.Click += OnPowerButtonClick;
            // 
            // portNumericUpDown
            // 
            portNumericUpDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            portNumericUpDown.Location = new Point(540, 3);
            portNumericUpDown.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            portNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            portNumericUpDown.Name = "portNumericUpDown";
            portNumericUpDown.Size = new Size(101, 23);
            portNumericUpDown.TabIndex = 1;
            portNumericUpDown.Value = new decimal(new int[] { 1984, 0, 0, 0 });
            // 
            // statusLabel
            // 
            statusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            statusLabel.Location = new Point(67, 29);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(652, 17);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Not running";
            statusLabel.TextAlign = ContentAlignment.TopRight;
            // 
            // powerPanel
            // 
            powerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            powerPanel.BackColor = Color.White;
            powerPanel.BorderStyle = BorderStyle.FixedSingle;
            powerPanel.Controls.Add(networkInterfacesComboBox);
            powerPanel.Controls.Add(qrPictureBox);
            powerPanel.Controls.Add(portNumericUpDown);
            powerPanel.Controls.Add(statusLabel);
            powerPanel.Controls.Add(powerButton);
            powerPanel.Location = new Point(12, 12);
            powerPanel.Name = "powerPanel";
            powerPanel.Size = new Size(727, 51);
            powerPanel.TabIndex = 5;
            // 
            // networkInterfacesComboBox
            // 
            networkInterfacesComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            networkInterfacesComboBox.FormattingEnabled = true;
            networkInterfacesComboBox.Location = new Point(56, 3);
            networkInterfacesComboBox.Name = "networkInterfacesComboBox";
            networkInterfacesComboBox.Size = new Size(478, 23);
            networkInterfacesComboBox.TabIndex = 3;
            // 
            // qrPictureBox
            // 
            qrPictureBox.BackColor = Color.White;
            qrPictureBox.BorderStyle = BorderStyle.FixedSingle;
            qrPictureBox.Location = new Point(-1, -1);
            qrPictureBox.Name = "qrPictureBox";
            qrPictureBox.Size = new Size(51, 51);
            qrPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            qrPictureBox.TabIndex = 4;
            qrPictureBox.TabStop = false;
            // 
            // devicesListView
            // 
            devicesListView.CheckBoxes = true;
            devicesListView.Dock = DockStyle.Fill;
            devicesListView.FullRowSelect = true;
            devicesListView.Location = new Point(3, 3);
            devicesListView.Name = "devicesListView";
            devicesListView.Size = new Size(713, 417);
            devicesListView.TabIndex = 6;
            devicesListView.UseCompatibleStateImageBehavior = false;
            devicesListView.View = System.Windows.Forms.View.Details;
            devicesListView.VirtualMode = true;
            devicesListView.ColumnReordered += OnDevicesListColumnReordered;
            devicesListView.RetrieveVirtualItem += OnDevicesListRetrieveVirtualItem;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(devicesTabPage);
            tabControl1.Controls.Add(statisticsTabPage2);
            tabControl1.Location = new Point(12, 69);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(727, 404);
            tabControl1.TabIndex = 7;
            // 
            // devicesTabPage
            // 
            devicesTabPage.Controls.Add(devicesListView);
            devicesTabPage.Location = new Point(4, 24);
            devicesTabPage.Name = "devicesTabPage";
            devicesTabPage.Padding = new Padding(3);
            devicesTabPage.Size = new Size(719, 423);
            devicesTabPage.TabIndex = 0;
            devicesTabPage.Text = "Devices";
            devicesTabPage.UseVisualStyleBackColor = true;
            // 
            // statisticsTabPage2
            // 
            statisticsTabPage2.Controls.Add(tabControl2);
            statisticsTabPage2.Location = new Point(4, 24);
            statisticsTabPage2.Name = "statisticsTabPage2";
            statisticsTabPage2.Padding = new Padding(3);
            statisticsTabPage2.Size = new Size(719, 376);
            statisticsTabPage2.TabIndex = 1;
            statisticsTabPage2.Text = "Statistics";
            statisticsTabPage2.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(statisticsCommonTabPage);
            tabControl2.Controls.Add(statisticsDetailedTabPage);
            tabControl2.Dock = DockStyle.Fill;
            tabControl2.Location = new Point(3, 3);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(713, 370);
            tabControl2.TabIndex = 0;
            // 
            // statisticsCommonTabPage
            // 
            statisticsCommonTabPage.Controls.Add(tableLayoutPanel1);
            statisticsCommonTabPage.Location = new Point(4, 24);
            statisticsCommonTabPage.Name = "statisticsCommonTabPage";
            statisticsCommonTabPage.Padding = new Padding(3);
            statisticsCommonTabPage.Size = new Size(705, 342);
            statisticsCommonTabPage.TabIndex = 0;
            statisticsCommonTabPage.Text = "General";
            statisticsCommonTabPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(connectionsNumberGroupBox, 0, 1);
            tableLayoutPanel1.Controls.Add(networkLoadGroupBox, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 51.1400642F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 48.8599358F));
            tableLayoutPanel1.Size = new Size(699, 336);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // connectionsNumberGroupBox
            // 
            connectionsNumberGroupBox.Controls.Add(connectionsNumberChart);
            connectionsNumberGroupBox.Dock = DockStyle.Fill;
            connectionsNumberGroupBox.Location = new Point(3, 174);
            connectionsNumberGroupBox.Name = "connectionsNumberGroupBox";
            connectionsNumberGroupBox.Size = new Size(693, 159);
            connectionsNumberGroupBox.TabIndex = 6;
            connectionsNumberGroupBox.TabStop = false;
            connectionsNumberGroupBox.Text = "Connections Number";
            // 
            // connectionsNumberChart
            // 
            connectionsNumberChart.Dock = DockStyle.Fill;
            connectionsNumberChart.Location = new Point(3, 19);
            connectionsNumberChart.Name = "connectionsNumberChart";
            connectionsNumberChart.Size = new Size(687, 137);
            connectionsNumberChart.TabIndex = 0;
            // 
            // networkLoadGroupBox
            // 
            networkLoadGroupBox.Controls.Add(networkLoadChart);
            networkLoadGroupBox.Dock = DockStyle.Fill;
            networkLoadGroupBox.Location = new Point(3, 3);
            networkLoadGroupBox.Name = "networkLoadGroupBox";
            networkLoadGroupBox.Size = new Size(693, 165);
            networkLoadGroupBox.TabIndex = 5;
            networkLoadGroupBox.TabStop = false;
            networkLoadGroupBox.Text = "Network Load";
            // 
            // networkLoadChart
            // 
            networkLoadChart.Dock = DockStyle.Fill;
            networkLoadChart.Location = new Point(3, 19);
            networkLoadChart.Name = "networkLoadChart";
            networkLoadChart.Size = new Size(687, 143);
            networkLoadChart.TabIndex = 0;
            // 
            // statisticsDetailedTabPage
            // 
            statisticsDetailedTabPage.Controls.Add(detailedStatisticsDeviceInformationLabel);
            statisticsDetailedTabPage.Controls.Add(detailedNetworkLoadTimelineGroupBox);
            statisticsDetailedTabPage.Controls.Add(detailedConnectionsTimelineGroupBox);
            statisticsDetailedTabPage.Controls.Add(detailedStatisticsDeviceComboBox);
            statisticsDetailedTabPage.Location = new Point(4, 24);
            statisticsDetailedTabPage.Name = "statisticsDetailedTabPage";
            statisticsDetailedTabPage.Padding = new Padding(3);
            statisticsDetailedTabPage.Size = new Size(705, 342);
            statisticsDetailedTabPage.TabIndex = 1;
            statisticsDetailedTabPage.Text = "Detailed";
            statisticsDetailedTabPage.UseVisualStyleBackColor = true;
            // 
            // detailedStatisticsDeviceInformationLabel
            // 
            detailedStatisticsDeviceInformationLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            detailedStatisticsDeviceInformationLabel.Location = new Point(6, 32);
            detailedStatisticsDeviceInformationLabel.Name = "detailedStatisticsDeviceInformationLabel";
            detailedStatisticsDeviceInformationLabel.Size = new Size(696, 33);
            detailedStatisticsDeviceInformationLabel.TabIndex = 9;
            detailedStatisticsDeviceInformationLabel.Text = "Select a device to view information";
            // 
            // detailedNetworkLoadTimelineGroupBox
            // 
            detailedNetworkLoadTimelineGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            detailedNetworkLoadTimelineGroupBox.Controls.Add(detailedNetworkLoadTimelineChart);
            detailedNetworkLoadTimelineGroupBox.Location = new Point(6, 225);
            detailedNetworkLoadTimelineGroupBox.Name = "detailedNetworkLoadTimelineGroupBox";
            detailedNetworkLoadTimelineGroupBox.Size = new Size(693, 111);
            detailedNetworkLoadTimelineGroupBox.TabIndex = 8;
            detailedNetworkLoadTimelineGroupBox.TabStop = false;
            detailedNetworkLoadTimelineGroupBox.Text = "Network Load";
            // 
            // detailedNetworkLoadTimelineChart
            // 
            detailedNetworkLoadTimelineChart.Dock = DockStyle.Fill;
            detailedNetworkLoadTimelineChart.Location = new Point(3, 19);
            detailedNetworkLoadTimelineChart.Name = "detailedNetworkLoadTimelineChart";
            detailedNetworkLoadTimelineChart.Size = new Size(687, 89);
            detailedNetworkLoadTimelineChart.TabIndex = 0;
            // 
            // detailedConnectionsTimelineGroupBox
            // 
            detailedConnectionsTimelineGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            detailedConnectionsTimelineGroupBox.Controls.Add(detailedConnectionsTimelineChart);
            detailedConnectionsTimelineGroupBox.Location = new Point(6, 68);
            detailedConnectionsTimelineGroupBox.Name = "detailedConnectionsTimelineGroupBox";
            detailedConnectionsTimelineGroupBox.Size = new Size(693, 151);
            detailedConnectionsTimelineGroupBox.TabIndex = 7;
            detailedConnectionsTimelineGroupBox.TabStop = false;
            detailedConnectionsTimelineGroupBox.Text = "Connections";
            // 
            // detailedConnectionsTimelineChart
            // 
            detailedConnectionsTimelineChart.Dock = DockStyle.Fill;
            detailedConnectionsTimelineChart.Location = new Point(3, 19);
            detailedConnectionsTimelineChart.Name = "detailedConnectionsTimelineChart";
            detailedConnectionsTimelineChart.Size = new Size(687, 129);
            detailedConnectionsTimelineChart.TabIndex = 0;
            // 
            // detailedStatisticsDeviceComboBox
            // 
            detailedStatisticsDeviceComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            detailedStatisticsDeviceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            detailedStatisticsDeviceComboBox.FormattingEnabled = true;
            detailedStatisticsDeviceComboBox.Location = new Point(6, 6);
            detailedStatisticsDeviceComboBox.Name = "detailedStatisticsDeviceComboBox";
            detailedStatisticsDeviceComboBox.Size = new Size(693, 23);
            detailedStatisticsDeviceComboBox.TabIndex = 0;
            detailedStatisticsDeviceComboBox.SelectedIndexChanged += detailedStatisticsDeviceComboBoxSelectedIndexChanged;
            // 
            // mainStatusStrip
            // 
            mainStatusStrip.Items.AddRange(new ToolStripItem[] { mainToolStripStatusLabel, mainToolStripProgressBar });
            mainStatusStrip.Location = new Point(0, 476);
            mainStatusStrip.Name = "mainStatusStrip";
            mainStatusStrip.RenderMode = ToolStripRenderMode.Professional;
            mainStatusStrip.Size = new Size(751, 22);
            mainStatusStrip.TabIndex = 8;
            mainStatusStrip.Text = "mainStatusStrip";
            // 
            // mainToolStripStatusLabel
            // 
            mainToolStripStatusLabel.Image = Properties.Resources.cloud_warning17;
            mainToolStripStatusLabel.ImageAlign = ContentAlignment.MiddleLeft;
            mainToolStripStatusLabel.Name = "mainToolStripStatusLabel";
            mainToolStripStatusLabel.Size = new Size(534, 17);
            mainToolStripStatusLabel.Spring = true;
            mainToolStripStatusLabel.Text = "Inaction";
            // 
            // mainToolStripProgressBar
            // 
            mainToolStripProgressBar.Alignment = ToolStripItemAlignment.Right;
            mainToolStripProgressBar.MarqueeAnimationSpeed = 50;
            mainToolStripProgressBar.Name = "mainToolStripProgressBar";
            mainToolStripProgressBar.Size = new Size(200, 16);
            mainToolStripProgressBar.Step = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(751, 498);
            Controls.Add(mainStatusStrip);
            Controls.Add(tabControl1);
            Controls.Add(powerPanel);
            Name = "MainForm";
            Text = "AraneaOculus.Manager.UI";
            ((System.ComponentModel.ISupportInitialize)portNumericUpDown).EndInit();
            powerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)qrPictureBox).EndInit();
            tabControl1.ResumeLayout(false);
            devicesTabPage.ResumeLayout(false);
            statisticsTabPage2.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            statisticsCommonTabPage.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            connectionsNumberGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)connectionsNumberChart).EndInit();
            networkLoadGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)networkLoadChart).EndInit();
            statisticsDetailedTabPage.ResumeLayout(false);
            detailedNetworkLoadTimelineGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)detailedNetworkLoadTimelineChart).EndInit();
            detailedConnectionsTimelineGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)detailedConnectionsTimelineChart).EndInit();
            mainStatusStrip.ResumeLayout(false);
            mainStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button powerButton;
        private NumericUpDown portNumericUpDown;
        private Label statusLabel;
        private Panel powerPanel;
        private ListView devicesListView;
        private TabControl tabControl1;
        private TabPage devicesTabPage;
        private TabPage statisticsTabPage2;
        private StatusStrip mainStatusStrip;
        private ToolStripStatusLabel mainToolStripStatusLabel;
        private ComboBox networkInterfacesComboBox;
        private ToolStripProgressBar mainToolStripProgressBar;
        private TabControl tabControl2;
        private TabPage statisticsCommonTabPage;
        private TabPage statisticsDetailedTabPage;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox networkLoadGroupBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart networkLoadChart;
        private GroupBox connectionsNumberGroupBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart connectionsNumberChart;
        private ComboBox detailedStatisticsDeviceComboBox;
        private GroupBox detailedConnectionsTimelineGroupBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart detailedConnectionsTimelineChart;
        private GroupBox detailedNetworkLoadTimelineGroupBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart detailedNetworkLoadTimelineChart;
        private Label detailedStatisticsDeviceInformationLabel;
        private PictureBox qrPictureBox;
    }
}
