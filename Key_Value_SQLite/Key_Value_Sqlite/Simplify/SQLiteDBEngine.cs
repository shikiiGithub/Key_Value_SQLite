using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace dotNetLab.Data
{
    public class SQLiteDBEngine : DBPlatform, IKeyValue
    {
        public SQLiteDBEngine(String DBPath = "shikii.db")
        {
            CheckSQLiteRefFiles();
            this.DbInteractiveArgs = this.GetDbInteractiveArgs();
            InitDB(DBPath, DbInteractiveArgs);
        }

        void InitDB(String   strCmpactDBFilePath , DbInteractiveArgs InitSqliteArgs)
        {
            try
            {
             
                DbInteractiveArgs args = InitSqliteArgs;
                args.Database = strCmpactDBFilePath;
                args.ThisDbConnection.ConnectionString =String.Format("data source={0}", strCmpactDBFilePath);

                bool b = this.Connect(args);
                if (b)
                {
                    List<String> AllTableNames =  GetAllTableNames(args);
                    int nIndex = -1;
                    nIndex = (int)AllTableNames?.IndexOf( DefaultTable);
                    int nUpperIndex = (int)AllTableNames?.IndexOf( DefaultTable.ToUpper());
                    if (nIndex == -1 && nUpperIndex == -1)
                    {
                        CreateKeyValueTable( DefaultTable, args);

                    }

                    Console.WriteLine("已经成功连接数据库");

                }
                else
                {
                    Console.WriteLine("未能连接数据库");
                }
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }

        }

        public List<string> GetAllTableNames(  DbInteractiveArgs args = null)
        {
            DataTable dt = null;List<String> TableNames = new List<string>() ;
            try
            {
                
                dt = this.ProvideTable("select name from sqlite_master where type='table' or type='view' order by name;", args);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TableNames.Add(dt.Rows[i][0].ToString());
                }
                return TableNames;
            }
            catch (Exception e)
            {
                PerformErrorHandler(dt, e);
                return null;
            }
        }

        public List<string> GetAllViewNames(  DbInteractiveArgs args = null)
        {
            List<String> AllTableNames = new List<string>();
            DataTable dt = null;
            try
            {
                string sql = "select name from sqlite_master where  type='view' order by name;";
                 dt = this.ProvideTable(sql, args);

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    AllTableNames.Add(dt.Rows[i][0].ToString());
                }
                return AllTableNames;
            }
            catch (Exception ex)
            {

                PerformErrorHandler(dt, ex);
                return null;
            }
            

        }
        public List<string> GetTableColumnNames(String tableName, DbInteractiveArgs args = null)
        {
            try
            {

            DataTable dt;
            List<String> AllColumnNames = new List<string>();

            string SQLFormat = "PRAGMA table_info({0})";
            dt = this.ProvideTable(String.Format(SQLFormat, tableName), args);
            for (int i = 0; i < dt.Rows.Count; i++)
                AllColumnNames.Add(dt.Rows[i][1].ToString());
                return AllColumnNames;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public List<string> GetTableColumnTypes(string tableName, DbInteractiveArgs args = null)
        {
            string sql = String.Format("PRAGMA table_info ({0})",tableName) ;
            DataTable dt =  ProvideTable(sql,args);
            List<String> typeList = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String type = dt.Rows[i]["type"].ToString().ToLower();
                type = TypeInfer(type);
                typeList.Add(type);
            }
            return typeList;
        }

         bool IsX86Architecture(string filePath)
        {
            ushort architecture = 0;
            try
            {
                using (System.IO.FileStream fStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    using (System.IO.BinaryReader bReader = new System.IO.BinaryReader(fStream))
                    {
                        if (bReader.ReadUInt16() == 23117)
                        {
                            fStream.Seek(0x3A, System.IO.SeekOrigin.Current);
                            fStream.Seek(bReader.ReadUInt32(), System.IO.SeekOrigin.Begin);
                            if (bReader.ReadUInt32() == 17744)
                            {
                                fStream.Seek(20, System.IO.SeekOrigin.Current);
                                architecture = bReader.ReadUInt16();
                            }
                        }
                    }
                }
            }
            catch { }
            if (architecture == 0x10b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        void CheckSQLiteRefFiles()
        {


            bool isX86 = IsX86Architecture(Application.ExecutablePath);
         
            Action<bool> AddSQLiteRefFiles = (_isX86) =>
            {
                AddRef("System.Data.SQLite.dll", Key_Value_Sqlite.Res.System_Data_SQLite);
                if (_isX86)
                {
                    AddRef("SQLite.Interop.dll", Key_Value_Sqlite.Res.SQLite_Interop_x86);
                    AddRef("sqlite3.dll", Key_Value_Sqlite.Res.sqlite3_x86);
                }
                else
                {

                    AddRef("SQLite.Interop.dll", Key_Value_Sqlite.Res.SQLite_Interop_x64);
                    AddRef("sqlite3.dll", Key_Value_Sqlite.Res.sqlite3_x64);
                }
            };
            if (!File.Exists("System.Data.SQLite.dll"))
            {
                AddSQLiteRefFiles(isX86);
            }
            else
            {
                bool isDllX86 = IsX86Architecture("SQLite.Interop.dll");
                if (isDllX86 != isX86)
                {
                    AddSQLiteRefFiles(isX86);
                }
            }
        }

      
          object GetReflectOject(string strDllPath, string strObjectFullName)
        {
            return System.Activator.CreateInstanceFrom(
             strDllPath, strObjectFullName).Unwrap();

        }
        public DbInteractiveArgs GetDbInteractiveArgs()
        {
            DbInteractiveArgs args = new DbInteractiveArgs();
            args.ThisDbConnection =
               this.GetReflectOject("System.Data.SQLite.dll", "System.Data.SQLite.SQLiteConnection")
                as DbConnection;
            args.ThisDBPar =
                GetReflectOject("System.Data.SQLite.dll", "System.Data.SQLite.SQLiteParameter")
                as DbParameter;
            args.ThisDbCommand =
              GetReflectOject("System.Data.SQLite.dll", "System.Data.SQLite.SQLiteCommand")
               as DbCommand;
            args.ThisDBPar.ParameterName = "Data";
            args.ThisDBPar.Value = System.Data.SqlDbType.Image;
            return args;
        }
        public static void AddRef(string strFileName, byte[] bytArr)
        {
            FileStream fs = new FileStream(strFileName, FileMode.Create);

            fs.Write(bytArr, 0, bytArr.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();

        }

    }
}
