using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Conference_Start
{
    class Program
    {

        static void Main(string[] args)
        {
            Process[] processes = Process.GetProcessesByName(Constant.ApplicationName);
            Process[] processes2 = Process.GetProcessesByName(Constant.LyncName);

            foreach (var item in processes)
            {
                item.Kill();
            }
            foreach (var item in processes2)
            {
                item.Kill();
            }

            Process process = new Process();
            process.StartInfo.FileName = Constant.ApplicationFullName;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
        }
    }
}
