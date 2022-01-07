using ARS408.Core;
using ARS408.Model;
using CommonLib.UIControlUtil;
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
    public partial class FormInfo : Form
    {
        private readonly DataService_Radar dataService = new DataService_Radar();
        private readonly int _width = 240, _height = 60; //TextBox默认尺寸
        private readonly List<TextBoxWrapper> _wrappers;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="monitor">父窗体</param>
        public FormInfo(FormMonitor monitor)
        {
            InitializeComponent();
            _width = textBox_Radar_Sample.Width;
            _height = textBox_Radar_Sample.Height;
            if (BaseConst.RadarList != null && BaseConst.RadarList.Count > 0)
                _wrappers = BaseConst.RadarList.Select(radar => new TextBoxWrapper(radar.Id, string.Format("textBox_Radar{0}", radar.Id), _width, _height)).ToList();
            if (flowLayoutPanel_TextBoxes.Controls.Count > 0)
                foreach (Control control in flowLayoutPanel_TextBoxes.Controls)
                    control.Dispose();
            flowLayoutPanel_TextBoxes.Controls.Clear();
            _wrappers.ForEach(wrapper => flowLayoutPanel_TextBoxes.Controls.Add(wrapper.Control));
        }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public FormInfo() : this(null) { }

        private void FormInfo_Load(object sender, EventArgs e)
        {
            Start();
        }

        public void Start()
        {
            timer1.Start();
            button_Start.Enabled = false;
            button_Stop.Enabled = true;
        }

        public void Stop()
        {
            timer1.Stop();
            button_Start.Enabled = true;
            button_Stop.Enabled = false;
        }

        private void Button_Start_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void Button_Stop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            label_RadarCount.Text = BaseConst.RadarList.Count.ToString();
            label_RadarWorkingCount.Text = BaseConst.RadarList.Where(radar => radar.Working == 1).Count().ToString();
            RefreshRadarInfos();
            textBox_Info.Text = BaseFunc.GetInfoString();
        }

        //private void RefreshRadarInfos()
        //{
        //    int i = 1;
        //    TextBox textBox;
        //    foreach (Radar radar in BaseConst.RadarList)
        //    {
        //        try
        //        {
        //            double d;
        //            textBox = (TextBox)Controls.Find("textBox_Radar" + i, false)[0];
        //            textBox.Text = BaseFunc.GetRadarString(radar, out d);
        //            i++;
        //            if (BaseConst.Save2Database && radar.GroupType == RadarGroupType.Wheel)
        //                dataService.InsertRadarDistance(radar.Id, radar.Name, d);
        //        }
        //        catch (Exception) { }
        //    }
        //}

        private void RefreshRadarInfos()
        {
            BaseConst.RadarList.ForEach(radar =>
            {
                try
                {
                    double d;
                    _wrappers.FirstOrDefault(w => w.Id == radar.Id).Text = radar.GetRadarString(out d);
                    if (BaseConst.Save2Database && radar.GroupType == RadarGroupType.Wheel)
                        dataService.InsertRadarDistance(radar.Id, radar.Name, d);
                }
                catch (Exception) { }

            });
        }
    }
}
