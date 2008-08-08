﻿using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public class Clickables
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
        public bool Visible { get; set; }
        public event FingerUI.StatusItem.ClickedWordDelegate WordClicked;


        public List<FingerUI.StatusItem.Clickable> Items
        {
            set
            {
                TextItems = new List<string>();
                foreach (FingerUI.StatusItem.Clickable c in value)
                {
                    if (!TextItems.Contains(c.Text))
                    {
                        TextItems.Add(c.Text);
                    }
                }
                TextItems.Add("Exit");
            }
        }
        private List<string> TextItems = new List<string>(new string[]{"Exit"});
        private int _CurrentlyFocused = 0;
        public Clickables()
        {
            
        }

        public void KeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if(_CurrentlyFocused< TextItems.Count-1)
                {
                    _CurrentlyFocused++;
                }
            }
            if (e.KeyCode == Keys.Up)
            {
                if (_CurrentlyFocused > 0)
                {
                    _CurrentlyFocused--;
                }
            }
            if (e.KeyCode == Keys.Enter)
            {
                this.Visible = false;

                if (TextItems[_CurrentlyFocused] == "Exit")
                {
                    _CurrentlyFocused = 0;
                    return;
                }
                if (WordClicked != null)
                {
                    WordClicked(TextItems[_CurrentlyFocused]);
                    _CurrentlyFocused = 0;
                }
            }
            
        }
        
        public void Render(Graphics g)
        {
            int ItemHeight = (ClientSettings.TextSize * 2);
            int TopOfItem = ((this.Height / 2) - ((TextItems.Count * ItemHeight) / 2));
            

            int i = 0;
            using (Pen whitePen = new Pen(ClientSettings.ForeColor))
            {
                foreach (string Item in TextItems)
                {
                    Rectangle r = new Rectangle(this.Left, TopOfItem, this.Width, ItemHeight);
                    int TextTop = ((r.Bottom - r.Top) / 2) + r.Top;
                    Color BackColor;
                    if (i == _CurrentlyFocused)
                    {
                        BackColor = ClientSettings.SelectedBackColor;
                    }
                    else
                    {
                        BackColor = ClientSettings.BackColor;
                    }
                    using (Brush b = new SolidBrush(BackColor))
                    {
                        g.FillRectangle(b, r);
                        g.DrawRectangle(whitePen, r);
                        StringFormat sFormat = new StringFormat();
                        sFormat.LineAlignment = StringAlignment.Center;
                        using (Brush c = new SolidBrush(ClientSettings.ForeColor))
                        {
                            g.DrawString(Item, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold), c, r.Left + 4, TextTop, sFormat);
                        }
                    }
                    TopOfItem = TopOfItem + ItemHeight;
                    i++;
                }
            }
        }
    }
}
