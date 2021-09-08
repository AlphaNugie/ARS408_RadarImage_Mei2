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
using System.Web;
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
            //Radar radara = new Radar(), radarb = null;
            //bool res = radara == radarb;
            //return;

            //string source = "首钢京唐A"; //源中文字符串
            //byte[] bytes = Encoding.Default.GetBytes(source); //用默认编码编码为byte数组
            //int[] numbers = bytes.Select(b => b > 127 ? b - 256 : b).ToArray(); //调整取值范围（AB公司PLC内字符串每一位字符取值范围为-128~127）
            ////int[] numbers = new int[] { -54, -41, -72, -42, -66, -87, -52, -58, 65 };
            ////方法1：转化为URL编码并通过GB2312解码为字符串
            //string step1 = string.Join(string.Empty, numbers.Select(n => n < 0 ? n + 256 : n).Select(n => '%' + n.ToString("X2")).ToArray());
            //string test = HttpUtility.UrlDecode(step1, Encoding.GetEncoding("GB2312"));
            ////string test = HttpUtility.UrlDecode("%CA%D7%B8%D6%BE%A9%CC%C6%41", Encoding.GetEncoding("GB2312"));
            ////方法2：调整回byte类型取值范围并解码为字符串
            //byte[] step2 = numbers.Select(n => (byte)(n < 0 ? n + 256 : n)).ToArray();
            //test = Encoding.Default.GetString(step2);
            //return;

            //Distance dist = new Distance(true, 15, 20);
            //dist.SlideIntoRunway();
            //int[] levels = new int[] { 0, 1, 2, 1, 0, 2, 0, 3, 0, 1, 3, 1, 2, 3, 2, 0, 1, 1, 1, 2, 3, 0, 3, 0, 1 };
            //foreach (var level in levels)
            //    dist.Level = level;
            //double[] values = new double[] { 12, 11, 9, 7.5, 6, 1, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 1.5, 3, 3, 3, 3, 3, 3, 8, 8, 8, 8, 8, 15, 15, 15, 15, 15, 15, 15 };
            //foreach (var value in values)
            //    dist.SetValue(value);

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
