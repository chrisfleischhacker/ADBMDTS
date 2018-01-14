using System;
using System.Diagnostics;
using System.Windows.Forms;
using ADBMDTS.Properties;
using System.Drawing;
using System.Configuration;
using System.Threading;

namespace ADBMDTS
{
	/// <summary>
	/// 
	/// </summary>
	class ContextMenus
	{
		/// <summary>
		/// Is the About box displayed?
		/// </summary>
        bool isAboutLoaded = false;
        bool isMessageLoaded = false;

		/// <summary>
		/// Creates this instance.
		/// </summary>
		/// <returns>ContextMenuStrip</returns>
		public ContextMenuStrip Create()
		{
			// Add the default menu options.
			ContextMenuStrip menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			ToolStripSeparator sep;

			// Sync Attachments.
			item = new ToolStripMenuItem();
			item.Text = "Sync Attachments";
			item.Click += new EventHandler(Sync_Attachments_Click);
			item.Image = Resources.SyncStart;
			menu.Items.Add(item);

            // Run ADBM.
            item = new ToolStripMenuItem();
            item.Text = "Run ADBM";
            item.Click += new EventHandler(Run_ADBM_Click);
            item.Image = Resources.ADBM;
            menu.Items.Add(item);

			// About.
			item = new ToolStripMenuItem();
			item.Text = "About";
			item.Click += new EventHandler(About_Click);
			item.Image = Resources.About;
			menu.Items.Add(item);

			// Separator.
			sep = new ToolStripSeparator();
			menu.Items.Add(sep);

			// Exit.
			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new System.EventHandler(Exit_Click);
			item.Image = Resources.SyncExit;
			menu.Items.Add(item);

			return menu;
		}

		/// <summary>
		/// Handles the Click event of the Explorer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Sync_Attachments_Click(object sender, EventArgs e)
		{
            StartUp();
		}

        /// <summary>
        /// Handles the Click event of the Explorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Run_ADBM_Click(object sender, EventArgs e)
        {
            Process.Start(@"c:\adbmw\adbmwin.exe", null);
        }

		/// <summary>
		/// Handles the Click event of the About control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void About_Click(object sender, EventArgs e)
		{
			if (!isAboutLoaded)
			{
				isAboutLoaded = true;
				new AboutBox().ShowDialog();
				isAboutLoaded = false;
			}
		}

		/// <summary>
		/// Processes a menu item.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Exit_Click(object sender, EventArgs e)
		{
			// Quit without further ado.
			Application.Exit();
		}


        protected void StartUp()
        {
            if (InternetAvailable.PingNetwork("adbmftp.ADBM.com"))
            {
                SyncChangedAttachmentFolders();
            }
            else{ShowMessage("ADBM FTP Not Available");}
        }

        private string GetRemotePathToThisAttachment(int p1, int p2)
        {
            string path;
            path = ConfigurationManager.AppSettings["pathRemoteAttachments"].ToString();
            if (p1 != 0) { path += @"\By_Log\" + p1; } else { path += @"\By_ERefNo\" + p2; }
            return path;
        }

        private string GetHostPathToThisAttachment(int p1, int p2)
        {
            string path;
            path = ConfigurationManager.AppSettings["pathHostAttachments"].ToString();
            if (p1 != 0) { path += @"/By_Log/" + p1; } else { path += @"/By_ERefNo/" + p2; }
            return path;
        }

        protected void SyncChangedAttachmentFolders()
        {
            string sel, hostpath, remotepath;
            sel = @"select LogX, ERefNo from _Attachments where changed = 1";
            AttachmentCollection changed = Attachments.GetCollection(sel);
            if ((changed == null) || (changed.Count <= 0)) { ShowMessage("No Attachments Require Syncing."); }
            else
            {
                foreach (Attachment a in changed)
                {
                    hostpath = GetHostPathToThisAttachment(a.Logx, a.ERefNo);
                    remotepath = GetRemotePathToThisAttachment(a.Logx, a.ERefNo);
                    int xxx = 0;
                    xxx = SFTPInterface.SynchronizeAttachmentFolder(remotepath, hostpath);
                    if (xxx == 0) { Attachment.ClearAttachmentChangedFlag(a); }
                    else { ShowMessage("Attachment Sync Failed for Folder " + hostpath); }
                }
            }
       }

        private void ShowMessage(string p)
        {
            if (!isMessageLoaded)
            {
                isMessageLoaded = true;
                new MessageBox(p).ShowDialog();
                isMessageLoaded = false;
            }
        }

        private void Sleep(int numberofmilliseconds)
        {
            //sleeptime = Convert.ToInt32(60000 * Convert.ToDecimal(ConfigurationManager.AppSettings["SyncSleepMinutes"]));
            Thread.Sleep(numberofmilliseconds); // 1000 = 1 second, 60000 = 1 minute, 3,600,000 = 1 hour
            StartUp();
        }
	}
}