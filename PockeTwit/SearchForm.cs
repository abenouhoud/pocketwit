using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class SearchForm : Form
    {
        private delegate void delEnableDistance();

        public string GPSLocation = null;
        private LocationManager Locator = new LocationManager();
        

		#region Constructors (1) 

        public SearchForm()
        {
            Locator.LocationReady += new LocationManager.delLocationReady(Locator_LocationReady);
            InitializeComponent();
            SetUpSearchBox();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            this.DialogResult = DialogResult.Cancel;

            if (ClientSettings.UseGPS)
            {
                lblGPSStatus.Text = "Searching for GPS...";
                Locator.StartGPS();
                cmbMeasurement.SelectedValue = ClientSettings.DistancePreference;
                cmbMeasurement.Text = ClientSettings.DistancePreference;
            }
            else
            {
                lblGPSStatus.Text = "GPS disabled.";
            }
            
        }

        void Locator_LocationReady(string Location)
        {
            SetLabelVisible();
            GPSLocation = Location;
            EnableDistance();
        }

        private void EnableDistance()
        {
            if (InvokeRequired)
            {
                delEnableDistance d = new delEnableDistance(EnableDistance);
                this.Invoke(d, null);
            }
            else
            {
                lblWithin.Visible = true;
                cmbDistance.Visible = true;
                cmbMeasurement.Visible = true;
                cmbDistance.Enabled = true;
                cmbMeasurement.Enabled = true;
            }
        }
        private delegate void delSetLabelVisible();
        private void SetLabelVisible()
        {
            if (InvokeRequired)
            {
                delSetLabelVisible d = new delSetLabelVisible(SetLabelVisible);
                this.Invoke(d, null);
            }
            else
            {
                lblGPSStatus.Visible = false;
            }
        }
        
		#endregion Constructors 

		#region Properties (1) 

        public string SearchText{get;set;}

		#endregion Properties 

		#region Methods (2) 


		// Private Methods (2) 

        private void menuCancel_Click(object sender, EventArgs e)
        {
            if (ClientSettings.UseGPS)
            {
                Locator.StopGPS();
            }
            this.DialogResult = DialogResult.Cancel;

        }

        private void menuSearch_Click(object sender, EventArgs e)
        {
            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                if (!string.IsNullOrEmpty(txtSearch.Text) && !ClientSettings.SearchItems.Contains(txtSearch.Text))
                {
                    ClientSettings.SearchItems.Enqueue(txtSearch.Text);
                    if (ClientSettings.SearchItems.Count > 4)
                    {
                        ClientSettings.SearchItems.Dequeue();
                    }
                    ClientSettings.SaveSettings();
                }
            }
            if (ClientSettings.UseGPS)
            {
                Locator.StopGPS();
            }
            StringBuilder b = new StringBuilder();
            if(!string.IsNullOrEmpty(txtSearch.Text))
            {
                b.Append("q=");
                b.Append(System.Web.HttpUtility.UrlEncode(txtSearch.Text));
                if(!string.IsNullOrEmpty(cmbDistance.Text))
                {
                    b.Append("&");
                }
            }
            if (!string.IsNullOrEmpty(cmbDistance.Text))
            {
                b.Append("geocode=" + this.GPSLocation);
                b.Append("," + cmbDistance.Text);
                switch(cmbMeasurement.Text)
                {
                    case "Miles":
                        b.Append("mi");
                        break;
                    case "Kilometers":
                        b.Append("km");
                        break;
                }
                
            }
            this.SearchText = b.ToString();
            
            this.DialogResult = DialogResult.OK;
        }


		#endregion Methods 

        private void cmbMeasurement_SelectedValueChanged(object sender, EventArgs e)
        {
            ClientSettings.DistancePreference = (string)cmbMeasurement.SelectedValue;
            ClientSettings.SaveSettings();
        }


        private void SetUpSearchBox()
        {

            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                this.txtSearch = new System.Windows.Forms.ComboBox();
                ComboBox txtSearchBox = (ComboBox)txtSearch;
                txtSearchBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
                foreach (string Item in ClientSettings.SearchItems)
                {
                    txtSearchBox.Items.Add(Item);
                }
            }
            else
            {
                this.txtSearch = new System.Windows.Forms.TextBox();
            }
            
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(58, 55);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(179, 22);
            txtSearch.BringToFront();
            this.txtSearch.TabIndex = 8;

            
            this.Controls.Add(this.txtSearch);
            this.ResumeLayout(false);

        }
    }
}