namespace AutoItBot
{
    partial class BotHomeScreen
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
            this.btnStart = new System.Windows.Forms.Button();
            this.lblRunningStatus = new System.Windows.Forms.Label();
            this.chkHaltScreenSaver = new System.Windows.Forms.CheckBox();
            this.btnUpdatePrices = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.chkSingleThread = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(221, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(131, 27);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Bot";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.BtnStartClick);
            // 
            // lblRunningStatus
            // 
            this.lblRunningStatus.AutoSize = true;
            this.lblRunningStatus.Location = new System.Drawing.Point(12, 479);
            this.lblRunningStatus.Name = "lblRunningStatus";
            this.lblRunningStatus.Size = new System.Drawing.Size(103, 13);
            this.lblRunningStatus.TabIndex = 1;
            this.lblRunningStatus.Text = "Status: Not Running";
            // 
            // chkHaltScreenSaver
            // 
            this.chkHaltScreenSaver.AutoSize = true;
            this.chkHaltScreenSaver.Checked = true;
            this.chkHaltScreenSaver.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHaltScreenSaver.Location = new System.Drawing.Point(12, 12);
            this.chkHaltScreenSaver.Name = "chkHaltScreenSaver";
            this.chkHaltScreenSaver.Size = new System.Drawing.Size(148, 17);
            this.chkHaltScreenSaver.TabIndex = 3;
            this.chkHaltScreenSaver.Text = "Keep Screen Saver Away";
            this.chkHaltScreenSaver.UseVisualStyleBackColor = true;
            // 
            // btnUpdatePrices
            // 
            this.btnUpdatePrices.Location = new System.Drawing.Point(452, 6);
            this.btnUpdatePrices.Name = "btnUpdatePrices";
            this.btnUpdatePrices.Size = new System.Drawing.Size(128, 27);
            this.btnUpdatePrices.TabIndex = 4;
            this.btnUpdatePrices.Text = "Update Prices";
            this.btnUpdatePrices.UseVisualStyleBackColor = true;
            this.btnUpdatePrices.Click += new System.EventHandler(this.BtnUpdatePricesClick);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(452, 472);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(128, 27);
            this.btnTest.TabIndex = 5;
            this.btnTest.Text = "Test Functionality";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.BtnTestClick);
            // 
            // chkSingleThread
            // 
            this.chkSingleThread.AutoSize = true;
            this.chkSingleThread.Checked = true;
            this.chkSingleThread.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSingleThread.Location = new System.Drawing.Point(12, 35);
            this.chkSingleThread.Name = "chkSingleThread";
            this.chkSingleThread.Size = new System.Drawing.Size(123, 17);
            this.chkSingleThread.TabIndex = 6;
            this.chkSingleThread.Text = "Run as single thread";
            this.chkSingleThread.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(452, 439);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 27);
            this.button1.TabIndex = 7;
            this.button1.Text = "Launch Pixel Finder";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.LaunchPixelFinder);
            // 
            // BotHomeScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 501);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkSingleThread);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnUpdatePrices);
            this.Controls.Add(this.chkHaltScreenSaver);
            this.Controls.Add(this.lblRunningStatus);
            this.Controls.Add(this.btnStart);
            this.Name = "BotHomeScreen";
            this.Text = "BotHomeScreen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblRunningStatus;
        private System.Windows.Forms.CheckBox chkHaltScreenSaver;
        private System.Windows.Forms.Button btnUpdatePrices;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.CheckBox chkSingleThread;
        private System.Windows.Forms.Button button1;
    }
}