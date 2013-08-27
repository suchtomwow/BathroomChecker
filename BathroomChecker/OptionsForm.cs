using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BathroomChecker
{
    public partial class OptionsForm : Form
    {
        private static readonly string AppName = "BathroomChecker";
        public OptionsForm()
        {
            InitializeComponent();
            setCheckBox();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (startCheckBox.Checked)
                rk.SetValue(AppName, Application.ExecutablePath.ToString());
            else
                rk.DeleteValue(AppName, false); 
        }

        private void setCheckBox()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
            if (rk.GetValue(AppName) == null)
            {
                startCheckBox.Checked = false;
            }
            else
            {
                startCheckBox.Checked = true;
            }
        }
    }
}
