namespace AutoItBot
{
    partial class AutoItCommands
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
            this.label1 = new System.Windows.Forms.Label();
            this.autoItAction = new System.Windows.Forms.ComboBox();
            this.doAutoItAction = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.instructions = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.values = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Action:";
            // 
            // autoItAction
            // 
            this.autoItAction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.autoItAction.FormattingEnabled = true;
            this.autoItAction.Location = new System.Drawing.Point(69, 17);
            this.autoItAction.Name = "autoItAction";
            this.autoItAction.Size = new System.Drawing.Size(494, 21);
            this.autoItAction.TabIndex = 1;
            this.autoItAction.SelectedIndexChanged += new System.EventHandler(this.ActionChanged);
            // 
            // doAutoItAction
            // 
            this.doAutoItAction.Location = new System.Drawing.Point(571, 17);
            this.doAutoItAction.Name = "doAutoItAction";
            this.doAutoItAction.Size = new System.Drawing.Size(37, 20);
            this.doAutoItAction.TabIndex = 2;
            this.doAutoItAction.Text = "Go";
            this.doAutoItAction.UseVisualStyleBackColor = true;
            this.doAutoItAction.Click += new System.EventHandler(this.DoAutoItActionClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Instructions:";
            // 
            // instructions
            // 
            this.instructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.instructions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.instructions.Location = new System.Drawing.Point(12, 90);
            this.instructions.Name = "instructions";
            this.instructions.Size = new System.Drawing.Size(596, 115);
            this.instructions.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 216);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Values:";
            // 
            // values
            // 
            this.values.AcceptsReturn = true;
            this.values.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.values.Location = new System.Drawing.Point(12, 232);
            this.values.Multiline = true;
            this.values.Name = "values";
            this.values.ReadOnly = true;
            this.values.Size = new System.Drawing.Size(595, 505);
            this.values.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(69, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(135, 20);
            this.button1.TabIndex = 7;
            this.button1.Text = "Repeat Checksum";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.GetChecksum);
            // 
            // AutoItCommands
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 759);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.values);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.instructions);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.doAutoItAction);
            this.Controls.Add(this.autoItAction);
            this.Controls.Add(this.label1);
            this.Name = "AutoItCommands";
            this.Text = "Auto It Commands";
            this.Load += new System.EventHandler(this.AutoItCommands_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CheckKeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox autoItAction;
        private System.Windows.Forms.Button doAutoItAction;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label instructions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox values;
        private System.Windows.Forms.Button button1;
    }
}

