﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace FingerUI
{
    public class StatusItem : IDisposable, IComparable
    {

        public static char[] IgnoredAtChars = new char[] { ':', ',', '-', '.', '!', '?', '~','=','&','*','>',')' };

		#region�Fields�(15)�

        private Graphics _ParentGraphics;
        private PockeTwit.Library.status _Tweet;
        private Rectangle currentOffset;
        private Rectangle m_bounds;
        private bool hasFavoriteStar = false;
        private KListControl m_parent;
        private bool m_selected = false;
        private StringFormat m_stringFormat = new StringFormat();
        private string m_text;
        private object m_value;
        private int m_x = -1;
        private int m_y = -1;
        private PockeTwit.Library.User ReplyUser = null;
        private Font TextFont;
        private Font SelectedFont;
        #endregion�Fields�

		#region�Constructors�(2)�

        /// <summary>
        /// Initializes a new instance of the <see cref="KListItem"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        public StatusItem(KListControl parent, string text, object value)
        {
            m_parent = parent;
            m_text = text;
            m_value = value;
            TextFont = m_parent.Font;
            SelectedFont = m_parent.SelectedFont;
        }

        public StatusItem()
        {
        }

		#endregion�Constructors�

		#region�Properties�(12)�

        /// <summary>
        /// The unscrolled bounds for this item.
        /// </summary>
        public Rectangle Bounds { get { return m_bounds; }
            set 
            {
                if (m_bounds.Width!=0 && value.Width != m_bounds.Width)
                {
                    ResetTexts();
                }
                m_bounds = value;
                Rectangle textBounds;
                if (ClientSettings.ShowAvatars)
                {
                    textBounds = new Rectangle(ClientSettings.SmallArtSize + ClientSettings.Margin, 0, m_bounds.Width - (ClientSettings.SmallArtSize + (ClientSettings.Margin * 2)), m_bounds.Height);
                }
                else
                {
                    textBounds = new Rectangle(ClientSettings.Margin, 0, m_bounds.Width - (ClientSettings.Margin * 2), m_bounds.Height);
                }
                BreakUpTheText(_ParentGraphics, textBounds);
            }
        }

        private void ResetTexts()
        {
            Tweet.SplitLines = new List<string>();
            Tweet.Clickables = new List<Clickable>();
        }

        public bool Highlighted { get { return hasFavoriteStar; } set { hasFavoriteStar = value; } }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public int Index { get { return m_y; } set { m_y = value; } }

        public bool isFavorite
        {
            get 
            {
                if(string.IsNullOrEmpty(Tweet.favorited))
                {
                    return false;
                }
                return bool.Parse(Tweet.favorited);
            }
            set
            {
                Tweet.favorited = value.ToString();
                this.Highlighted = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public KListControl Parent { get { return m_parent; } 
            set 
            {
                m_parent = value;
                TextFont = m_parent.Font;
                SelectedFont = m_parent.SelectedFont;
            }
        }

        public Graphics ParentGraphics 
        {
            set
            {
                _ParentGraphics = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KListItem"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected
        { 
            get 
            { 
                return m_selected;  
            } 
            set 
            {
                m_selected = value; 
            } 
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get { return m_text; } set { m_text = value; } }

        public PockeTwit.Library.status Tweet 
        {
            get { return _Tweet; }
            set
            {
                _Tweet = value;
                if (string.IsNullOrEmpty(value.favorited))
                {
                    hasFavoriteStar = false;
                }
                else
                {
                    hasFavoriteStar = bool.Parse(value.favorited);
                }
                if (Tweet.Clickables == null)
                {
                    Tweet.Clickables = new List<Clickable>();
                }
                if (Tweet.SplitLines == null)
                {
                    Tweet.SplitLines = new List<string>();
                }
                
            }

        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get { return m_value; } set { m_value = value; } }

        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public int XIndex { get { return m_x; } set { m_x = value; } }

        private float _LetterWidth = -1;
        
		#endregion�Properties�

		#region�Delegates�and�Events�(1)�


		//�Delegates�(1)�

        public delegate void ClickedWordDelegate(string TextClicked);

		#endregion�Delegates�and�Events�

		#region�Methods�(4)�


		//�Public�Methods�(2)�

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {

            m_parent = null;
        }

        /// <summary>
        /// Renders to the specified graphics.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="bounds">The bounds.</param>
        public virtual void Render(Graphics g)
        {
            Render(g, this.Bounds);
        }
        public virtual void Render(Graphics g, Rectangle bounds)
        {
            try
            {
                g.Clip = new Region(bounds);
                currentOffset = bounds;
                SolidBrush ForeBrush = new SolidBrush(ClientSettings.ForeColor);
                Rectangle textBounds;
                //Shrink the text area to accomidate avatars if appropriate
                if (ClientSettings.ShowAvatars)
                {
                    textBounds = new Rectangle(bounds.X + (ClientSettings.SmallArtSize + ClientSettings.Margin), bounds.Y, bounds.Width - (ClientSettings.SmallArtSize + (ClientSettings.Margin * 2)), bounds.Height);
                }
                else
                {
                    textBounds = new Rectangle(bounds.X + ClientSettings.Margin, bounds.Y, bounds.Width - (ClientSettings.Margin * 2), bounds.Height);
                }

                Rectangle InnerBounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
                InnerBounds.Offset(1, 1);
                InnerBounds.Width--; InnerBounds.Height--;

                if (m_selected)
                {
                    ForeBrush = new SolidBrush(ClientSettings.SelectedForeColor);
                    if (ClientSettings.SelectedBackColor != ClientSettings.SelectedBackGradColor)
                    {
                        try
                        {
                            Gradient.GradientFill.Fill(g, InnerBounds, ClientSettings.SelectedBackColor, ClientSettings.SelectedBackGradColor, Gradient.GradientFill.FillDirection.TopToBottom);
                        }
                        catch
                        {
                            using (Brush BackBrush = new SolidBrush(ClientSettings.SelectedBackColor))
                            {
                                g.FillRectangle(BackBrush, InnerBounds);
                            }
                        }
                    }
                    else
                    {
                        using (Brush BackBrush = new SolidBrush(ClientSettings.SelectedBackColor))
                        {
                            g.FillRectangle(BackBrush, InnerBounds);
                        }
                    }
                }
                else
                {
                    if (ClientSettings.BackColor != ClientSettings.BackGradColor)
                    {
                        try
                        {
                            Gradient.GradientFill.Fill(g, InnerBounds, ClientSettings.BackColor, ClientSettings.BackGradColor, Gradient.GradientFill.FillDirection.TopToBottom);
                        }
                        catch
                        {
                            using (Brush BackBrush = new SolidBrush(ClientSettings.BackColor))
                            {
                                g.FillRectangle(BackBrush, InnerBounds);
                            }
                        }
                    }
                    else
                    {
                        using (Brush BackBrush = new SolidBrush(ClientSettings.BackColor))
                        {
                            g.FillRectangle(BackBrush, InnerBounds);
                        }
                    }
                }


                Point ImageLocation = new Point(bounds.X + ClientSettings.Margin, bounds.Y + ClientSettings.Margin);

                //Add the timestamp if the settings call for it.
                if (ClientSettings.ShowExtra)
                {
                    Color SmallColor = ClientSettings.SmallTextColor;
                    if (this.Selected) { SmallColor = ClientSettings.SelectedSmallTextColor; }
                    using (Brush dateBrush = new SolidBrush(SmallColor))
                    {
                        g.DrawString(Tweet.TimeStamp, ClientSettings.SmallFont, dateBrush, bounds.Left + ClientSettings.Margin, ClientSettings.SmallArtSize + ClientSettings.Margin + bounds.Top, m_stringFormat);
                    }
                }

                //Get and draw the avatar area.
                if (ClientSettings.ShowAvatars)
                {
                    string artURL = Tweet.user.high_profile_image_url;
                    if (!ClientSettings.HighQualityAvatars)
                    {
                        artURL = Tweet.user.profile_image_url;
                    }
                    using (Image UserImage = PockeTwit.ThrottledArtGrabber.GetArt(Tweet.user.screen_name, artURL))
                    {
                        g.DrawImage(UserImage, ImageLocation.X, ImageLocation.Y);
                    }

                    //This is usually disabled, but we may draw a smaller avatar over the first one
                    if (ClientSettings.ShowReplyImages)
                    {
                        DrawReplyImage(g, ImageLocation);
                    }

                    //Only occasionally is an item "starred", but we draw one on there if it is.
                    if (hasFavoriteStar)
                    {
                        System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
                        ia.SetColorKey(PockeTwit.ThrottledArtGrabber.FavoriteImage.GetPixel(0, 0), PockeTwit.ThrottledArtGrabber.FavoriteImage.GetPixel(0, 0));
                        g.DrawImage(PockeTwit.ThrottledArtGrabber.FavoriteImage,
                            new Rectangle(bounds.X + 5, bounds.Y + 5, 7, 7), 0, 0, 7, 7, GraphicsUnit.Pixel, ia);
                    }

                    //If it's a reply or direct message, overlay that on the avatar
                    if (Tweet.TypeofMessage == PockeTwit.Library.StatusTypes.Reply)
                    {
                        using (Brush sBrush = new SolidBrush(ClientSettings.SelectedForeColor))
                        {

                            Rectangle ImageRect = new Rectangle(ImageLocation.X, ImageLocation.Y, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize);
                            Point sPoint = new Point(ImageRect.Right - 15, ImageRect.Top);

                            using (Brush bBrush = new SolidBrush(ClientSettings.SelectedBackColor))
                            {
                                g.FillRectangle(bBrush, new Rectangle(ImageRect.Right - 15, ImageRect.Top, 15, 15));
                            }
                            g.DrawString("@", TextFont, sBrush, new Rectangle(ImageRect.Right - 12, ImageRect.Top - 2, 10, 20));
                        }
                    }
                    else if (Tweet.TypeofMessage == PockeTwit.Library.StatusTypes.Direct)
                    {
                        using (Brush sBrush = new SolidBrush(ClientSettings.SelectedForeColor))
                        {
                            Rectangle ImageRect = new Rectangle(ImageLocation.X, ImageLocation.Y, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize);
                            Point sPoint = new Point(ImageRect.Right - 15, ImageRect.Top);

                            using (Brush bBrush = new SolidBrush(ClientSettings.SelectedBackColor))
                            {
                                g.FillRectangle(bBrush, new Rectangle(ImageRect.Right - 15, ImageRect.Top, 15, 15));
                            }
                            g.DrawString("D", TextFont, sBrush, new Rectangle(ImageRect.Right - 10, ImageRect.Top, 10, 20));
                        }

                    }
                }


                textBounds.Offset(ClientSettings.Margin, 1);
                textBounds.Width = textBounds.Width - ClientSettings.Margin;
                textBounds.Height--;

                m_stringFormat.Alignment = StringAlignment.Near;

                m_stringFormat.LineAlignment = StringAlignment.Near;
                BreakUpTheText(g, textBounds);
                int lineOffset = 0;


                for (int i = 0; i < Tweet.SplitLines.Count; i++)
                {
                    if (i >= ClientSettings.LinesOfText)
                    {
                        break;
                    }
                    float Position = ((lineOffset * (ClientSettings.TextSize)) + textBounds.Top);

                    g.DrawString(Tweet.SplitLines[i], TextFont, ForeBrush, textBounds.Left, Position, m_stringFormat);
                    lineOffset++;
                }
                if (ClientSettings.UseClickables)
                {
                    MakeClickable(g, textBounds);
                }
                ForeBrush.Dispose();
                g.Clip = new Region();
            }
            catch (ObjectDisposedException)
            {
            }
            
            
        }

        public override string ToString()
        {
            return this.Tweet.text;
        }

		//�Private�Methods�(2)�

        private void DrawReplyImage(Graphics g, Point ImageLocation)
        {
            ImageLocation.Offset(-5, -5);
            if (Tweet.SplitLines.Count > 0 && Tweet.SplitLines[0].IndexOf(' ')>0)
            {
                if (Tweet.SplitLines[0].Split(new char[] { ' ' })[0].StartsWith("@"))
                {
                    string ReplyTo = Tweet.SplitLines[0].Split(new char[] { ' ' })[0].TrimEnd(IgnoredAtChars).TrimStart('@');
                    Image ReplyImage = null;
                    if (!PockeTwit.ThrottledArtGrabber.HasArt(ReplyTo))
                    {
                        if (ReplyUser == null)
                        {
                            ReplyUser = PockeTwit.Library.User.FromId(ReplyTo, this.Tweet.Account);
                        }
                        if (ReplyUser != null)
                        {
                            if (ClientSettings.HighQualityAvatars)
                            {
                                ReplyImage = PockeTwit.ThrottledArtGrabber.GetArt(ReplyUser.screen_name, ReplyUser.high_profile_image_url);
                            }
                            else
                            {
                                ReplyImage = PockeTwit.ThrottledArtGrabber.GetArt(ReplyUser.screen_name, ReplyUser.profile_image_url);
                            }
                        }
                    }
                    else
                    {
                        ReplyImage = PockeTwit.ThrottledArtGrabber.GetArt(ReplyTo, "");
                    }

                    if (ReplyImage != null)
                    {
                        Rectangle ReplyRect = new Rectangle(ImageLocation.X + ClientSettings.Margin + (ClientSettings.SmallArtSize / 2), ImageLocation.Y + ClientSettings.Margin + (ClientSettings.SmallArtSize / 2), (ClientSettings.SmallArtSize / 2), (ClientSettings.SmallArtSize / 2));
                        g.DrawImage(ReplyImage, ReplyRect, new Rectangle(0, 0, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize), GraphicsUnit.Pixel);
                        using (Pen sPen = new Pen(ClientSettings.ForeColor))
                        {
                            g.DrawRectangle(sPen, ReplyRect);
                        }
                        ReplyImage.Dispose();
                    }
                }
            }
        }

        //texbounds is the area we're allowed to draw within
        //lineOffset is how many lines we've already drawn in these bounds
        private void MakeClickable(Graphics g, Rectangle textBounds)
        {
            
            using (Pen lPen = new Pen(ClientSettings.LinkColor))
            {
                using (Pen hPen = new Pen(ClientSettings.HashLinkColor))
                {
                    foreach (Clickable c in Tweet.Clickables)
                    {
                        Pen uPen = lPen;
                        if (c.Text.StartsWith("#")) { uPen = hPen; }
                        g.DrawLine(uPen, (int)c.Location.Left + textBounds.Left, (int)c.Location.Bottom + textBounds.Top,
                            (int)c.Location.Right + textBounds.Left, (int)c.Location.Bottom + textBounds.Top);
                    }
                }
                
            }
        }


		#endregion�Methods�

		#region�Nested�Classes�(1)�


        [Serializable]
        public class Clickable
        {

		#region�Fields�(2)�

            public RectangleF Location;
            public string Text;

		#endregion�Fields�

		#region�Methods�(2)�


		//�Public�Methods�(2)�

                        public override bool Equals(object obj)
            {
                Clickable otherClick = (Clickable)obj;
                if (otherClick.Location.Top == this.Location.Top &&
                    otherClick.Location.Left == this.Location.Left)
                {
                    return true;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }


		#endregion�Methods�

        }
		#endregion�Nested�Classes�


        #region Parsing Routines
        private void BreakUpTheTextWithoutSpaces(Graphics g, Rectangle textBounds)
        {
            string CurrentLine = System.Web.HttpUtility.HtmlDecode(this.Tweet.DisplayText);
            SizeF size = g.MeasureString(CurrentLine, TextFont);

            bool SpaceSplit = false;
            if (size.Width < textBounds.Width)
            {
                string line = CurrentLine.TrimStart(new char[] { ' ' });
                Tweet.SplitLines.Add(line);
                FindClickables(line, g, 0);
            }
            int LineOffset = 1;
            while (size.Width > textBounds.Width)
            {
                int lastBreak = 0;
                int currentPos = 0;
                StringBuilder newString = new StringBuilder();
                foreach (char c in CurrentLine)
                {
                    newString.Append(c);
                    if (g.MeasureString(newString.ToString(), TextFont).Width >= textBounds.Width - (ClientSettings.Margin*2))
                    {
                        if (!SpaceSplit | lastBreak == 0)
                        {
                            lastBreak = currentPos - 1;
                        }
                        newString = new StringBuilder(CurrentLine.Substring(0, lastBreak));
                        break;
                    }
                    if (c == ' ')
                    {
                        lastBreak = currentPos;
                    }
                    currentPos++;
                }
                string line = newString.ToString().TrimStart(new char[] { ' ' });
                Tweet.SplitLines.Add(line);
                FindClickables(line, g, LineOffset - 1);
                if (
                    Tweet.SplitLines.Count >= ClientSettings.LinesOfText || 
                    Tweet.text.IndexOf("http://shortText.com/")>0
                    )
                {
                    Tweet.Clipped = true;
                }
                if (lastBreak != 0)
                {
                    CurrentLine = CurrentLine.Substring(lastBreak);
                }
                size = g.MeasureString(CurrentLine, TextFont);
                if (size.Width <= textBounds.Width)
                {
                    line = CurrentLine.TrimStart(new char[] { ' ' });
                    Tweet.SplitLines.Add(line);
                    FindClickables(line, g, LineOffset);
                }
                LineOffset++;
            }
        }

        private void BreakUpTheText(Graphics g, Rectangle textBounds)
        {
            if (Tweet.SplitLines==null ||  Tweet.SplitLines.Count == 0)
            {
                //How could this happen!  We have no texts!
                if (this.Tweet.DisplayText == null) { return; }
                Tweet.SplitLines = new List<string>();
                string CurrentLine = System.Web.HttpUtility.HtmlDecode(this.Tweet.DisplayText).Replace('\n', ' ');
                FirstClickableRun(CurrentLine);
                SizeF size = g.MeasureString(CurrentLine, TextFont);

                string subText;
                if (this.Tweet.DisplayText.Split(' ')[0].StartsWith("@"))
                {
                    subText = this.Tweet.DisplayText.Substring(this.Tweet.DisplayText.IndexOf(' ') + 1);
                }
                else
                {
                    subText = this.Tweet.DisplayText;
                }

                if (subText.IndexOf(' ') <= 0)
                {
                    BreakUpTheTextWithoutSpaces(g, textBounds);
                    return;
                }
                if (size.Width < textBounds.Width)
                {
                    string line = CurrentLine.TrimStart(new char[] { ' '});
                    Tweet.SplitLines.Add(line);
                    FindClickables(line, g, 0);
                }
                int LineOffset = 1;
                bool bMulti;
                while (size.Width > textBounds.Width)
                {
                    int lastBreak = 0;
                    int currentPos = 0;
                    StringBuilder newString = new StringBuilder();
                    string[] Words = CurrentLine.Split(new char[]{' ','-'});
                    bMulti = false;
                    foreach (string word in Words)
                    {
                        newString.Append(word + ' ');    
                        if (g.MeasureString(newString.ToString(), TextFont).Width > textBounds.Width)
                        {
                            if (bMulti && lastBreak == 0)
                            {
                                lastBreak = currentPos;
                                newString = new StringBuilder(CurrentLine.Substring(0, lastBreak));
                            }
                            else
                            {
                                //First word is too long!
                                if (!isClickable(word))
                                {
                                    StringBuilder newWord = new StringBuilder();
                                    int letters = 1;
                                    foreach (char c in word)
                                    {
                                        newWord.Append(c);
                                        letters++;
                                        if (g.MeasureString(newWord.ToString(), TextFont).Width > textBounds.Width)
                                        {
                                            break;
                                        }
                                    }
                                    if (letters > 1) { letters--; }
                                    //If the word isn't a link, split it.
                                    newString = new StringBuilder(word.Substring(0, letters));
                                    currentPos = currentPos + letters;
                                    lastBreak = currentPos;
                                }
                                else
                                {
                                    //For now, we just move on to a new line                               
                                    currentPos = currentPos + word.Length + 1;
                                    lastBreak = currentPos;
                                }
                            }
                            
                            break;
                        }
                        bMulti = true;
                        currentPos = currentPos + word.Length + 1;
                    }
                    string line = newString.ToString().Trim(new char[] { ' ' });
                    Tweet.SplitLines.Add(line);
                    FindClickables(line, g, LineOffset-1);
                    if (Tweet.SplitLines.Count >= ClientSettings.LinesOfText || Tweet.text.IndexOf("http://shortText.com/") > 0)
                    {
                        Tweet.Clipped = true;
                    }
                    if (lastBreak > CurrentLine.Length)
                    {
                        lastBreak = CurrentLine.Length;
                    }
                    if (lastBreak != 0)
                    {
                        CurrentLine = CurrentLine.Substring(lastBreak);
                    }
                    size = g.MeasureString(CurrentLine, TextFont);
                    if (size.Width <= textBounds.Width)
                    {
                        line = CurrentLine.TrimStart(new char[] { ' ' });
                        Tweet.SplitLines.Add(line);
                        FindClickables(line,g,LineOffset);
                    }
                    LineOffset++;
                }
            }
        }

        private void FirstClickableRun(string text)
        {
            Tweet.Clickables = new List<Clickable>();
            string[] words = text.Split(new char[] { ' ' });
            foreach (string word in words)
            {
                if (isClickable(word))
                {
                    Clickable c = new Clickable();
                    c.Text = word.TrimEnd(IgnoredAtChars);
                    Tweet.Clickables.Add(c);
                }
            }
        }

        private static bool isClickable(string word)
        {
            return (word.StartsWith("http:") | word.StartsWith("@") | word.StartsWith("#")) && word.Length > 1;
        }
        private void FindClickables(string Line, Graphics g, int lineOffSet)
        {
            if (!ClientSettings.UseClickables) { return; }
            string[] Words = Line.Split(' ');
            StringBuilder LineBeforeThisWord = new StringBuilder();
            float Position = ((lineOffSet * (ClientSettings.TextSize)));
            for (int i = 0; i < Words.Length; i++)
            {
                string WordToCheck = Words[i].TrimEnd(IgnoredAtChars);
                if (!string.IsNullOrEmpty(WordToCheck))
                {
                    List<Clickable> OriginalClicks = new List<Clickable>(Tweet.Clickables);
                    foreach (Clickable c in OriginalClicks)
                    {
                        if (i == Words.Length - 1)
                        {
                            if (!string.IsNullOrEmpty(WordToCheck) && c.Text.StartsWith(WordToCheck.TrimEnd(IgnoredAtChars)))
                            {
                                float startpos = g.MeasureString(LineBeforeThisWord.ToString(), TextFont).Width;
                                //Find the size of the word
                                SizeF WordSize = g.MeasureString(Words[i], TextFont);
                                //A structure containing info we need to know about the word.
                                c.Location = new RectangleF(startpos, Position, WordSize.Width, WordSize.Height);

                                string SecondPart = null;
                                if (WordToCheck.Length < c.Text.Length)
                                {
                                    SecondPart = c.Text.Substring(WordToCheck.Length);
                                }

                                if (!string.IsNullOrEmpty(SecondPart))
                                {
                                    Clickable wrapClick = new Clickable();
                                    wrapClick.Text = c.Text;
                                    //Find the size of the word
                                    WordSize = g.MeasureString(SecondPart, TextFont);
                                    //A structure containing info we need to know about the word.
                                    float NextPosition = (((lineOffSet + 1) * (ClientSettings.TextSize)));
                                    wrapClick.Location = new RectangleF(0F, NextPosition, WordSize.Width, WordSize.Height);
                                    Tweet.Clickables.Add(wrapClick);
                                }
                            }
                        }
                        else if (WordToCheck.TrimEnd(IgnoredAtChars) == c.Text)
                        {
                            //Find out how far to the right this word will appear
                            float startpos = g.MeasureString(LineBeforeThisWord.ToString(), TextFont).Width;
                            //Find the size of the word
                            SizeF WordSize = g.MeasureString(Words[i], TextFont);
                            //A structure containing info we need to know about the word.
                            c.Location = new RectangleF(startpos, Position, WordSize.Width, WordSize.Height);
                            c.Text = WordToCheck.TrimEnd(IgnoredAtChars);
                        }
                    }
                }
                LineBeforeThisWord.Append(Words[i]+" ");
            }
        }
        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            StatusItem otherItem = (StatusItem)obj;
            return otherItem.Tweet.CompareTo(this.Tweet);
        }

        #endregion
    }
}