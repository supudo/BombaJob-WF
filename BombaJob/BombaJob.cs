using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BombaJob
{
    public partial class BombaJob : Form
    {
        public BombaJob()
        {
            InitializeComponent();
            this.Load += new EventHandler(BombaJob_Load);
        }

        private void BombaJob_Load(object sender, EventArgs e)
        {
            this.initMainMenu();
        }

        #region Main Menu
        private void initMainMenu()
        {
            this.addMenuItem(AppSettings.GetLanguageValue("menu_Newest"), "1", true);
            this.addMenuItem(AppSettings.GetLanguageValue("menu_Offers"), "2", true);
            this.addMenuItem(AppSettings.GetLanguageValue("menu_People"), "3", true);
            this.addMenuItem(AppSettings.GetLanguageValue("menu_Search"), "4", true);
            this.addMenuItem(AppSettings.GetLanguageValue("menu_Post"), "5", true);
            this.addMenuItem(AppSettings.GetLanguageValue("menu_Settings"), "6", true);
            this.addMenuItem(AppSettings.GetLanguageValue("menu_About"), "7", false);
        }

        private void addMenuItem(string text, string tag, bool addSeparator)
        {
            ToolStripMenuItem mmi = new ToolStripMenuItem();
            mmi.Tag = tag;
            mmi.Text = text;
            mmi.Click += new EventHandler(mmi_Click);
            this.menuStrip.Items.Insert(this.menuStrip.Items.Count, mmi);
            if (addSeparator)
                this.menuStrip.Items.Add(new ToolStripSeparator());
        }

        private void mmi_Click(object sender, EventArgs e)
        {
            string tag = (string)((ToolStripMenuItem)sender).Tag;
            switch (tag)
            {
                case "1":
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
