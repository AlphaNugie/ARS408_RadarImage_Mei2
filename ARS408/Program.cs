using ARS408.Core;
using ARS408.Forms;
using ARS408.Model;
using CommonLib.Enums;
using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARS408
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            BaseFunc.InitConfigs(); //配置初始化
            #region 测试
            #endregion

            int threat = BaseFunc.GetThreatLevelByValue(3);
            threat = BaseFunc.GetThreatLevelByValue(2);
            threat = BaseFunc.GetThreatLevelByValue(1.7);
            threat = BaseFunc.GetThreatLevelByValue(1.5);
            threat = BaseFunc.GetThreatLevelByValue(1.3);
            threat = BaseFunc.GetThreatLevelByValue(1);
            threat = BaseFunc.GetThreatLevelByValue(0.5);
            //string test = new string("12345".ToCharArray().Reverse().ToArray());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string argstring = args == null ? string.Empty : ";" + string.Join(";", args).ToUpper() + ";";
            int temp = 1;
            if (temp == 2)
                argstring = ";SINGLE;";
            bool startAsSingle = argstring.Contains(";SINGLE;"); //是否以单独显示窗口显示
            //Form form = argstring.Contains(";SINGLE;") ? (Form)new FormDisplay() : new FormMain();
            BaseConst.Log.WriteLogsToFile("进入程序入口点，是否以独立模式启动: " + startAsSingle.ToString());
            Form form = startAsSingle ? (Form)new FormDisplay(new Radar()) : new FormMain();

            Application.Run(form);
        }
    }
}
