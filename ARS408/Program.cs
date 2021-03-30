using ARS408.Core;
using ARS408.Forms;
using ARS408.Model;
using CommonLib.Clients;
using CommonLib.Enums;
using CommonLib.Extensions;
using CommonLib.Function;
using ProtoBuf;
using SerializationFactory;
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
            //Distance dist = new Distance(true, 50, 42);
            //dist.DistCorr = -4;
            ////dist.SlideIntoRunway();
            //while (true)
            //{
            //    dist.SetValue(8);
            //}

            //GlitchFilter[] glitchs = new GlitchFilter[6];
            //for (int i = 0; i < glitchs.Length; i++)
            //    glitchs[i] = new GlitchFilter(true, 6, 5) { LongDistAvoidEnabled = true, LongDist2Avoid = BlockConst.DefaultDistance, LongDistCountLimit = BlockConst.LongDistCountLimit, AverageGapEnabled = false, FixedGapEnabled = true, FixedGapThres = 0.7, FixedGapCountLimit = 3 };
            //string filePath = @"D:\煤二期\Documents\雷达数据\MATLAB处理\防碰\原始网格数据-20210127173130.csv";
            //List<double[]> list = File.ReadAllLines(filePath).Skip(1).Select(line => line.Split(',').Skip(1).Select(p => double.Parse(p)).ToArray()).ToList();
            //GlitchFilter glitch = glitchs[3]; //右前
            //foreach (var numbers in list)
            //{
            //    double right_front = numbers[3];
            //    glitch.PushValue(ref right_front);
            //    right_front = glitch.CurrVal;
            //}
            //return;
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException_Raising); //未捕获异常触发事件
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

        #region 事件
        /// <summary>
        /// 未捕获异常触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void UnhandledException_Raising(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            FileClient.WriteFailureInfo(new string[] { string.Format("未处理异常被触发，运行时是否即将终止：{0}，错误信息：{1}", args.IsTerminating, e.Message), e.StackTrace, e.TargetSite.ToString() }, "UnhandledException", "unhandled " + DateTime.Now.ToString("yyyy-MM-dd"));
        }
        #endregion
    }
}
