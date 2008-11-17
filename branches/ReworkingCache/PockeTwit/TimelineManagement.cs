﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace PockeTwit
{
    public class TimelineManagement
    {
        #region Events
        public delegate void delFriendsUpdated(int count);
        public delegate void delMessagesUpdated(int count);
        public delegate void delBothUpdated(int Messagecount, int FreindsCount);
        public delegate void delProgress(int percentage, string Status);
        public delegate void delComplete();
        public delegate void delNullReturnedByAccount(Yedda.Twitter.Account t, Yedda.Twitter.ActionType Action);
        public event delFriendsUpdated FriendsUpdated;
        public event delMessagesUpdated MessagesUpdated;
        public event delBothUpdated BothUpdated;
        public event delProgress Progress;
        public event delComplete CompleteLoaded;
        public event delNullReturnedByAccount NoData;
        public event delNullReturnedByAccount ErrorCleared;

        #endregion
        public bool RunInBackground = true;
        public enum TimeLineType
        {
            Friends,
            Messages
        }
        public Dictionary<TimeLineType, TimeLine> TimeLines = new Dictionary<TimeLineType, TimeLine>();
        private Dictionary<Yedda.Twitter, string> LastStatusID = new Dictionary<Yedda.Twitter, string>();
        private Dictionary<Yedda.Twitter, string> LastReplyID = new Dictionary<Yedda.Twitter, string>();
        private Dictionary<Yedda.Twitter, string> LastDirectID = new Dictionary<Yedda.Twitter, string>();
        private System.Threading.Timer timerUpdate;
        private List<Yedda.Twitter> TwitterConnections;
        private int HoldNewMessages = 0;
        private int HoldNewFriends = 0;
        private RegistryKey PockeTwitKey;
        private RegistryKey StatusIDs;
        private RegistryKey ReplyIDs;
        private RegistryKey DirectIDs;


        private void SetUp()
        {
            PockeTwitKey = Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("PockeTwit");
            StatusIDs = Registry.CurrentUser.OpenSubKey("Software\\PockeTwit", true).CreateSubKey("LastStatusID");
            ReplyIDs = Registry.CurrentUser.OpenSubKey("Software\\PockeTwit", true).CreateSubKey("LastReplyID");
            DirectIDs = Registry.CurrentUser.OpenSubKey("Software\\PockeTwit", true).CreateSubKey("LastDirectID");
        }

        private void LoadIDsFromRegistry()
        {
            foreach (Yedda.Twitter t in TwitterConnections)
            {
                LastStatusID.Add(t, "");
                LastReplyID.Add(t, "");
                LastDirectID.Add(t, "");
            }
            foreach (Yedda.Twitter t in this.TwitterConnections)
            {
                LastStatusID[t] = (string)StatusIDs.GetValue(t.AccountInfo.ToString());
                LastReplyID[t] = (string)ReplyIDs.GetValue(t.AccountInfo.ToString());
                LastDirectID[t] = (string)DirectIDs.GetValue(t.AccountInfo.ToString());
            }
        }
        
        

        private void SaveIDsToregistry()
        {
            foreach (Yedda.Twitter t in LastStatusID.Keys)
            {
                if (LastStatusID[t] != null)
                {
                    StatusIDs.SetValue(t.AccountInfo.ToString(), LastStatusID[t]);
                }
            }
            foreach (Yedda.Twitter t in LastReplyID.Keys)
            {
                if (LastReplyID[t] != null)
                {
                    ReplyIDs.SetValue(t.AccountInfo.ToString(), LastReplyID[t]);
                }
            }
            foreach (Yedda.Twitter t in LastDirectID.Keys)
            {
                if (LastDirectID[t] != null)
                {
                    DirectIDs.SetValue(t.AccountInfo.ToString(), LastDirectID[t]);
                }
            }
        }
        
        public TimelineManagement(List<Yedda.Twitter> TwitterConnectionsToFollow)
        {
            SetUp();
        }

        public void Startup(List<Yedda.Twitter> TwitterConnectionsToFollow)
        {
            TwitterConnections = TwitterConnectionsToFollow;
            LoadIDsFromRegistry();
            Progress(0, "Starting");
            TimeLines.Add(TimeLineType.Friends, new TimeLine());
            TimeLines.Add(TimeLineType.Messages, new TimeLine());
            /*
            
             */
            Progress(0, "Loading Cache");
            LoadCachedtimeline();
            Progress(0, "Fetching Friends TimeLine");
            GetFriendsTimeLine();
            Progress(0, "Fetching Messages TimeLine");
            GetMessagesTimeLine();
            CompleteLoaded();
            if (ClientSettings.UpdateInterval > 0)
            {
                timerUpdate = new System.Threading.Timer(new System.Threading.TimerCallback(timerUpdate_Tick), null, ClientSettings.UpdateInterval, ClientSettings.UpdateInterval);
            }
        }

        void timerUpdate_Tick(object state)
        {
            if (RunInBackground)
            {
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(BackgroundUpdate);
                System.Threading.Thread t = new System.Threading.Thread(ts);
                t.Name = "BackgroundUpdate";
                t.IsBackground = true;
                t.Start();
            }
            else
            {
                BackgroundUpdate();
            }
        }
    
        private void BackgroundUpdate()
        {
            //if (NotificationHandler.NotifyOfFriends)
            //{
                GetFriendsTimeLine(false);
            //}
            //if (NotificationHandler.NotifyOfMessages)
            //{
                GetMessagesTimeLine(false);
            //}
            if (HoldNewMessages > 0 && HoldNewFriends > 0)
            {
                if (BothUpdated != null)
                {
                    BothUpdated(HoldNewMessages, HoldNewFriends);
                }
            }
            else if (HoldNewMessages > 0)
            {
                if (MessagesUpdated != null)
                {
                    MessagesUpdated(HoldNewMessages);
                }
            }
            else if (HoldNewFriends > 0)
            {
                if (FriendsUpdated != null)
                {
                    FriendsUpdated(HoldNewFriends);
                }
            }
            HoldNewFriends = 0;
            HoldNewMessages = 0;
            GC.Collect();
        }

        public void RefreshFriendsTimeLine()
        {
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetFriendsTimeLine);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "BackgroundUpdate";
            t.IsBackground = true;
            t.Start();
        }

        public void RefreshMessagesTimeLine()
        {
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetMessagesTimeLine);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "BackgroundUpdate";
            t.IsBackground = true;
            t.Start();
        }

        private void LoadCachedtimeline()
        {
            if (System.IO.File.Exists(ClientSettings.AppPath + "\\" + "FriendsTime.xml"))
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(ClientSettings.AppPath + "\\" + "FriendsTime.xml"))
                {
                    string s = r.ReadToEnd();
                    Library.status[] newstats = Library.status.Deserialize(s);
                    TimeLine cachedLine = new TimeLine(newstats);
                    TimeLines[TimeLineType.Friends] = cachedLine;
                }
            }
        }

        public Library.status[] SearchTwitter(Yedda.Twitter t, string SearchString)
        {
            string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Search, SearchString);
            if (string.IsNullOrEmpty(response))
            {
                NoData(t.AccountInfo, Yedda.Twitter.ActionType.Search);
                return null;
            }
            else
            {
                ErrorCleared(t.AccountInfo, Yedda.Twitter.ActionType.Search);
            }
            return Library.status.DeserializeFromAtom(response, t.AccountInfo);
        }

        public Library.status[] GetUserTimeLine(Yedda.Twitter t, string UserID)
        {
            string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Show, UserID);
            if (string.IsNullOrEmpty(response))
            {
                NoData(t.AccountInfo, Yedda.Twitter.ActionType.Show);
                return null;
            }
            else
            {
                ErrorCleared(t.AccountInfo, Yedda.Twitter.ActionType.Show);
            }
            return Library.status.Deserialize(response, t.AccountInfo);
        }

        private void GetMessagesTimeLine()
        {
            GetMessagesTimeLine(true);
        }
        private void GetMessagesTimeLine(bool Notify)
        {
            List<Library.status> TempLine = new List<PockeTwit.Library.status>();
            foreach (Yedda.Twitter t in TwitterConnections)
            {
                if (t.AccountInfo.Enabled && t.AccountInfo.ServerURL.ServerType != Yedda.Twitter.TwitterServer.pingfm)
                {
                    string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Replies);
                    if (!string.IsNullOrEmpty(response))
                    {
                        Library.status[] NewStats = Library.status.Deserialize(response, t.AccountInfo, PockeTwit.Library.StatusTypes.Reply);
                        TempLine.AddRange(NewStats);
                        if (NewStats.Length > 0)
                        {
                            LastReplyID[t] = NewStats[0].id;
                            SaveIDsToregistry();
                        }
                        ErrorCleared(t.AccountInfo, Yedda.Twitter.ActionType.Replies);
                    }
                    else
                    {
                        NoData(t.AccountInfo, Yedda.Twitter.ActionType.Replies);
                    }
                    ////I HATE DIRECT MESSAGES
                    
                    if (t.DirectMessagesWork)
                    {
                        response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Direct_Messages);
                        if (!string.IsNullOrEmpty(response))
                        {
                            Library.status[] NewStats = Library.status.FromDirectReplies(response, t.AccountInfo);
                            TempLine.AddRange(NewStats);
                            if (NewStats.Length > 0)
                            {
                                LastStatusID[t] = NewStats[0].id;
                                SaveIDsToregistry();
                            }
                            ErrorCleared(t.AccountInfo, Yedda.Twitter.ActionType.Direct_Messages);
                        }
                        else
                        {
                            NoData(t.AccountInfo, Yedda.Twitter.ActionType.Direct_Messages);
                        }
                    }
                }
            }
            int NewItems = TimeLines[TimeLineType.Messages].MergeIn(TempLine);
            if (MessagesUpdated != null && NewItems>0)
            {
                if (Notify)
                {
                    MessagesUpdated(NewItems);
                }
                else
                {
                    HoldNewMessages = NewItems;
                }
            }
            TempLine.Clear();
            TempLine.TrimExcess();
        }

        private void GetFriendsTimeLine()
        {
            GetFriendsTimeLine(true);
        }
        private void GetFriendsTimeLine(bool Notify)
        {
            List<Library.status> TempLine = new List<PockeTwit.Library.status>();
            foreach (Yedda.Twitter t in TwitterConnections)
            {
                if (t.AccountInfo.Enabled && t.AccountInfo.ServerURL.ServerType != Yedda.Twitter.TwitterServer.pingfm)
                {
                    string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Friends_Timeline);

                    if (!string.IsNullOrEmpty(response))
                    {
                        Library.status[] NewStats = Library.status.Deserialize(response, t.AccountInfo);
                        TempLine.AddRange(NewStats);
                        if (NewStats.Length > 0)
                        {
                            LastStatusID[t] = NewStats[0].id;
                            SaveIDsToregistry();
                        }
                        ErrorCleared(t.AccountInfo, Yedda.Twitter.ActionType.Friends_Timeline);
                    }
                    else
                    {
                        NoData(t.AccountInfo, Yedda.Twitter.ActionType.Friends_Timeline);
                    }
                }
            }
            int NewItems = TimeLines[TimeLineType.Friends].MergeIn(TempLine);
            
            if (FriendsUpdated != null && NewItems>0)
            {
                if (Notify)
                {
                    FriendsUpdated(NewItems);
                }
                else
                {
                    HoldNewFriends = NewItems;
                }
                SaveStatuses(TimeLines[TimeLineType.Friends].ToArray());
            }
            TempLine.Clear();
            TempLine.TrimExcess();
        }

        private void SaveStatuses(PockeTwit.Library.status[] statuses)
        {
            if (statuses.Length <= 20)
            {
                //No need to cache less than 20 tweets.  
                return;
            }
            string StatusString = Library.status.Serialize(statuses);

            using (System.IO.TextWriter w = new System.IO.StreamWriter(ClientSettings.AppPath + "\\" + "FriendsTime.xml"))
            {
                w.Write(StatusString);
                w.Flush();
                w.Close();  //Shouldn't need this in using, but I'm desperate   
            }
        }

        private string FetchSpecificFromTwitter(Yedda.Twitter t, Yedda.Twitter.ActionType TimelineType)
        {
            return FetchSpecificFromTwitter(t, TimelineType, null);
        }
        private string FetchSpecificFromTwitter(Yedda.Twitter t, Yedda.Twitter.ActionType TimelineType, string AdditionalParameter)
        {
            string response = "";
            try
            {
                switch (TimelineType)
                {
                    case Yedda.Twitter.ActionType.Direct_Messages:
                        response = t.GetDirectTimeLineSince(LastDirectID[t]);
                        break;
                    case Yedda.Twitter.ActionType.Friends_Timeline:
                        if (!t.BigTimeLines)
                        {
                            response = t.GetFriendsTimeline(Yedda.Twitter.OutputFormatType.XML);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(LastStatusID[t]))
                            {
                                response = t.GetFriendsTimeLineMax(Yedda.Twitter.OutputFormatType.XML);
                            }
                            else
                            {
                                response = t.GetFriendsTimeLineSince(Yedda.Twitter.OutputFormatType.XML, LastStatusID[t]);
                            }
                        }
                        break;
                    case Yedda.Twitter.ActionType.Public_Timeline:
                        response = t.GetPublicTimeline(Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Replies:
                        if (!t.BigTimeLines)
                        {
                            response = t.GetRepliesTimeLine(Yedda.Twitter.OutputFormatType.XML);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(LastReplyID[t]))
                            {
                                response = t.GetRepliesTimeLine(Yedda.Twitter.OutputFormatType.XML);
                            }
                            else
                            {
                                response = t.GetRepliesTimeLineSince(Yedda.Twitter.OutputFormatType.XML, LastReplyID[t]);
                            }
                        }
                        break;
                    case Yedda.Twitter.ActionType.User_Timeline:
                        response = t.GetUserTimeline(AdditionalParameter, Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Show:
                        response = t.GetUserTimeline(AdditionalParameter, Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Favorites:
                        response = t.GetFavorites();
                        break;
                    case Yedda.Twitter.ActionType.Search:
                        response = t.SearchFor(AdditionalParameter);
                        break;
                }
            }
            catch (Exception ex)
            {
            }
            return response;
        }


        internal void UpdateImagesForUser(string User, string NewURL)
        {
            try
            {
                foreach (TimeLine t in this.TimeLines.Values)
                {
                    foreach (Library.status stat in t)
                    {
                        if (stat.user.screen_name == User)
                        {
                            if (string.IsNullOrEmpty(stat.user.profile_image_url))
                            {
                                stat.user.profile_image_url = NewURL;
                            }
                            else
                            {
                                Uri U1 = new Uri(stat.user.profile_image_url);
                                Uri U2 = new Uri(NewURL);
                                if (U1.Host == U2.Host)
                                {
                                    stat.user.profile_image_url = NewURL;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}