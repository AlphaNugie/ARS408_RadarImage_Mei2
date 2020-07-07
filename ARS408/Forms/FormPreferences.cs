using ARS408.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARS408.Forms
{
    public partial class FormPreferences : Form
    {
        public FormPreferences()
        {
            InitializeComponent();
            this.RefreshControls();
        }

        private void RefreshControls()
        {
            this.checkBox_ShowDeserted.Checked = BaseConst.ShowDesertedPoints;
        }

        private void CheckBox_ShowDeserted_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Main", "ShowDesertedPoints", this.checkBox_ShowDeserted.Checked ? "1" : "0");
        }
    }
}
