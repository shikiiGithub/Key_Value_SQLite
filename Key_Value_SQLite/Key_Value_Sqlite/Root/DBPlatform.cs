using dotNetLab.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

namespace dotNetLab.Data
{

    public class DbInteractiveArgs
    {
        public DbCommand ThisDbCommand
        {
            get;
            set;
            
        }
        public DbConnection ThisDbConnection
        {
            get;

            set;
            
        }
        public DbParameter ThisDBPar{get;set;}
  
        public String Database{get;set;}
        public String Password { get; set; }
        public String UserName { get; set; }


    }
    public partial  class DBPlatform: IReadableLog
    {
        public DbInteractiveArgs DbInteractiveArgs { get; set; }
        public bool Status
        {
            get;

            set;
        }
        public int Port { get; set; }



        public string BinaryParam = "@data";
        public readonly string CREATEVIEW = "CREATE VIEW {0} AS SELECT {1} FROM  {2};";
        public const string EMBEDDEDTABLEDEF = "Name nvarchar(128) primary key,Val nvarchar(512) not null";

      
        public  DBPlatform()
        {
           
           

        }
     
      public void PerformErrorHandler(Object o, Exception e)
        {
            ErrorHandler?.Invoke(o, e);
        }
        
        public void FreeDataTable( DataTable dt)
        {
           
            if (dt != null)
            {
                dt.Clear();
                dt.Dispose();

            }
            
        }


        public void FastQueryData( string sql,
            Action<Object> QueriedDataCallback,
            Action EndQueryedRowCallback=null,
            DbInteractiveArgs args=null)
        {
                DbCommand cmd = null;
            try
            {
                if (args == null)
                    cmd = DbInteractiveArgs.ThisDbCommand;
                else
                    cmd = args.ThisDbCommand;
                cmd.CommandText = sql;

                DbDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)//如果有数据
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++) //逐个字段的遍历
                        {
                           //示例
                            //if (NeedQuote(AllTableTypes[i]))
                            //    sb.Append($"'{reader[i].ToString()},'");
                            //else
                            //    sb.Append($"'{reader[i]}',");

                            QueriedDataCallback(reader[i]);
                        }

                        EndQueryedRowCallback?.Invoke();

                    }
                }

                //先关闭Reader
                reader.Close();
            }
            catch (Exception ex)
            {
                ErrorHandler?.Invoke(cmd,ex);
            }
        }
        public bool NeedQuote(string type)
        {
            type = type.ToLower();
            if (type.Contains("text") || type.Contains("char") || type == "datetime" || type.Contains("time") || type.Contains("date"))
                return true;
            else
              return  false;
        }
        public string TypeInfer(string s)
        {
            string type = s;
            if (type.Contains("text") || type.Contains("char"))
                type = "String";
            if (type == "bigint")
                type = "long";
            if (type == "smallint")
                type = "short";
            if (type == "tinyint")
            {
                type = "byte";
            }
            if (type == "real")
                type = "double";
            if (type == "datetime" || type.Contains("time") || type.Contains("date"))
                type = "DateTime";
            if (type.Contains("blob") || type.Contains("binary") || type.Contains("image"))
                type = "byte []";
            return type;
        }
        public DataTable ProvideTable(string sql,DbInteractiveArgs args = null )
        {
            DataTable dt = null ;
            DbCommand cmd = null;

            if (args == null)
            {
                cmd = this.DbInteractiveArgs.ThisDbCommand;
            }
            else
                cmd = args.ThisDbCommand;
            try
            {
                  dt = new DataTable() ;
              
                cmd.CommandText = sql;
                
                  DbDataReader  reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    reader.Close();
                this.Status = true;
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(cmd, e);
                return null;
            }

            return dt;
        }
        public string UniqueResult(string sql, DbInteractiveArgs args = null)
        {
            DbCommand cmd = null;

            if (args == null)
            {
                cmd = this.DbInteractiveArgs.ThisDbCommand;
            }
            else
                cmd = args.ThisDbCommand;

            cmd.CommandText = sql;
            String strResult = null;
            try
            {
               
                    strResult = cmd.ExecuteScalar().ToString();
                    this.Status = true;
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(cmd,e);
                return null;
            }
            return strResult;

        }
        
        public void RemoveTable(string strTableName, DbInteractiveArgs args = null)
        {

                DbCommand cmd = null;
            try
            {

                if (args == null)
                {
                    cmd = this.DbInteractiveArgs.ThisDbCommand;
                }
                else
                    cmd = args.ThisDbCommand;
                cmd.CommandText = string.Format("drop table  {0} ;", strTableName);
                
                    cmd.ExecuteNonQuery();
                    this.Status = true;
 
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(cmd,e);

            }
        }
     
        public void RemoveRecord(string TableName, String strRequirement, DbInteractiveArgs args = null)
        {
                DbCommand cmd = null;
            try
            {

                if (args == null)
                {
                    cmd = this.DbInteractiveArgs.ThisDbCommand;
                }
                else
                    cmd = args.ThisDbCommand;
                cmd.CommandText = string.Format("Delete from  {0} where {1} ;", TableName, strRequirement);
                
                    cmd.ExecuteNonQuery();
                    this.Status = true;

                
                
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(cmd, e);
            }
        }
        public void Update(string TableName, string strColumnAssignAndRequirment, DbInteractiveArgs args = null)
        {
            DbCommand cmd = null;
            try
            {

                if (args == null)
                {
                    cmd = this.DbInteractiveArgs.ThisDbCommand;
                }
                else
                    cmd = args.ThisDbCommand;
                cmd.CommandText =
                 string.Format
                    ("update  {0} set {1}",
                    TableName, strColumnAssignAndRequirment);
                 
                    cmd.ExecuteNonQuery();
                    this.Status = true;
            }
            catch (Exception e)
            {
                 
                ErrorHandler?.Invoke(cmd, e);

            }
        }
        public virtual void NewDB(string DBName, DbInteractiveArgs args = null)
        {

            DbCommand cmd = null;
            try
            {

                if (args == null)
                {
                    cmd = this.DbInteractiveArgs.ThisDbCommand;
                }
                else
                    cmd = args.ThisDbCommand;
                cmd.CommandText = string.Format("create database {0} ;", DBName);
                 
                    cmd.ExecuteNonQuery();
                    this.Status = true;
                 
            }
            catch (Exception e)
            {
                ErrorHandler(cmd, e);
            }

        }
        public void RemoveDB(string strDBName, DbInteractiveArgs args = null)
        {
            DbCommand cmd = null;
            try
            {

                if (args == null)
                {
                    cmd = this.DbInteractiveArgs.ThisDbCommand;
                }
                else
                    cmd = args.ThisDbCommand;
                cmd.CommandText = string.Format("DROP database {0} ;", strDBName);
                
                    cmd.ExecuteNonQuery();
                    this.Status = true;

                 
            }
            catch (Exception e)
            {
                ErrorHandler(cmd, e);
            }
        }
        public void NewTable(string tablename, string tableDef, DbInteractiveArgs args = null)
        {
            DbCommand cmd = null;
            try
            {

                if (args == null)
                {
                    cmd = this.DbInteractiveArgs.ThisDbCommand;
                }
                else
                    cmd = args.ThisDbCommand;
                cmd.CommandText = string.Format("create table {0}({1}) ;",
                    tablename, tableDef);
                
                    cmd.ExecuteNonQuery();
                    this.Status = true;

               
            }
            catch (Exception e)
            {

                ErrorHandler(cmd, e);
            }

        }
        public void NewRecord(string strTableName, string strValue,DbInteractiveArgs args = null)
        {
            DbCommand cmd = null;
            try
            {

                if (args == null)
                {
                    cmd = this.DbInteractiveArgs.ThisDbCommand;
                }
                else
                    cmd = args.ThisDbCommand;
                cmd.CommandText = string.Format("insert into {0} values({1})", strTableName, strValue);
                // Console.WriteLine(cmd.CommandText);
                
                    cmd.ExecuteNonQuery();
                    this.Status = true;
  
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(cmd,e);
            }

        }
        // strColumnNames such as " age,sex,... "
        public void NewView(string ViewName, string strTableName, string strColumnNames, DbInteractiveArgs args = null)
        {
            DbCommand cmd = null;
            try
            {

                if (args == null)
                {
                    cmd = this.DbInteractiveArgs.ThisDbCommand;
                }
                else
                    cmd = args.ThisDbCommand;

                  cmd.CommandText =
                    string.Format(CREATEVIEW, ViewName, strColumnNames, strTableName);
               
                    cmd.ExecuteNonQuery();
                    this.Status = true;
                
                 
            }
            catch (Exception ex)
            {

                ErrorHandler(cmd, ex);
            }

        }
        public void NewKeyValueView(string ViewName, string strFromTableName,
            string strColumnName_Name, string strColumnName_Val, DbInteractiveArgs args = null)
        {
            string str = string.Format("{0} as Name,{1} as Val",
                strColumnName_Name, strColumnName_Val
                );
            NewView(ViewName, strFromTableName, str,args);
        }
        public void ExecuteNonQuery(string sql, DbInteractiveArgs args = null)
        {
            DbCommand cmd = null;
            try
            {

                if (args == null)
                {
                    cmd = this.DbInteractiveArgs.ThisDbCommand;
                }
                else
                    cmd = args.ThisDbCommand;

                cmd.CommandText = sql;
                
                    cmd.ExecuteNonQuery();
                    this.Status = true;
               
            }
            catch (Exception e)
            {
                ErrorHandler(cmd, e);
            }

        }
       

        protected bool IsDouble(string str)
        {
            try
            {
                double i = Convert.ToDouble(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
        protected bool IsDateTime(string str)
        {
            try
            {
                DateTime ui = Convert.ToDateTime(str);
                return true;
            }
            catch
            {
                return false;
            }

        }
     
        
        public String BinaryToBase64Str(String strFileName)
        {


            FileStream fs = new FileStream(strFileName, FileMode.Open);
            byte[] arr = new byte[fs.Length];
            fs.Read(arr, 0, (int)fs.Length);

            fs.Close();
            fs.Dispose();
            return Convert.ToBase64String(arr); ;
        }
        public byte[] Base64StrToBinary(String strBase64Binary)
        {

            return Convert.FromBase64String(strBase64Binary);
        }

        public bool Connect(DbInteractiveArgs args)
        {
            try
            {
                
                args.
                ThisDbConnection?.Open();
                if (args.ThisDbConnection != null && args.ThisDbCommand != null)
                {
                    args.ThisDbCommand.Connection = args.ThisDbConnection;
                }
                this.bConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                bConnected = false;
                return false;

            }


        }

        public List<string> GetNameColumnValues(string strTableName, DbInteractiveArgs args = null)
        {
            try
            {
                DataTable dt = this.ProvideTable(String.Format("select Name from {0}; ", strTableName), args);
                if (dt == null)
                    return new List<string>();
                if (dt != null)
                {
                    if (dt.Rows.Count == 0)
                        return new List<String>();
                }
                List<String> lst = new List<String>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    lst.Add(dt.Rows[i][0].ToString());
                }
                return lst;
            }
            catch (Exception e)
            {

                return null;
            }
        }

        public void CreateKeyValueTable(string strTableName, DbInteractiveArgs args = null)
        {
            this.NewTable(strTableName, EMBEDDEDTABLEDEF,args);
        }

        public void CopyKeyValueTable(string oldTableName, string newTableName, DbInteractiveArgs args = null)
        {
            CreateKeyValueTable(newTableName);
            this.ExecuteNonQuery(
                string.Format("insert into {0} SELECT * FROM {1}"
                , newTableName, oldTableName)
                , args);
        }

        protected bool bConnected = false;
        public const char SPLITMARK = '^';
        public const string FIELD_VALUE = "Val";
        public const string FIELD_NAME = "Name";
        protected const string BADCONNECTION = "未能连接本地数据库";
        protected int nError = -99999;

        public event ErrorHandler ErrorHandler;
        public event InfoHandler InfoHandler;
    }
}
