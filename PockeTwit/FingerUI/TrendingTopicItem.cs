﻿using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PockeTwit.FingerUI.Menu;
using System.Windows.Forms;

namespace PockeTwit.FingerUI
{
    class TrendingTopic
    {
        String name;
        String lastTrended;
        String firstTrended;
        String query;
        String description;

        public String Name
        {
          get { return name; }
          set { name = value; }
        }
        public String LastTrended
        {
            get { return lastTrended; }
            set { lastTrended = value; }
        }
        public String FirstTrended
        {
            get { return firstTrended; }
            set { firstTrended = value; }
        }
        public String Query
        {
            get { return query; }
            set { query = value; }
        }
        public String Description
        {
            get { return description; }
            set { description = value; }
        }
    }

    class TrendingTopicItem : IDisplayItem
    {
        TweetList _list;
        string _searchString;
        bool _saveResults;
        ISpecialTimeLine _timeLine;
        public TrendingTopic TrendingTopic;

        public TrendingTopicItem(TweetList list, string searchString, bool saveResults)
        {
            _list = list;
            _searchString = searchString;
            _saveResults = saveResults;
            Value = this.GetType().ToString();

        }
        public TrendingTopicItem(TweetList list, string searchString, TrendingTopic trendingTopic)
        {
            _list = list;
            _searchString = searchString;
            TrendingTopic = trendingTopic;
            Value = this.GetType().ToString();

        }
        public TrendingTopicItem(TweetList list, ISpecialTimeLine timeLine)
        {
            _list = list;
            _timeLine = timeLine;
            Value = this.GetType().ToString();
        }

        #region IDisplayItem Members

        Graphics _parentGraphics;
        public System.Drawing.Graphics ParentGraphics
        {
            set 
            {
                _parentGraphics = value;
            }
        }

        public KListControl Parent { get; set; }
        
        //public int Index { get; set; }
        public int Index { get { return _mY; } set { _mY = value; } }
        private int _mY = -1;

        public void OnMouseClick(Point p)
        {
            //OnMouseDblClick();
        }

        public void OnMouseDblClick()
        {
            //if (_timeLine == null) // direct Search
            //    _list.ShowSearchResults(_searchString, _saveResults, Yedda.Twitter.PagingMode.Forward);
            //else
            //    _list.ShowSpecialTimeLine(_timeLine, Yedda.Twitter.PagingMode.Forward);
        }


        public void Render(System.Drawing.Graphics g, System.Drawing.Rectangle bounds)
        {
            try
            {
                g.Clip = new Region(bounds);
                //_currentOffset = bounds;
                var foreBrush = new SolidBrush(ClientSettings.ForeColor);
                
                Rectangle textBounds = new Rectangle(bounds.X + ClientSettings.Margin, bounds.Y, bounds.Width - (ClientSettings.Margin * 2), bounds.Height);

                var innerBounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
                innerBounds.Offset(1, 1);
                innerBounds.Width--; innerBounds.Height--;
                DisplayItemDrawingHelper.DrawItemBackground(g, innerBounds, Selected);
                
                textBounds.Offset(ClientSettings.Margin, 1);
                textBounds.Height--;

                //BreakUpTheText(g, textBounds);
                //int lineOffset = 0;

                SizeF textSize = g.MeasureString(TrendingTopic.Name, ClientSettings.MenuFont);
                Point startPoint = new Point((int)(bounds.Left + (bounds.Width - textSize.Width) / 2), (int)(bounds.Top + (bounds.Height - textSize.Height) / 2));

                textBounds.Location = new Point(textBounds.X, textBounds.Y + startPoint.Y);

                textBounds.Height = 20;

                Color drawColor = ClientSettings.MenuTextColor;
                using (Brush drawBrush = new SolidBrush(drawColor))
                {
                    g.DrawString(TrendingTopic.Name, ClientSettings.MenuFont, drawBrush, startPoint.X, startPoint.Y - 20);
                    g.DrawString(TrendingTopic.Description, ClientSettings.MenuFont, drawBrush, new RectangleF(textBounds.Left, textBounds.Top, textBounds.Width, textBounds.Height));
                }


                //if (!ClientSettings.UseClickables)
                //{
                //    g.DrawString(Tweet.DisplayText, ClientSettings.TextFont, foreBrush, new RectangleF(textBounds.Left, textBounds.Top, textBounds.Width, textBounds.Height));
                //    //g.DrawString(Tweet.DisplayText, TextFont, ForeBrush, textBounds.Left, textBounds.Top, _mStringFormat);
                //}
                //else
                //{

                //    for (int i = 0; i < Tweet.SplitLines.Count; i++)
                //    {
                //        if (i >= ClientSettings.LinesOfText)
                //        {
                //            break;
                //        }
                //        float position = ((lineOffset * (ClientSettings.TextSize)) + textBounds.Top);

                //        g.DrawString(Tweet.SplitLines[i], ClientSettings.TextFont, foreBrush, textBounds.Left, position, _mStringFormat);
                //        lineOffset++;
                //    }
                //    MakeClickable(g, textBounds);
                //    foreBrush.Dispose();
                //}
                //g.Clip = new Region();
                //Tweet.SplitLines = null;
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public System.Drawing.Rectangle Bounds
        {
            get;
            set;
        }

        public bool Selected
        {
            get;
            set;
        }
        
        public object Value { get; set; }

        #endregion
    }
}
