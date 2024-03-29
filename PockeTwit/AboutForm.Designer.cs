namespace PockeTwit
{
    partial class AboutForm
    {

		#region Fields (9) 

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuClose;
        private System.Windows.Forms.MenuItem menuUpdate;

		#endregion Fields 

		#region Methods (1) 


		// Protected Methods (1) 

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


		#endregion Methods 


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuUpdate = new System.Windows.Forms.MenuItem();
            this.menuClose = new System.Windows.Forms.MenuItem();
            this.lblVersion = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSys = new System.Windows.Forms.TextBox();
            this.lblWait = new System.Windows.Forms.Label();
            this.lnkContributors = new System.Windows.Forms.LinkLabel();
            this.lnkSystem = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuUpdate);
            this.mainMenu1.MenuItems.Add(this.menuClose);
            // 
            // menuUpdate
            // 
            this.menuUpdate.Text = "Check Upgrades";
            this.menuUpdate.Click += new System.EventHandler(this.menuUpdate_Click);
            // 
            // menuClose
            // 
            this.menuClose.Text = "Close";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblVersion.ForeColor = System.Drawing.Color.LightGray;
            this.lblVersion.Location = new System.Drawing.Point(7, 5);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(125, 20);
            this.lblVersion.Text = "v0.0";
            // 
            // linkLabel1
            // 
            this.linkLabel1.ForeColor = System.Drawing.Color.LightBlue;
            this.linkLabel1.Location = new System.Drawing.Point(95, 50);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(108, 14);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.Text = "@PockeTwitDev";
            this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // linkLabel2
            // 
            this.linkLabel2.ForeColor = System.Drawing.Color.LightBlue;
            this.linkLabel2.Location = new System.Drawing.Point(4, 50);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(85, 14);
            this.linkLabel2.TabIndex = 0;
            this.linkLabel2.Text = "Visit Website";
            this.linkLabel2.Click += new System.EventHandler(this.linkLabel2_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.txtSys);
            this.panel1.Controls.Add(this.lblWait);
            this.panel1.Location = new System.Drawing.Point(4, 93);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(195, 77);
            this.panel1.GotFocus += new System.EventHandler(this.panel1_GotFocus);
            // 
            // txtSys
            // 
            this.txtSys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSys.Location = new System.Drawing.Point(0, 0);
            this.txtSys.Multiline = true;
            this.txtSys.Name = "txtSys";
            this.txtSys.ReadOnly = true;
            this.txtSys.Size = new System.Drawing.Size(195, 77);
            this.txtSys.TabIndex = 5;
            this.txtSys.Visible = false;
            // 
            // lblWait
            // 
            this.lblWait.Location = new System.Drawing.Point(4, 4);
            this.lblWait.Name = "lblWait";
            this.lblWait.Size = new System.Drawing.Size(100, 20);
            this.lblWait.Text = "Please wait...";
            // 
            // lnkContributors
            // 
            this.lnkContributors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkContributors.Location = new System.Drawing.Point(5, 70);
            this.lnkContributors.Name = "lnkContributors";
            this.lnkContributors.Size = new System.Drawing.Size(195, 20);
            this.lnkContributors.TabIndex = 2;
            this.lnkContributors.Text = "Contributors: (How can you help?)";
            this.lnkContributors.Click += new System.EventHandler(this.lnkContributors_Click);
            // 
            // lnkSystem
            // 
            this.lnkSystem.Location = new System.Drawing.Point(4, 28);
            this.lnkSystem.Name = "lnkSystem";
            this.lnkSystem.Size = new System.Drawing.Size(135, 17);
            this.lnkSystem.TabIndex = 5;
            this.lnkSystem.Text = "System Information...";
            this.lnkSystem.Visible = false;
            this.lnkSystem.Click += new System.EventHandler(this.lnkSystem_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(176, 173);
            this.Controls.Add(this.lnkSystem);
            this.Controls.Add(this.lnkContributors);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.lblVersion);
            this.Menu = this.mainMenu1;
            this.Name = "AboutForm";
            this.Text = "About PockeTwit";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblWait;
        private System.Windows.Forms.LinkLabel lnkContributors;
        private System.Windows.Forms.TextBox txtSys;
        private System.Windows.Forms.LinkLabel lnkSystem;
    }
}