using dotNetLab.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Test_Key_Value_DB
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        public static SQLiteDBEngine CompactDB; 

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //You may not add "System.Data.SQLite.dll" etc. dll Files
            //using this Key-value Sqlite DB please make sure your project's "Target Platform" selected as X86 or X64
            //使用这个本dll时不需要去添加例如“System.Data.SQLite.dll” 文件的引用。
            //在使用之前请确保CPU 目标平台为X86或者x64,可以自定义数据库文件路径，这里默认使用“执行目录下/shikii.db”这个路径。
            CompactDB = new SQLiteDBEngine();
            Application.Run(new Form1());
        }
    }
}
