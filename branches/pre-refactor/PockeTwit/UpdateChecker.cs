﻿using System;

using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;

namespace PockeTwit
{
    public class UpdateChecker
    {
        public double currentVersion = .20;
        private string UpdateURL = "http://pocketwit.googlecode.com/svn/LatestRelease/Release.xml";
        private string XMLResponse;
        private UpdateInfo WebVersion;
        public delegate void delUpdateFound(UpdateInfo Info);
        public event delUpdateFound UpdateFound;
        public event delUpdateFound CurrentVersion;

        public struct UpdateInfo
        {
            public double webVersion;
            public string DownloadURL;
            public string UpdateNotes;
        }

        
        public UpdateChecker()
        {
            if (ClientSettings.CheckVersion)
            {
                CheckForUpdate();
            }
        }
        public UpdateChecker(bool Auto)
        {
            if (Auto)
            {
                CheckForUpdate();
            }
        }

        public void CheckForUpdate()
        {
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetWebResponse);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "CheckUpdates";
            t.Start();
        }

        private void GetWebResponse()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UpdateURL);

            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        XMLResponse = reader.ReadToEnd();
                    }
                }
            }
            catch{}
            XmlDocument UpdateInfoDoc = new XmlDocument();
            try
            {
                if (XMLResponse != null)
                {
                    UpdateInfoDoc.LoadXml(XMLResponse);
                    WebVersion = new UpdateInfo();
                    WebVersion.webVersion = double.Parse(UpdateInfoDoc.SelectSingleNode("//version").InnerText,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    WebVersion.DownloadURL = UpdateInfoDoc.SelectSingleNode("//url").InnerText;
                    WebVersion.UpdateNotes = UpdateInfoDoc.SelectSingleNode("//notes").InnerText;

                    if (WebVersion.webVersion > currentVersion)
                    {
                        if (UpdateFound != null)
                        {
                            UpdateFound(WebVersion);
                        }
                    }
                    else
                    {
                        if (CurrentVersion != null)
                        {
                            CurrentVersion(WebVersion);
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("Update check complete");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Update check failed");
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Unable to check for upgrades.\r\nIf the problem persists let me know @PockeTwitdev", "Upgrade error.");

            }
        }
    

        
    }
}