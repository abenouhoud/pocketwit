﻿using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class UpdateForm : Form
    {
        private UpdateChecker.UpdateInfo _NewVersion;
        public UpdateChecker.UpdateInfo NewVersion 
        {
            set
            {
                lblVersion.Text = value.webVersion.ToString();
                lblInfo.Text = value.UpdateNotes;
                _NewVersion = value;
            }
        }
        public UpdateForm()
        {
            InitializeComponent();
        }

        private void menuUpdate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
            pi.FileName = _NewVersion.DownloadURL;
            pi.UseShellExecute = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
            Application.Exit();
        }

        private void menuIgnore_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}