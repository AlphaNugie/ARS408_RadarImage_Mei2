﻿using ARS408.Core;
using ARS408.Model;
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
        private FormMonitor monitor = null;
        private DataService_Radar dataService = new DataService_Radar();

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="monitor">父窗体</param>
        public FormInfo(FormMonitor monitor)
        {
            InitializeComponent();
            this.monitor = monitor;
        }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public FormInfo() : this(null) { }

        private void FormInfo_Load(object sender, EventArgs e)
        {
            this.Start();
        }

        public void Start()
        {
            this.timer1.Start();
            this.button_Start.Enabled = false;
            this.button_Stop.Enabled = true;
        }

        public void Stop()
        {
            this.timer1.Stop();
            this.button_Start.Enabled = true;
            this.button_Stop.Enabled = false;
        }

        private void Button_Start_Click(object sender, EventArgs e)
        {
            this.Start();
        }

        private void Button_Stop_Click(object sender, EventArgs e)
        {
            this.Stop();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (this.monitor == null)
                return;

            this.label_RadarCount.Text = BaseConst.RadarList.Count.ToString();
            this.label_DistThres.Text = BaseConst.BorderDistThres.ToString();
            this.RefreshRadarInfos();
            this.textBox_Info.Text = BaseFunc.GetInfoString();
        }

        private void RefreshRadarInfos()
        {
            int i = 1;
            TextBox textBox;
            foreach (Radar radar in BaseConst.RadarList)
            {
                try
                {
                    double d;
                    textBox = (TextBox)this.Controls.Find("textBox_Radar" + i, false)[0];
                    textBox.Text = BaseFunc.GetRadarString(radar, out d);
                    i++;
                    if (BaseConst.Save2Database && radar.GroupType == RadarGroupType.Wheel)
                        this.dataService.InsertRadarDistance(radar.Id, radar.Name, d);
                }
                catch (Exception) { }
            }
        }
    }
}
