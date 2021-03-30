using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Core
{
    public static class MatlabFunctions
    {
        [DllImport(@"..\..\..\..\DLL\MATLAB_2_Cplus_1_Win32.dll", CallingConvention = CallingConvention.Cdecl)]
        //[DllImport(@"E:\Downloads\TIM\支持向量机DLL\MATLAB_2_Cplus_1_Win32.dll", EntryPoint = "SVM_model", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private extern static double SVM_model(double[] parameters);

        /// <summary>
        /// 通过给定数组计算是否出垛边
        /// </summary>
        /// <param name="paras">储存数据的数组，长度75</param>
        /// <returns></returns>
        public static bool IsOutOfStack(double[] paras)
        {
            return paras == null || paras.Length < 75 ? true : SVM_model(paras) == 1;
        }
    }
}
