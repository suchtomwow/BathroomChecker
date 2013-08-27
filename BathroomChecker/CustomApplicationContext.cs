using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Timers;

namespace BathroomChecker
{
    /// <summary>
    /// Framework for running application as a tray app.
    /// </summary>
    /// <remarks>
    /// Tray app code adapted from "Creating Applications with NotifyIcon in Windows Forms", Jessica Fosler,
    /// http://windowsclient.net/articles/notifyiconapplications.aspx
    /// </remarks>
    public class CustomApplicationContext : ApplicationContext
    {
        private static readonly string IconFileName = "signal_yellow.ico";
        private static readonly string DefaultTooltip = "Men's Bathroom Status";
        private readonly BathroomChecker brChecker;

        /// <summary>
        /// This class should be created and passed into Application.Run( ... )
        /// </summary>
        public CustomApplicationContext() 
        {
            InitializeContext();
            brChecker = new BathroomChecker(notifyIcon);

            brChecker.checkStatus();

            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += CheckBathroomStatus;
            timer.Start();
            CheckBathroomStatus(null, null);
        }

        private void CheckBathroomStatus(object sender, ElapsedEventArgs args)
        {
            brChecker.checkStatus();
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            notifyIcon.ContextMenuStrip.Items.Clear();
            notifyIcon.ContextMenuStrip.Items.Add(brChecker.ToolStripMenuItemWithHandler("&Options", showOptionsItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(brChecker.ToolStripMenuItemWithHandler("&Exit", exitItem_Click));
        }

        # region the child forms

        private OptionsForm optionsForm;
        
        private void ShowOptionsForm()
        {
            if (optionsForm == null)
            {
                optionsForm = new OptionsForm();
                optionsForm.Closed += optionsForm_Closed; // avoid reshowing a disposed form
                optionsForm.Show();
            }
            else { optionsForm.Activate(); }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e) { ShowOptionsForm();    }

 
        // From http://stackoverflow.com/questions/2208690/invoke-notifyicons-context-menu
        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }
 
        // attach to context menu items
        private void showOptionsItem_Click(object sender, EventArgs e)  { ShowOptionsForm();  }

        // null out the forms so we know to create a new one.
        private void optionsForm_Closed(object sender, EventArgs e)     { optionsForm = null; }

        # endregion the child forms

        # region generic code framework

        private System.ComponentModel.IContainer components;	// a list of components to dispose when the context is disposed
        private NotifyIcon notifyIcon;				            // the icon that sits in the system tray

        private void InitializeContext()
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(components)
                             {
                                 ContextMenuStrip = new ContextMenuStrip(),
                                 Icon = new Icon(IconFileName),
                                 Text = DefaultTooltip,
                                 Visible = true
                             };
            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            //notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.MouseUp += notifyIcon_MouseUp;
        }

        /// <summary>
        /// When the application context is disposed, dispose things like the notify icon.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && components != null) { components.Dispose(); }
        }

        /// <summary>
        /// When the exit menu item is clicked, make a call to terminate the ApplicationContext.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitItem_Click(object sender, EventArgs e) 
        {
            ExitThread();
        }

        /// <summary>
        /// If we are presently showing a form, clean it up.
        /// </summary>
        protected override void ExitThreadCore()
        {
            // before we exit, let forms clean themselves up.
            //if (introForm != null) { introForm.Close(); }
            //if (detailsForm != null) { detailsForm.Close(); }

            notifyIcon.Visible = false; // should remove lingering tray icon
            base.ExitThreadCore();
        }

        # endregion generic code framework

    }
}
