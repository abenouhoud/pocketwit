using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class OtherSettings : Form
    {

        #region Constructors (1) 

        public OtherSettings()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            PopulateForm();
        }

		#endregion Constructors 

		#region Properties (1) 

        public bool NeedsReset { get; set; }

		#endregion Properties 

		#region Methods (4) 


		// Private Methods (4) 

        

        private void menuAccept_Click(object sender, EventArgs e)
        {
            IFormatProvider format = new System.Globalization.CultureInfo(1033);
            ClientSettings.UseGPS = chkGPS.Checked;
            ClientSettings.CheckVersion = chkVersion.Checked;
            ClientSettings.AutoTranslate = chkTranslate.Checked;
            ClientSettings.UseSkweezer = chkSkweezer.Checked;
            ClientSettings.RunOnStartUp = chkStartup.Checked;            if (string.IsNullOrEmpty(cmbMediaService.Items[cmbMediaService.SelectedIndex].ToString()))            {                ClientSettings.MediaService = "TwitPic";            }            else            {                ClientSettings.MediaService = cmbMediaService.Items[cmbMediaService.SelectedIndex].ToString();            }            if (ClientSettings.UpdateMinutes != int.Parse(txtUpdate.Text, format))
            {
                MessageBox.Show("You will need to restart PockeTwit for the update interval to change.", "PockeTwit");
                ClientSettings.UpdateMinutes = int.Parse(txtUpdate.Text, format);
            }
            if (ClientSettings.CacheDir != txtCaheDir.Text)
            {
                try
                {
                    if (!System.IO.Directory.Exists(txtCaheDir.Text))
                    {
                        System.IO.Directory.CreateDirectory(txtCaheDir.Text);
                    }
                    ClientSettings.CacheDir = txtCaheDir.Text;
                    ThrottledArtGrabber.ResetCacheDir();

                }
                catch
                {
                    MessageBox.Show("Unable to use that folder as a cache directory");
                }
            }
            ClientSettings.SaveSettings();
            
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void PopulateForm()
        {

            chkStartup.Checked = ClientSettings.RunOnStartUp;
            chkSkweezer.Checked = ClientSettings.UseSkweezer;
            chkVersion.Checked = ClientSettings.CheckVersion;
            chkGPS.Checked = ClientSettings.UseGPS;
            txtUpdate.Text = ClientSettings.UpdateMinutes.ToString();
            chkTranslate.Checked = ClientSettings.AutoTranslate;
            txtCaheDir.Text = ClientSettings.CacheDir;
            setMediaService(ClientSettings.MediaService);
            cmbMediaService.SelectedValue = 
            chkTranslate.Text = "Auto-translate to " + ClientSettings.TranslationLanguage;
            this.DialogResult = DialogResult.Cancel;
        }

        private void setMediaService(string value)
        {
            foreach (string comboValue in cmbMediaService.Items)
            {
                if (comboValue == value)
                {
                    cmbMediaService.SelectedItem = value;
                    return;
                }
            }
        }

		#endregion Methods 

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                System.Diagnostics.ProcessStartInfo ps = new System.Diagnostics.ProcessStartInfo("\\Windows\\ctlpnl.exe", "cplmain.cpl,9,1");
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo = ps;
                p.Start();
            }
            else
            {
                SettingsHandler.NotificationSettings n = new PockeTwit.SettingsHandler.NotificationSettings();
                n.ShowDialog();
            }
        }

        private void OtherSettings_Load(object sender, EventArgs e)
        {

        }

    }
}