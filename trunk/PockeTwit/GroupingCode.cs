﻿using System;

using System.Data.SQLite;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PockeTwit
{
    public class SpecialTimeLine
    {
        public struct groupTerm
        {
            public string Term;
            public bool Exclusive;
        }

        public string name { get; set; }
        public groupTerm[] Terms { get; set; }

        public void AddItem(string Term, bool Exclusive)
        {
            groupTerm newTerm = new groupTerm() { Term = Term, Exclusive = Exclusive };
            if (Terms != null && Terms.Length > 0)
            {
                List<groupTerm> items = new List<groupTerm>(Terms);
                if (!items.Contains(newTerm))
                {
                    items.Add(newTerm);
                }
                Terms = items.ToArray();
            }
            else
            {
                Terms = new groupTerm[] { newTerm };
            }
        }
        public void RemoveItem(string Term)
        {
            List<groupTerm> items = new List<groupTerm>(Terms);
            groupTerm toRemove = new groupTerm();
            foreach (groupTerm t in items)
            {
                if (t.Term == Term)
                {
                    toRemove = t;
                }
            }
            if (items.Contains(toRemove))
            {
                items.Remove(toRemove);
            }
            Terms = items.ToArray();
            SpecialTimeLines.Save();
        }

        public string GetConstraints()
        {
            string ret = "";
            List<string> UserList = new List<string>();
            foreach (groupTerm t in Terms)
            {
                UserList.Add("'"+t.Term+"'");
                
            }
            if (UserList.Count > 0)
            {
                ret = " AND statuses.userid IN(" + string.Join(",", UserList.ToArray()) + ") ";
            }

            return ret;
        }
    }

    public static class SpecialTimeLines
    {
        private static string configPath = ClientSettings.AppPath + "\\savedTimelines.xml";
        private static Dictionary<string, SpecialTimeLine> _Items = new Dictionary<string, SpecialTimeLine>();

        public static SpecialTimeLine[] GetList()
        {
            List<SpecialTimeLine> s = new List<SpecialTimeLine>();
            lock (_Items)
            {
                foreach (SpecialTimeLine item in _Items.Values)
                {
                    s.Add(item);
                }
            }
            return s.ToArray();
        }
        public static void Add(SpecialTimeLine newLine)
        {
            lock (_Items)
            {
                if (!_Items.ContainsKey(newLine.name))
                {
                    _Items.Add(newLine.name, newLine);
                }
            }
        }
        public static void Remove(SpecialTimeLine oldLine)
        {
            lock (_Items)
            {
                if(_Items.ContainsKey(oldLine.name))
                {
                    _Items.Remove(oldLine.name);
                }
            }
        }
        public static void Clear()
        {
            lock (_Items)
            {
                _Items.Clear();
            }
        }

        public static void Load()
        {
            
            using (SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                conn.Open();
                using (SQLiteCommand comm = new SQLiteCommand(conn))
                {
                    comm.CommandText = "SELECT groupname, userid, exclusive FROM usersInGroups";
                    using (SQLiteDataReader r = comm.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string groupName = r.GetString(0);
                            string userID = r.GetString(1);
                            bool exclusive = r.GetBoolean(2);
                            SpecialTimeLine thisLine = new SpecialTimeLine();
                            if (_Items.ContainsKey(groupName))
                            {
                                thisLine = _Items[groupName];
                            }
                            else
                            {
                                thisLine.name = groupName;
                                Add(thisLine);
                            }
                            thisLine.AddItem(userID,exclusive);
                        }
                    }
                }
            }
        }
        public static void Save()
        {
            
            if (_Items.Count > 0)
            {
                using (SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
                {
                    lock (_Items)
                    {
                        conn.Open();
                        using (SQLiteTransaction t = conn.BeginTransaction())
                        {
                            foreach (SpecialTimeLine group in _Items.Values)
                            {
                                using (SQLiteCommand comm = new SQLiteCommand(conn))
                                {
                                    comm.CommandText = "INSERT INTO groups (groupname) VALUES (@name);";
                                    comm.Parameters.Add(new SQLiteParameter("@name", group.name));

                                    comm.ExecuteNonQuery();
                                    
                                    foreach (SpecialTimeLine.groupTerm groupItem in group.Terms)
                                    {
                                        comm.Parameters.Clear();
                                        comm.CommandText = "INSERT INTO usersInGroups (id, groupname, userid, exclusive) VALUES (@pairid, @name, @userid, @exclusive)";
                                        comm.Parameters.Add(new SQLiteParameter("@pairid", group.name + groupItem.Term));
                                        comm.Parameters.Add(new SQLiteParameter("@name", group.name));
                                        comm.Parameters.Add(new SQLiteParameter("@userid", groupItem.Term));
                                        comm.Parameters.Add(new SQLiteParameter("@exclusive", groupItem.Exclusive));
                                        comm.ExecuteNonQuery();

                                    }
                                }
                            }
                            t.Commit();
                        }
                    }
                }
            }
        }

        internal static SpecialTimeLine GetFromName(string ListName)
        {
            SpecialTimeLine ret = null;
            foreach (SpecialTimeLine t in GetList())
            {
                if ("Grouped_TimeLine_" + t.name == ListName)
                {
                    ret = t;
                }
            }
            return ret;
        }
    }
}
