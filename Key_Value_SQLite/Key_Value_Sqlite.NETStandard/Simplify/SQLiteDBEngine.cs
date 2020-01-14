using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace dotNetLab.Data
{
    public class SQLiteDBEngine : DBPlatform, IKeyValue
    {
        public static Assembly assembly_Sqlite_Connection;
        String SqliteDBFilePath;
        public SQLiteDBEngine(String SqliteDBFilePath)
        {
            this.SqliteDBFilePath = SqliteDBFilePath;
        }

        public DbInteractiveArgs Connect()
        {
            try
            {
                DbInteractiveArgs args = new DbInteractiveArgs();


                String name_lower = null;
                if (assembly_Sqlite_Connection != null)
                {

                    Type[] types = assembly_Sqlite_Connection.GetTypes();

                    for (int i = 0; i < types.Length; i++)
                    {
                        name_lower = types[i].Name.ToLower();
                        if (name_lower.Equals("sqliteconnection"))
                            args.ThisDbConnection = assembly_Sqlite_Connection.CreateInstance(types[i].FullName) as DbConnection;
                        else if (name_lower.Equals("sqlitecommand"))
                            args.ThisDbCommand = assembly_Sqlite_Connection.CreateInstance(types[i].FullName) as DbCommand;
                        else if (name_lower.Equals("sqliteparameter"))
                        {
                            args.ThisDBPar = assembly_Sqlite_Connection.CreateInstance(types[i].FullName) as DbParameter;
                        }



                    }

                }
                else
                    return null;

                args.Database = SqliteDBFilePath;
                String strCmpactDBFilePath = SqliteDBFilePath;


                args.Database = strCmpactDBFilePath;
                args.ThisDbConnection.ConnectionString = $"data source={strCmpactDBFilePath}";
                if (DbInteractiveArgs == null)
                    DbInteractiveArgs = args;
                bool b = Connect(args);
                if (!b)
                {
                    PerformErrorHandler(this, new Exception("未能连接数据库"));
                    args = null;
                }
                return args;

            }
            catch (Exception exx)
            {

              
                return null;
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
    
       
      

    }
}
