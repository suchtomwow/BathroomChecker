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
        private readonly NotifyIcon _notifyIcon;
        private const string Site = "http://kalosdev/bathroom.php";
        private const string OpenResponse = "<div><img src=\"images/Open.jpg\"  height=\"100\" width=\"60\" /></div>";
        private const string ClosedResponse = "<div><img src=\"images/Closed.jpg\"  height=\"100\" width=\"60\" /></div>";
        private const string Status = "Bathroom Status: ";
        private const string OpenToolTip = "Vacant";
        private const string ClosedToolTip = "Occupied";
        private const string PendingToolTip = "Unknown";
        private string _oldStatus;

        public BathroomChecker(NotifyIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
        }

        public void CheckStatus()
        {
            var status = ReadStatus();
            var check = (status.Equals(_oldStatus) == false);

            if (check)
            {
                var tooltip = PendingToolTip;
                var icon = Properties.Resources.signal_yellow;

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

                _notifyIcon.Icon.Dispose();
                _notifyIcon.Icon = icon;
                _notifyIcon.Text = Status + tooltip;
            } 
            
            _oldStatus = status;
        }

        private string ReadStatus()
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
