using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.IO;

namespace BathroomChecker
{
    public class BathroomChecker
    {
        private readonly NotifyIcon notifyIcon;
        private static readonly string OpenIcon = "signal_green.ico";
        private static readonly string ClosedIcon = "signal_red.ico";
        private static readonly string PendingIcon = "signal_yellow.ico";
        private static readonly string Site = "http://kalosdev/bathroom.php";
        private static readonly string OpenResponse = "<div><img src=\"images/Open.jpg\"  height=\"100\" width=\"60\" /></div>";
        private static readonly string ClosedResponse = "<div><img src=\"images/Closed.jpg\"  height=\"100\" width=\"60\" /></div>";
        private static readonly string Status = "Bathroom Status: ";
        private static readonly string OpenToolTip = "Vacant";
        private static readonly string ClosedToolTip = "Occupied";
        private static readonly string PendingToolTip = "Unknown";
        private string OldStatus;

        public BathroomChecker(NotifyIcon notifyIcon)
        {
            this.notifyIcon = notifyIcon;
        }

        public void checkStatus()
        {
            string status = readStatus();
            bool check = (status.Equals(OldStatus) == false);

            if (check)
            {
                string tooltip = PendingToolTip;
                Icon icon = Properties.Resources.signal_yellow;

                if (status.Equals(OpenResponse))
                {
                    icon = Properties.Resources.signal_green;
                    tooltip = OpenToolTip;
                }
                else if (status.Equals(ClosedResponse))
                {
                    icon = Properties.Resources.signal_red;
                    tooltip = ClosedToolTip;
                }

                this.notifyIcon.Icon.Dispose();
                this.notifyIcon.Icon = icon;
                this.notifyIcon.Text = Status + tooltip;
            } 
            
            OldStatus = status;
        }

        private string readStatus()
        {
            var client = new WebClient();

            // Add a user agent header in case the 
            // requested URI contains a query.

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            string result;
            using (var data = client.OpenRead(Site))
            {
                using (var reader = new StreamReader(data))
                {
                    result = reader.ReadToEnd();
                }
                
            }

            GC.Collect();

            return result;
        }

        private ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, int enabledCount, int disabledCount, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null) { item.Click += eventHandler; }

            item.Image = null;
            return item;
        }

        public ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler)
        {
            return ToolStripMenuItemWithHandler(displayText, 0, 0, eventHandler);
        }

    }
}
